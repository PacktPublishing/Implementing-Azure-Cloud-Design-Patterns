using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace CircuitBreakerSample.MongoDatabase
{
    public class OrderRepository : IOrderRepository
    {
        private MongoClient _client;
        public OrderRepository(MongoClient client) 
        {
            _client = client;
            
        }
        public virtual List<Order> Read()
        {
            return _client
                    .GetDatabase(Config.DbName)
                    .GetCollection<Order>("orders")
                    .Find(new BsonDocument())
                    .ToList();
        }
        public virtual void Write(Order order) 
        {
            _client
              .GetDatabase(Config.DbName)
              .GetCollection<Order>("orders")
              .InsertOne(order);
        }
    }
}