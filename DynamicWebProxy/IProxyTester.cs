using DynamicWebProxy.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy
{
    public interface IProxyTester
    {
        public TimeSpan Timeout { get; }

        public event EventHandler<TestSucceedEventArgs> OnTestSucceed;

        public event EventHandler<TestFailedEventArgs> OnTestFailed;

        double SpeedTest(ProxyItem proxyItem, string testUrl);

        Task<double> SpeedTestAsync(ProxyItem proxyItem, string testUrl);
    }
}