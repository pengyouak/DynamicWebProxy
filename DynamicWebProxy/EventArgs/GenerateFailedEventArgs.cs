using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class GenerateFailedEventArgs : System.EventArgs
    {
        public string Source { get; private set; }

        public string Message { get; private set; }

        public GenerateFailedEventArgs(string source, string message) : base()
        {
            Source = source;
            Message = message;
        }
    }
}