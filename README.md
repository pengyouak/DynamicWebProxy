# DynamicWebProxy

#### 介绍
自动获取可用http代理的库

#### 使用方法

- 收集可用的Http代理数据源，并编写自己的分析器来爬取相应的Http代理信息。
- 创建代理工厂类
- 在代理工厂中添加自己写的分析器
- 获取可用代理

#### 分析器
代码中写了几个简单的示例
- KuaiGenerator
- MifengGenerator
- XiaohuanGenerator

#### 代理工厂示例

```csharp
public class ProxyHelper
    {
        private static HashSet<ProxyGenerator> ProxyGenerators = new HashSet<ProxyGenerator>();

        private static async Task Init()
        {
            ProxyGenerator xiaohuan = new XiaohuanGenerator() { MaxValidTimes = 2 };
            xiaohuan.OnGenerateLoadSucceed += OnGenerateLoadSucceed;
            xiaohuan.OnGenerateLoadFailed += OnGenerateLoadFailed;
            xiaohuan.OnGenerateSucceed += OnGenerateSucceed;
            xiaohuan.OnGenerateFailed += OnGenerateFailed;
            xiaohuan.OnGenerateTestHit += OnGenerateTestHit;
            await xiaohuan.GenerateProxyItems();

            ProxyGenerator kuai = new KuaiGenerator() { MaxValidTimes = 2 };
            kuai.OnGenerateLoadSucceed += OnGenerateLoadSucceed;
            kuai.OnGenerateLoadFailed += OnGenerateLoadFailed;
            kuai.OnGenerateSucceed += OnGenerateSucceed;
            kuai.OnGenerateFailed += OnGenerateFailed;
            kuai.OnGenerateTestHit += OnGenerateTestHit;
            await kuai.GenerateProxyItems();

            //ProxyGenerator mipu = new MipuGenerator();
            //await mipu.GenerateProxyItems();

            ProxyGenerator mifeng = new MifengGenerator() { MaxValidTimes = 2 };
            mifeng.OnGenerateLoadSucceed += OnGenerateLoadSucceed;
            mifeng.OnGenerateLoadFailed += OnGenerateLoadFailed;
            mifeng.OnGenerateSucceed += OnGenerateSucceed;
            mifeng.OnGenerateFailed += OnGenerateFailed;
            mifeng.OnGenerateTestHit += OnGenerateTestHit;
            await mifeng.GenerateProxyItems();

            ProxyGenerators.Add(xiaohuan);
            ProxyGenerators.Add(kuai);
            //ProxyGenerators.Add(mipu);
            ProxyGenerators.Add(mifeng);

#if DEBUG
            foreach (var item in ProxyGenerators)
            {
                await item.TestAll();
            }
#endif
        }

        public static IWebProxy GeneralProxy()
        {
            if (ProxyGenerators.Count == 0) Init().GetAwaiter().GetResult();

            var generator = ProxyGenerators.First(x => x.Available);

            var proxy = generator.GenerateProxy().GetAwaiter().GetResult();
            return proxy;
        }

        public static async Task<IWebProxy> GeneralProxyAsync()
        {
            if (ProxyGenerators.Count == 0) await Init();

            var generator = ProxyGenerators.First(x => x.Available);

            var proxy = await generator.GenerateProxy();
            return proxy;
        }

        private static void OnGenerateTestHit(object? sender, EventArgs.GenerateTestEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[TEST] {e.Source} {e.Uri} {e.Message}");
        }

        private static void OnGenerateFailed(object? sender, EventArgs.GenerateFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[GENT] {e.Source} 获取代理失败{e.Message}");
        }

        private static void OnGenerateSucceed(object? sender, EventArgs.GenerateSucceedEventArgs e)
        {
            if (!e.IsDefault)
                System.Diagnostics.Debug.WriteLine($"[GENT] {e.Source} 获取代理成功 {e.ProxyItem.Uri} {e.ProxyItem.Location}");
            else
                System.Diagnostics.Debug.WriteLine($"[GENT] {e.Source} 没有从代理池中获取到任何可用代理");
        }

        private static void OnGenerateLoadFailed(object? sender, EventArgs.GenerateLoadFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[LOAD] {e.Source} 加载代理列表失败 {e.Message}");
        }

        private static void OnGenerateLoadSucceed(object? sender, EventArgs.GenerateLoadSucceedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[LOAD] {e.Source} 加载代理列表成功 {e.ProxyItems.Count}条");
        }
    }
```

#### 使用方法

```csharp
 var proxy = await ProxyHelper.GeneralProxyAsync();
 System.Diagnostics.Debug.WriteLine((proxy as WebProxy)?.Address?.ToString());
```