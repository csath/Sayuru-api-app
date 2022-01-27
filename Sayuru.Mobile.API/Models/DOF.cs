
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Models
{
    public class DOF : MongoEntity
    {
        public List<Field> ButtonLinks { get; set; }
        public List<string> Images { get; set; }
        public string WebViewURL { get; set; }
    }

    public class Field
    {
        public string Label { get; set; }
        public string Url { get; set; }
    }
}
