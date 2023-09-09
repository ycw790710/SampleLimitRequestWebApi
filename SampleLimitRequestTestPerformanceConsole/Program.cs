using CodeExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using SampleLimitRequestTestPerformanceConsole.SwaggerPaths;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SampleLimitRequestTestPerformanceConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";
        static string apiUrlBase = $"api/";
        static string _swaggerPath = "swagger/v1/swagger.json";
        static IReadOnlyList<string> _paths = null!;
        static ConcurrentQueue<TimeSpan> _requestExpiredTime = new();

        static IHost host = null!;

        static async Task Main(string[] args)
        {
            var baseThread = 200;
            var addThread = (int)Math.Min(40, baseThread * 1.4);
            var totalThread = baseThread + addThread;
            ThreadPool.SetMinThreads(totalThread, totalThread);
            Console.WriteLine("SetMinThreads Ready");

            host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();
                })
                .Build();

            await WaitForApiToBeReady();
            _paths = await GetPaths();
            Console.WriteLine("Paths Ready");

            DisplayBoard();
            CallPathsTest();

            while (Console.ReadKey().Key != ConsoleKey.Q)
                ;
        }

        static async Task WaitForApiToBeReady()
        {
            var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_basePath);
            Stopwatch sw = new();
            sw.Start();
            while (sw.Elapsed < new TimeSpan(0, 0, 30))
            {
                try
                {
                    var response = await httpClient.GetAsync("");
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        break;
                }
                catch (HttpRequestException ex)
                {
                    var statusCode = ex.StatusCode;
                    if (statusCode != null)
                    {
                        if (statusCode == System.Net.HttpStatusCode.NotFound)
                            break;
                        throw ex;
                    }
                }
                Console.WriteLine("Wait Web Api...");

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("Api Ready");
        }

        static async Task<IReadOnlyList<string>> GetPaths()
        {
            var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_basePath);
            var swaggerJson = await httpClient.GetStringAsync(_swaggerPath);
            var swaggerData = JsonSerializer.Deserialize<SwaggerData>(swaggerJson);
            List<string> paths = new();
            if (swaggerData != null && swaggerData.paths != null)
            {
                foreach (var kvp in swaggerData.paths)
                {
                    if (kvp.Value.get != null &&
                        (kvp.Value.get.parameters == null || kvp.Value.get.parameters.Count() == 0))
                    {
                        paths.Add(kvp.Key);
                    }
                }
            }
            return paths;
        }

        static void DisplayBoard()
        {
            Configuration config = new();
            config.BasePath = _basePath;
            var apiInstance = new RequestRateLimitStatusApi(config);
            var dataAction = async () =>
            {
                List<string> allLines = new();
                int consoleWidth = GetConsoleWidth();
                try
                {
                    var status = await apiInstance.ApiRequestRateLimitStatusGetStatusPostAsync();
                    if (status != null)
                    {
                        allLines.Add($"更新時間: {status.UpdatedTime.ToLocalTime()}  Q:結束");
                        allLines.Add("呼叫Paths效能測試");
                        var monitorContainerCount = status.TypesContainers.Sum(a =>
                        a.Value.Count);
                        var monitorItemCount = status.TypesContainers.Sum(a =>
                        a.Value.Sum(b =>
                        b.Items.Count));

                        allLines.Add($"監視Container總數: {monitorContainerCount}");
                        allLines.Add($"監視Container-Item總數: {monitorItemCount}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                return (allLines, consoleWidth);
            };
            var displayAction = async () =>
            {
                int preShowLines = 0;
                while (_alive)
                {
                    try
                    {
                        var data = await dataAction();
                        while (_requestExpiredTime.Count > 0 &&
                        _requestExpiredTime.TryPeek(out var requestInfo) &&
                        requestInfo < GlobalTimer.NowTimeSpan() &&
                        _requestExpiredTime.TryDequeue(out requestInfo))
                            ;
                        data.allLines.Add($"每秒Request總數: {_requestExpiredTime.Count}");

                        Console.Clear();
                        for (int i = 0; i < data.allLines.Count; i++)
                            Console.WriteLine(data.allLines[i]);
                        for (int i = data.allLines.Count; i < preShowLines; i++)
                            Console.WriteLine(new string(' ', data.consoleWidth));
                        preShowLines = data.allLines.Count;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    SpinWait.SpinUntil(() => !_alive, 100);
                }
            };
            Task.Run(displayAction);


        }

        private static void CallPathsTest()
        {
            Stopwatch sw_share = new();
            sw_share.Start();
            for (int taskCount = 0; taskCount < 10; taskCount++)
                Task.Run(async () =>
                {
                    Random rand = new();
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
                                var userId = RandomNumberGenerator.GetInt32(1000);
                                httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", GetToken(userId));
                                httpClients.Add(httpClient);
                            }

                            Task[] tasks = new Task[httpClients.Count];
                            for (var j = 0; j < tasks.Length; j++)
                            {
                                var httpClient = httpClients[j];
                                var pathIndex = RandomNumberGenerator.GetInt32(_paths.Count);
                                tasks[j] = httpClient.GetAsync(_paths[pathIndex]);
                                _requestExpiredTime.Enqueue(GlobalTimer.AddMillisecondsForNowTimeSpan(1000));
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

        private static int GetColumnWidth(int consoleWidth, int columnCount)
        {
            return (consoleWidth - columnCount - 1) / columnCount;
        }

        private static int GetConsoleWidth()
        {
            int consoleWidth = 180;
            if (OperatingSystem.IsWindows())
                consoleWidth = Console.WindowWidth;
            return consoleWidth;
        }

        private static List<string> GetPrintingColumnLines(int columnCount, int columnWidth, List<string>[] columnsLines)
        {
            List<string> printingColumnLines = new();
            for (int colRow = 0; ; colRow++)
            {
                List<string> lines = new();
                int countEmpty = 0;
                for (int c = 0; c < columnCount; c++)
                {
                    if (colRow >= columnsLines[c].Count)
                    {
                        lines.Add(PadRight("", columnWidth));
                        countEmpty++;
                    }
                    else
                        lines.Add(columnsLines[c][colRow]);
                }
                if (countEmpty == columnCount)
                    break;

                printingColumnLines.Add("|" + string.Join('|', lines) + "|");
            }
            return printingColumnLines;
        }

        private static IReadOnlyList<string> SplitByWidth(string text, int width)
        {
            List<string> list = new();
            int i = 0;
            while (i < text.Length)
            {
                StringBuilder sb = new();
                sb.Append(text[i]);
                var textWidth = GetCharWidth(text[i++]);
                while (i < text.Length && textWidth + GetCharWidth(text[i]) <= width)
                {
                    sb.Append(text[i]);
                    textWidth += GetCharWidth(text[i++]);
                }
                list.Add(sb.ToString());
            }
            return list;
        }

        private static string PadRight(string text, int width)
        {
            int textWidth = GetTextWidth(text);
            var offset = textWidth - text.Length;
            return text.PadRight(width - offset);
        }

        private static int GetTextWidth(string text)
        {
            return text.Sum(c => GetCharWidth(c));
        }
        private static int GetCharWidth(char c)
        {
            return c < 256 ? 1 : 2;
        }

        private static string PadLeft(string text, int width)
        {
            int textWidth = GetTextWidth(text);
            var offset = textWidth - text.Length;
            return text.PadLeft(width - offset);
        }



        private static async Task CallTest1()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"---{nameof(CallTest1)}---");
            Console.ResetColor();

            var dateTimePattern = "yyyy/MM/dd hh:mm:ss.fff tt";

            Stopwatch sw = new();

            try
            {
                Configuration config = new Configuration();
                config.BasePath = _basePath;
                config.DefaultHeaders.Add("Authorization", "Bearer " + GetToken(1));
                var apiInstance = new SampleApi(config);
                var data = "data_example";

                Console.WriteLine($"{nameof(apiInstance.ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetAsync)}");
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        await apiInstance.ApiSampleGetLimitGlobal5PreSecondUser3PreSecondGetAsync(data);
                        Console.WriteLine($"[{DateTime.Now.ToString(dateTimePattern)}] Ok");
                    }
                    catch
                    {
                        Console.WriteLine($"[{DateTime.Now.ToString(dateTimePattern)}] Fail");
                    }
                }
            }
            catch
            {
            }
        }

        static string GetTimeStr(int milliseconds)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
            return $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        static string GetToken(int userId)
        {
            var signingKey = new SymmetricSecurityKey(GetSecretKey());

            var claims = CreateClaims(userId);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "SampleLimitRequest",
                audience: "SampleLimitRequest",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        static byte[] GetSecretKey()
        {
            var bytes = Encoding.UTF8.GetBytes("a98dghmnibqutldimpga08hpm3h;ovihdg;029;vty;d0aest0oiassad9pnyvg39wyh08tyvaote");
            Array.Resize(ref bytes, 64);
            return bytes;
        }

        static IEnumerable<Claim> CreateClaims(int userId)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userId.ToString()),
        };
            return claims;
        }

    }
}