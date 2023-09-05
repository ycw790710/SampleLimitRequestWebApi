using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using System.Diagnostics;

namespace SampleLimitRequestTestCountConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";
        static int globalCount = 0;

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(130, 130);

            Display();
            TestCount();
            // ctrl+F5:
            //  if too many timeout (5000 call one second, 10~30 seconds)
            //  , openapi-generator client Exception:
            // 'Org.OpenAPITools.Client.ApiException:
            //   Error calling ApiCountGetNormalGet: 一次只能用一個通訊端位址 (通訊協定/網路位址/連接埠)。 (localhost:7212)'
            // 或許不適合用來做壓力測試.

            while (Console.ReadKey().Key != ConsoleKey.Q)
                ;
        }

        private static void Display()
        {
            Task.Run(() => {
                while (_alive)
                {
                    Console.Clear();
                    Console.WriteLine(globalCount);
                    SpinWait.SpinUntil(() => !_alive, 100);
                }
            });
        }

        private static void TestCount()
        {

            Stopwatch sw_share = new();
            sw_share.Start();
            for (int i = 0; i < 50; i++)
                Task.Run(async () =>
                {
                    Configuration config = new();
                    config.BasePath = _basePath;
                    var countApi = new CountApi(config);
                    int fallCount = 0;
                    while (_alive)
                    {
                        var start = sw_share.ElapsedMilliseconds;
                        try
                        {
                            var count = await countApi.ApiCountGetNormalGetAsync();
                            Interlocked.Exchange(ref globalCount, count);
                        }
                        catch (Exception ex)
                        {
                            fallCount++;
                            if (fallCount > 10)
                            {
                                _alive = false;
                                Console.WriteLine(ex.ToString());
                                break;
                            }
                            config = new();
                            config.BasePath = _basePath;
                            countApi = new CountApi(config);
                        }
                        var end = sw_share.ElapsedMilliseconds;
                        var interval = 10;
                        var wait = (int)Math.Min(interval, Math.Max(0, interval - end + start));

                        SpinWait.SpinUntil(() => !_alive, wait);
                    }
                });
        }

    }
}