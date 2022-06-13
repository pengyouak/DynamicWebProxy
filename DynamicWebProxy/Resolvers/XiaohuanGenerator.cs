using DynamicWebProxy.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DynamicWebProxy.Resolvers
{
    /// <summary>
    /// 快代理
    /// </summary>
    public class XiaohuanGenerator : ProxyGenerator
    {
        private string url = "https://ip.ihuan.me/";

        public override event EventHandler<GenerateLoadSucceedEventArgs>? OnGenerateLoadSucceed;

        public override event EventHandler<GenerateLoadFailedEventArgs>? OnGenerateLoadFailed;

        public override async Task GenerateProxyItems()
        {
            Source = "小幻代理";

            // 普通代理
            using var webclient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            webclient.Timeout = new TimeSpan(0, 0, 5);

            try
            {
                // 1页
                for (int i = 1; i <= 1; i++)
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, String.Format(url));
                    message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    //message.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    message.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                    message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36");
                    var resp = await webclient.SendAsync(message);
                    SetItems(resp);
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void SetItems(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode) return;

            try
            {
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(httpResponse.Content.ReadAsStringAsync().Result);
                var tr = document.DocumentNode.SelectNodes("//tr");
                if (tr == null) return;
                for (int i = 1; i < tr.Count; i++)
                {
                    var item = new ProxyItem
                    {
                        Source = Source,
                        Location = tr[i].SelectSingleNode("./td[3]").InnerText,
                        Uri = new Uri($"http://{tr[i].SelectSingleNode("./td[1]").InnerText}:{tr[i].SelectSingleNode("./td[2]").InnerText}"),
                        ProxyType = $"HTTPS{tr[i].SelectSingleNode("./td[5]").InnerText} POST{tr[i].SelectSingleNode("./td[6]").InnerText}（{tr[i].SelectSingleNode("./td[7]").InnerText}）"
                    };
                    ProxyItems.Add(item);
                }

                OnGenerateLoadSucceed?.Invoke(this, new GenerateLoadSucceedEventArgs(Source, ProxyItems.Select(x => x.Clone()).ToList()));
            }
            catch (Exception ex)
            {
                OnGenerateLoadFailed?.Invoke(this, new GenerateLoadFailedEventArgs(Source, ex.Message, ex));
            }
        }
    }
}