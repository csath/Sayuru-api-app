using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sayuru.Mobile.API.Helpers
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; } = true;
        public int Code { get; set; } = 200;
        public T Data { get; set; }
        public string Error { get; set; }
    }
}
