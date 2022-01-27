using MongoDB.Driver;
using Sayuru.Mobile.API.Interfaces;
using Sayuru.Mobile.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API
{
    public class DBContext : IDBContext
    {
        public DBContext(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            UserData = database.GetCollection<UserData>("UserData");
            UserSecurity = database.GetCollection<UserSecurity>("UserSecurity");
            UserLocations = database.GetCollection<UserLocations>("UserLocations");
            UserRoutes = database.GetCollection<UserRoutes>("UserRoutes");
            UserAlerts = database.GetCollection<UserAlerts>("UserAlerts");
            UserActivityLog = database.GetCollection<UserActivityLog>("UserActivityLog");
            DOF = database.GetCollection<DOF>("DOF");
        }

        public IMongoCollection<UserData> UserData { get; }
        public IMongoCollection<UserSecurity> UserSecurity { get; }
        public IMongoCollection<UserLocations> UserLocations { get; }
        public IMongoCollection<UserRoutes> UserRoutes { get; }
        public IMongoCollection<UserAlerts> UserAlerts { get; }
        public IMongoCollection<UserActivityLog> UserActivityLog { get; }
        public IMongoCollection<DOF> DOF { get; }

        public void AddOne() 
        {
        }
    }
}
