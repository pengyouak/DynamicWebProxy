using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class TestSucceedEventArgs : System.EventArgs
    {
        public Uri Uri { get; private set; }

        public double Delay { get; private set; }

        public string TestUrl { get; private set; }

        public TestSucceedEventArgs(Uri uri, double delay, string testUrl) : base()
        {
            Uri = uri;
            Delay = delay;
            TestUrl = testUrl;
        }
    }
}