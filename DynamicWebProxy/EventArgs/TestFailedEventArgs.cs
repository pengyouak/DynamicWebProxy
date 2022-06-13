using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class TestFailedEventArgs : System.EventArgs
    {
        public Uri Uri { get; private set; }

        public string TestUrl { get; private set; }

        public string Message { get; private set; }

        public Exception Exception { get; private set; }

        public TestFailedEventArgs(Uri uri, string testUrl, string message, Exception ex = null) : base()
        {
            Uri = uri;
            TestUrl = testUrl;
            Message = message;
            Exception = ex;
        }
    }
}