using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Net.Http.Json;

namespace SampleLimitRequestTestCountConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";
        static string apiUrl = $"api/Count/Get_Normal";
        static IHost host = null!;

        static void Main(string[] args)
        {
            var baseThread = 200;
            var addThread = (int)Math.Min(40, baseThread * 1.4);
            var totalThread = baseThread + addThread;
            ThreadPool.SetMinThreads(totalThread, totalThread);
            host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();
                })
                .Build();

            Display();
            TestCount();

            while (Console.ReadKey().Key != ConsoleKey.Q)
                ;
        }

        private static void Display()
        {
            Task.Run(async() => {
                var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
                while (_alive)
                {
                    try
                    {
                        var httpClient = httpClientFactory.CreateClient();
                        httpClient.BaseAddress = new Uri(_basePath);

                        var response = await httpClient.GetAsync(apiUrl);
                        var count = await response.Content.ReadFromJsonAsync<int>();
                        httpClient.Dispose();

                        Console.Clear();
                        Console.WriteLine("Web Api 基礎效能測試");
                        Console.WriteLine(DateTime.Now.ToString("更新時間 yyyy/MM/dd HH:mm:ss.fff"));
                        Console.WriteLine(count +"/ per sec");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    SpinWait.SpinUntil(() => !_alive, 100);
                }
            });
        }

        private static void TestCount()
        {

            Stopwatch sw_share = new();
            sw_share.Start();
            for (int taskCount = 0; taskCount < 10; taskCount++)
                Task.Run(async () =>
                {
                    while (_alive)
                    {
                        var start = sw_share.ElapsedMilliseconds;
                        try
                        {
                            var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
                            List<HttpClient> httpClients = new();
                            for (int clientCount = 0; clientCount < 20; clientCount++)
                            {
                                var httpClient = httpClientFactory.CreateClient();
                                httpClient.BaseAddress = new Uri(_basePath);
                                httpClients.Add(httpClient);
                            }

                            Task[] tasks = new Task[httpClients.Count];
                            for (var j = 0; j < tasks.Length; j++)
                            {
                                var httpClient = httpClients[j];
                                tasks[j] = httpClient.GetAsync(apiUrl);
                            }

                            await Task.WhenAll(tasks);

                            foreach (var httpClient in httpClients)
                                httpClient.Dispose();
                        }
                        catch (Exception ex)
                        {
                        }
                        var end = sw_share.ElapsedMilliseconds;
                        var interval = 20;
                        var wait = (int)Math.Max(0, interval - end + start);

                        SpinWait.SpinUntil(() => !_alive, wait);
                    }
                });
        }

    }
}