using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class GenerateLoadSucceedEventArgs
    {
        public string Source { get; private set; }

        public List<ProxyItem> ProxyItems { get; private set; }

        public GenerateLoadSucceedEventArgs(string source, List<ProxyItem> proxyItems) : base()
        {
            Source = source;
            ProxyItems = proxyItems;
        }
    }
}