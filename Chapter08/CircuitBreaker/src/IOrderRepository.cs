using System.Collections.Generic;
namespace CircuitBreakerSample 
{
    public interface IOrderRepository
    {
        List<Order> Read();

        void Write(Order order);
    }
}