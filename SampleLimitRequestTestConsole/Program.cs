using RequestRateLimit.Components;
using RequestRateLimit.Dtos;
using RequestRateLimit.Services;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SampleLimitRequestTestConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";
        static async Task Main(string[] args)
        {
            ThreadPool.SetMinThreads(130, 3);
            TestServicePerformance();

            Console.ReadLine();
        }
        private static void TestServicePerformance()
        {
            var requestRateLimitStatusCacheService = new RequestRateLimitStatusCacheService();
            var requestRateLimitCacheService = new RequestRateLimitCacheService(requestRateLimitStatusCacheService);
            var requestRateLimitService = new RequestRateLimitService(requestRateLimitCacheService);
            var requestRateLimitStatusService = new RequestRateLimitStatusService(
                requestRateLimitStatusCacheService, requestRateLimitCacheService);

            Task.Run(async () => {
                while (_alive)
                {
                    requestRateLimitStatusService.UpdateStatuses();
                    await Task.Delay(100);
                }
            });

            Stopwatch sw_share = new();
            sw_share.Start();
            for (int i = 0; i < 100; i++)
            {
                Task.Run(() => {
                    while (_alive)
                    {
                        var startTime = sw_share.ElapsedMilliseconds;
                        try
                        {
                            var controllerName = nameof(TestGlobalRequestRateLimit);
                            var actionName = nameof(TestGlobalRequestRateLimit.TestAction);
                            var typeInfo = typeof(TestGlobalRequestRateLimit).GetTypeInfo();
                            var methodInfo = typeInfo.GetMethod(actionName);
                            var host = Dns.GetHostEntry(Dns.GetHostName());
                            var ipAddress = host.AddressList[0];
                            requestRateLimitService.IsRequestOverLimit(
                                methodInfo, typeInfo, controllerName, actionName, ipAddress);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                        var endTime = sw_share.ElapsedMilliseconds;
                        var time = (int)Math.Max(0, 0 - endTime + startTime);

                        SpinWait.SpinUntil(() => !_alive, time);
                    }
                });
            }

            Task.Run(async () => {

                var statusInfoJsonBytes = requestRateLimitStatusService.GetStatusInfoJsonBytes();
                var statusInfoJson = Encoding.UTF8.GetString(statusInfoJsonBytes);
                var statusInfo = JsonSerializer.Deserialize<RequestRateLimitStatusInfo>(statusInfoJson);
                while (_alive)
                {
                    try
                    {
                        var json = requestRateLimitStatusService.GetStatusJson();
                        var obj = JsonSerializer.Deserialize<RequestRateLimitStatus>(json);
                        var containerList = obj?.containerTypesContainers?.FirstOrDefault();
                        if (containerList != null)
                        {
                            var container = containerList.Value.Value.FirstOrDefault();
                            var item = container?.items?.FirstOrDefault();
                            if (item != null)
                            {
                                var containerTypeInfo = 
                                    statusInfo.containerTypeInfos.First(n => n.type == container.type);

                                var key = container.key;
                                var perTimeUnitinfo = statusInfo.perUnitInfos[(int)item.perTimeUnit];
                                Console.Clear();
                                Console.WriteLine($"Service 基礎效能測試");
                                Console.WriteLine($"[{containerTypeInfo.name}] [{key}] {item.capacity}/{item.limitTimes} [{perTimeUnitinfo.name}]");
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    await Task.Delay(100);
                }
            });
        }

    }

    [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
    class TestGlobalRequestRateLimit
    {
        public void TestAction()
        {

        }
    }
}