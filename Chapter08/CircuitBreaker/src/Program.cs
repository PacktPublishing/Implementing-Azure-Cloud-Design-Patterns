using System;
using System.Threading;

using CircuitBreakerSample.MongoDatabase;
namespace CircuitBreakerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            int requestToSend = 100;
            // Create a circuit breaker repository
            IOrderRepository repository = new CircuitBreakerRepository(ServiceConnectionFactory.Connection);
            for (int i = 0 ; i < requestToSend; i++) 
            {
                try 
                {
                    ReadOrWrite(repository, i);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
                }
                Thread.Sleep(1000);
            }
        }
        static void ReadOrWrite(IOrderRepository repository, int i) 
        {
            var random = new Random();
            
            if (random.Next(50) > 25) 
            {
                // make a write
                var order = new Order
                {
                    ID = i,
                    Value = 100 + i
                };
                repository.Write(order);
                Console.WriteLine($"Write Request: {order}");
                Console.WriteLine("");
            }
            else 
            {
                // make a read
                Console.WriteLine($"Read Request: {string.Join(", ", repository.Read())}");
                Console.WriteLine("");
            }
        }
    }
}
