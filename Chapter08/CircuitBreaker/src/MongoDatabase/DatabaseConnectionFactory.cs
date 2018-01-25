using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CircuitBreakerSample.MongoDatabase
{
    public class ServiceConnectionFactory
    {
        private static MongoClient _client = new MongoClient(new MongoClientSettings
        {
            Server = new MongoServerAddress(Config.DbUrl),
            SocketTimeout = new TimeSpan(0, 0, 0, 2),
            WaitQueueTimeout = new TimeSpan(0, 0, 0, 2),
            ConnectTimeout = new TimeSpan(0, 0, 0, 2),
            HeartbeatTimeout = new TimeSpan(0, 0, 0, 2),
            ServerSelectionTimeout = new TimeSpan(0, 0, 0, 2)
        });

        public static MongoClient Connection => _client;
    }
}