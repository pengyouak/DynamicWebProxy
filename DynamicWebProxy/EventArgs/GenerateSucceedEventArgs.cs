using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.EventArgs
{
    public class GenerateSucceedEventArgs : System.EventArgs
    {
        public string Source { get; private set; }

        public bool IsDefault { get; private set; }

        public ProxyItem ProxyItem { get; private set; }

        public GenerateSucceedEventArgs(string source, ProxyItem proxyItem, bool isDefault) : base()
        {
            Source = source;
            ProxyItem = proxyItem;
            IsDefault = isDefault;
        }
    }
}