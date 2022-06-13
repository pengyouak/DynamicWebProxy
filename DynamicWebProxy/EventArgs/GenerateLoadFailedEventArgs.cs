using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class GenerateLoadFailedEventArgs
    {
        public string Source { get; private set; }

        public string Message { get; private set; }

        public Exception Exception { get; private set; }

        public GenerateLoadFailedEventArgs(string source, string message, Exception exception = null) : base()
        {
            Source = source;
            Message = message;
            Exception = exception;
        }
    }
}