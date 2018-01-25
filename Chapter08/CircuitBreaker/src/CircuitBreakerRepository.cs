using System;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Driver;
using CircuitBreakerSample.MongoDatabase;
using CircuitBreakerSample.Exceptions;

namespace CircuitBreakerSample
{
    public class CircuitBreakerRepository : IOrderRepository
    {
        private CircuitBreakerState _state;
        private OrderRepository _repository;

        public CircuitBreakerRepository(MongoClient client)
        {
            _state = new CircuitBreakerClosed(this);
            _repository = new OrderRepository(client);
        }

        public List<Order> Read()
        {
            return _state.HandleRead();
        }

        public void Write(Order order) 
        {
            _state.HandleWrite(order);
        }
        private abstract class CircuitBreakerState 
        {
            protected CircuitBreakerRepository _owner;
            public CircuitBreakerState(CircuitBreakerRepository owner)
            {
                _owner = owner;
            }
            public abstract List<Order> HandleRead();
            public abstract void HandleWrite(Order order); 
        }

        private class CircuitBreakerClosed : CircuitBreakerState
        {
            private int _errorCount = 0;
            public CircuitBreakerClosed(CircuitBreakerRepository owner)
                :base(owner){}
            
            public override List<Order> HandleRead()
            {
                try
                {
                    return _owner._repository.Read();       
                }
                catch (Exception e) 
                {
                    _trackErrors(e);
                    throw e;
                }
            }

            public override void HandleWrite(Order order)
            {
                try
                {
                    _owner._repository.Write(order);       
                }
                catch (Exception e) 
                {
                    _trackErrors(e);
                    throw e;
                }
            }

            private void _trackErrors(Exception e) 
            {
                _errorCount += 1;
                if (_errorCount > Config.CircuitClosedErrorLimit) //Limit of error requests to accept
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                }
            }
        }
        private class CircuitBreakerOpen : CircuitBreakerState
        {
            
            public CircuitBreakerOpen(CircuitBreakerRepository owner)
                :base(owner)
            {
                new Timer( _ => 
                { 
                    owner._state = new CircuitBreakerHalfOpen(owner); 
                }, null, Config.CircuitOpenTimeout, Timeout.Infinite);
            }

            public override List<Order> HandleRead()
            { 
                throw new CircuitOpenException();
            }
            public override void HandleWrite(Order order)
            {
                throw new CircuitOpenException();
            }
        }
        private class CircuitBreakerHalfOpen : CircuitBreakerState
        {
            private static readonly string Message = "Call failed when circuit half open";
            public CircuitBreakerHalfOpen(CircuitBreakerRepository owner)
                :base(owner){}

            public override List<Order> HandleRead()
            { 
                try 
                {
                    var result = _owner._repository.Read();
                    _owner._state = new CircuitBreakerClosed(_owner);
                    return result;
                }
                catch (Exception e) 
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                    throw new CircuitOpenException(Message, e);
                }
            }
            public override void HandleWrite(Order order)
            {
                try 
                {
                    _owner._repository.Write(order);
                    _owner._state = new CircuitBreakerClosed(_owner);
                }
                catch (Exception e) 
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                    throw new CircuitOpenException(Message, e);
                }
            }
        }
    }
}