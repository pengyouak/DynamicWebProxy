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
    public class MipuGenerator : ProxyGenerator
    {
        private string url = "https://proxy.mimvp.com/freeopen?proxy={0}";

        public override event EventHandler<GenerateLoadSucceedEventArgs>? OnGenerateLoadSucceed;

        public override event EventHandler<GenerateLoadFailedEventArgs>? OnGenerateLoadFailed;

        public override async Task GenerateProxyItems()
        {
            Source = "米扑代理";

            // 普通代理
            var webclient = new HttpClient();
            webclient.Timeout = new TimeSpan(0, 0, 5);

            try
            {
                // 5页
                for (int i = 1; i <= 1; i++)
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, String.Format(url, "in_tp"));
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
                        Location = tr[i].SelectSingleNode("./td[6]").InnerText,
                        Uri = new Uri($"{tr[i].SelectSingleNode("./td[4]").InnerText}://{tr[i].SelectSingleNode("./td[2]").InnerText}:{tr[i].SelectSingleNode("./td[3]").InnerText}"),
                        ProxyType = $"{tr[i].SelectSingleNode("./td[4]").InnerText}（{tr[i].SelectSingleNode("./td[5]").InnerText}）"
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