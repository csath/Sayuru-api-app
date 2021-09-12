using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Interfaces
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
