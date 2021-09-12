using Sayuru.Mobile.API.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
