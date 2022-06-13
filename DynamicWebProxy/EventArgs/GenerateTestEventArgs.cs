using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class GenerateTestEventArgs
    {
        public string Source { get; init; }

        public bool Available { get; init; }

        public Uri Uri { get; init; }

        public double Delay { get; init; }

        public string TestUrl { get; init; }

        public string Message { get; init; }

        public Exception Exception { get; init; }
    }
}