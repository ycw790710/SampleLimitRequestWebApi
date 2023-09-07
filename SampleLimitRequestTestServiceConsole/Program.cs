using RequestRateLimit.Components;
using RequestRateLimit.Dtos;
using RequestRateLimit.Services;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SampleLimitRequestTestServiceConsole
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
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList[0];

            var assembly = Assembly.GetAssembly(typeof(Program));
            var controllerNameBase = "TestGlobalRequestRateLimit";
            var actionNameBase = "TestAction";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 1; j <= 1; j++)
                {
                    var a = j;
                    var controllerName = controllerNameBase + a.ToString();
                    for (int k = 1; k <= 10; k++)
                    {
                        var b = k;
                        var actionName = actionNameBase + b.ToString();
                        Task.Run(() =>
                        {
                            var typeInfo = assembly?.GetTypes()
                                .FirstOrDefault(n => n.Name == controllerName)?.GetTypeInfo();
                            if (typeInfo == null)
                                return;
                            var methodInfo = typeInfo.GetMethod(actionName);
                            if (methodInfo == null)
                                return;
                            LoopCallApiMethod(requestRateLimitService, sw_share, ipAddress,
                                controllerName, actionName, typeInfo, methodInfo);
                        });
                    }
                }
            }

            Task.Run(() => {

                var statusInfoJsonBytes = requestRateLimitStatusService.GetStatusInfoJsonBytes();
                var statusInfoJson = Encoding.UTF8.GetString(statusInfoJsonBytes);
                var statusInfo = JsonSerializer.Deserialize<RequestRateLimitStatusInfo>(statusInfoJson);

                if (statusInfo == null)
                {
                    Console.WriteLine("Empty statusInfo");
                    return;
                }

                while (_alive)
                {
                    try
                    {
                        List<string> alllines = new();

                        var json = requestRateLimitStatusService.GetStatusJson();
                        if (json == null)
                            return;
                        var obj = JsonSerializer.Deserialize<RequestRateLimitStatus>(json);
                        if (obj == null)
                            return;

                        foreach (var containerList in obj.typesContainers)
                        {
                            var containerTypeInfo =
                                statusInfo.typeInfos.First(n => (int)n.type == containerList.Key);
                            alllines.Add($"[{containerTypeInfo.name}]");
                            foreach (var container in containerList.Value)
                            {
                                if (container == null || container.items == null)
                                    continue;

                                var key = container.key;
                                alllines.Add($"\t[{key}]");
                                foreach (var item in container.items)
                                {
                                    if (item == null)
                                        continue;

                                    var perTimeUnitinfo = statusInfo.unitUnitInfos[(int)item.unit];
                                    alllines.Add($"\t\t{item.amount}/{item.limit} [{perTimeUnitinfo.name}]");
                                }

                            }
                        }

                        Console.Clear();
                        Console.WriteLine($"Service 基礎效能測試");
                        foreach (var line in alllines)
                            Console.WriteLine(line);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    SpinWait.SpinUntil(() => !_alive, 100);                    
                }
            });
        }

        static void LoopCallApiMethod(RequestRateLimitService requestRateLimitService,
            Stopwatch sw_share,
            IPAddress? ipAddress,
            string controllerName,
            string actionName, TypeInfo typeInfo, MethodInfo methodInfo)
        {
            while (_alive)
            {
                var startTime = sw_share.ElapsedMilliseconds;
                try
                {
                    requestRateLimitService.IsRequestOverLimit("GET",
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
        }

    }

    [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
    class TestGlobalRequestRateLimit
    {
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction1() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction2() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction3() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction4() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction5() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction6() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction7() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction8() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction9() { }
        [GlobalRequestRateLimit(5000, RequestRateLimitPerTimeUnit.Seconds)]
        public void TestAction10() { }
    }

    class TestGlobalRequestRateLimit1 : TestGlobalRequestRateLimit { }
    class TestGlobalRequestRateLimit2 : TestGlobalRequestRateLimit { }
}