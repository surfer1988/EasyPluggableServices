using System;
using System.Collections.Generic;
using System.Text;

namespace EasyPluggableServices
{
    public class LoadException : Exception
    {
        public LoadException(string message) : base(message) { }
        public LoadException(string message, Exception ex) : base(message, ex) { }
    }
}
