using Sayuru.Mobile.API.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Models
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
