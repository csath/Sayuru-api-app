using MongoDB.Driver;
using Sayuru.Mobile.API.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Interfaces
{
    public interface IDBContext
    {
        public IMongoCollection<UserData> UserData { get; }
        public IMongoCollection<UserSecurity> UserSecurity { get; }
        public IMongoCollection<DOF> DOF { get; }
        public IMongoCollection<UserLocations> UserLocations { get; }
        public IMongoCollection<UserRoutes> UserRoutes { get; }
        public IMongoCollection<UserAlerts> UserAlerts { get; }
        public IMongoCollection<UserActivityLog> UserActivityLog { get; }
    }
}
