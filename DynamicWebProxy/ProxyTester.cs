using DynamicWebProxy.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy
{
    public class ProxyTester : IProxyTester
    {
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 3);
        private System.Text.RegularExpressions.Regex _urlRegex = new System.Text.RegularExpressions.Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$");

        public event EventHandler<TestSucceedEventArgs>? OnTestSucceed;

        public event EventHandler<TestFailedEventArgs>? OnTestFailed;

        public async Task<double> SpeedTestAsync(ProxyItem proxyItem, string testUrl)
        {
            if (!_urlRegex.IsMatch(testUrl))
                throw new InvalidOperationException("url格式错误");

            var proxyObject = new WebProxy(proxyItem.Uri);
            proxyObject.UseDefaultCredentials = true;
            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = proxyObject,
                UseProxy = true,
                DefaultProxyCredentials = proxyObject.Credentials,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls11
                | System.Security.Authentication.SslProtocols.Tls12
                | System.Security.Authentication.SslProtocols.Tls13
            };

            var client = new HttpClient(proxyHttpClientHandler);
            var req = new HttpRequestMessage(HttpMethod.Get, testUrl);
            client.Timeout = Timeout;

            var watch = new System.Diagnostics.Stopwatch();
            try
            {
                watch.Start();
                var resp = await client.SendAsync(req);
                var content = resp.Content.ReadAsStringAsync().Result;
                resp.EnsureSuccessStatusCode();
                System.Diagnostics.Debug.WriteLine($"当前Http请求使用的IP：{content}");
                watch.Stop();

                OnTestSucceed?.Invoke(this, new TestSucceedEventArgs(proxyItem.Uri, watch.Elapsed.TotalMilliseconds, testUrl));
                return watch.Elapsed.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                OnTestFailed?.Invoke(this, new TestFailedEventArgs(proxyItem.Uri, testUrl, ex.InnerException == null ? ex.Message : ex.InnerException.Message, ex.InnerException ?? ex));
                return -1;
            }
            finally
            {
                client.Dispose();
                req.Dispose();
            }
        }

        public double SpeedTest(ProxyItem proxyItem, string testUrl)
        {
            return SpeedTestAsync(proxyItem, testUrl).GetAwaiter().GetResult();
        }
    }
}