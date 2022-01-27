
using System;
using System.Collections.Generic;
using System.Text;
using Sayuru.Mobile.API.Common;

namespace Sayuru.Mobile.API.Models
{
    public class UserRoutes : MongoEntity
    {
        public string UserId { get; set; }
        public long CreatedTimeStamp { get; set; }
        public string Name { get; set; }
        public string StartTimestamp { get; set; }
        public string EndTimestamp { get; set; }
        public double DistanceInKM { get; set; }
        public List<Location> Points { get; set; }
    }
}
