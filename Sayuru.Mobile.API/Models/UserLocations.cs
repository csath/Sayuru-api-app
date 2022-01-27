
using System;
using System.Collections.Generic;
using System.Text;
using Sayuru.Mobile.API.Common;

namespace Sayuru.Mobile.API.Models
{
    public class UserLocations : MongoEntity
    {
        public string UserId { get; set; }
        public long CreatedTimeStamp { get; set; }
        public string Name { get; set; }
        public string Timestamp { get; set; }
        public Location Point { get; set; }
    }
}
