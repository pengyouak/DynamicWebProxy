using DynamicWebProxy.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy
{
    // https://developer.aliyun.com/article/644876 代理汇总
    public abstract class ProxyGenerator
    {
        public int MaxValidTimes { get; set; } = 3;

        public string Source { get; protected set; } = string.Empty;

        public string TestUrl { get; set; } = "http://icanhazip.com/";

        public bool Available => ProxyItems.Count > 0 && !ProxyItems.TrueForAll(x => x.ProxyStatus == ProxyStatus.Invalid);

        protected IProxyTester ProxyTester { get; private set; } = new ProxyTester();

        protected List<ProxyItem> ProxyItems = new List<ProxyItem>();

        public event EventHandler<GenerateSucceedEventArgs>? OnGenerateSucceed;

        public event EventHandler<GenerateFailedEventArgs>? OnGenerateFailed;

        public event EventHandler<GenerateTestEventArgs>? OnGenerateTestHit;

        public virtual event EventHandler<GenerateLoadSucceedEventArgs>? OnGenerateLoadSucceed;

        public virtual event EventHandler<GenerateLoadFailedEventArgs>? OnGenerateLoadFailed;

        public ProxyGenerator()
        {
            ProxyTester.OnTestFailed += (s, e) => OnGenerateTestHit?.Invoke(this, new GenerateTestEventArgs
            {
                Source = Source,
                Available = false,
                Delay = -1,
                Exception = e.Exception,
                Message = e.Message,
                TestUrl = e.TestUrl,
                Uri = e.Uri
            });

            ProxyTester.OnTestSucceed += (s, e) => OnGenerateTestHit?.Invoke(this, new GenerateTestEventArgs
            {
                Source = Source,
                Available = true,
                Delay = e.Delay,
                Message = $"测试成功，访问速度 {e.Delay}ms",
                TestUrl = e.TestUrl,
                Uri = e.Uri
            });
        }

        /// <summary>
        /// 从指定Url初始化ProxyItems
        /// </summary>
        public abstract Task GenerateProxyItems();

        public virtual async Task<IWebProxy> GenerateProxy()
        {
            var item = await GetOneProxyItem();
            if (item == null)
            {
                OnGenerateSucceed?.Invoke(this, new GenerateSucceedEventArgs(Source, null, true));
                return WebRequest.GetSystemWebProxy();
            }

            OnGenerateSucceed?.Invoke(this, new GenerateSucceedEventArgs(Source, item, false));
            return new WebProxy(item.Uri);
        }

        protected virtual async Task<ProxyItem?> GetOneProxyItem()
        {
            if (ProxyItems.Count == 0)
            {
                OnGenerateFailed?.Invoke(this, new GenerateFailedEventArgs(Source, "代理池中的数据为空，使用系统默认代理"));
                return null;
            }

            var item = ProxyItems.Where(x => x.ProxyStatus != ProxyStatus.Invalid).OrderBy(x => x.HitCount).FirstOrDefault();
            if (item == null)
            {
                OnGenerateFailed?.Invoke(this, new GenerateFailedEventArgs(Source, "已经没有可用的代理了，使用系统默认代理"));
                return null;
            }

            item.HitCount++;

            var delay = await ProxyTester.SpeedTestAsync(item, TestUrl);
            if (delay > 0)
            {
                item.Delay = delay;
                item.ProxyStatus = ProxyStatus.Valid;
            }
            else
            {
                if (item.InValidTimes >= MaxValidTimes)
                    item.ProxyStatus = ProxyStatus.Invalid;
                else
                    item.InValidTimes++;
                return await GetOneProxyItem();
            }

            return item;
        }

        public virtual async Task TestAll()
        {
            Parallel.ForEach(ProxyItems, async item =>
            {
                var delay = await ProxyTester.SpeedTestAsync(item, TestUrl);
                if (delay > 0)
                {
                    item.Delay = delay;
                    item.ProxyStatus = ProxyStatus.Valid;
                }
                else
                {
                    item.ProxyStatus = ProxyStatus.Invalid;
                }
            });

            await Task.Yield();

            //OnGenerateTestHit?.Invoke(this, new GenerateTestEventArgs
            //{
            //    Source = Source,
            //    Available = ProxyItems.Any(x => x.ProxyStatus == ProxyStatus.Valid),
            //    Delay = -1,
            //    Message = $"本组IP全部测试完毕，可用数量{ProxyItems.Count(x => x.ProxyStatus == ProxyStatus.Valid)}",
            //    TestUrl = TestUrl,
            //});
        }
    }
}