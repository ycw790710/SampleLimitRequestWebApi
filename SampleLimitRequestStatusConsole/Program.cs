using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SampleLimitRequestStatusConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";
        static string apiUrl = $"api/RequestRateLimitStatus/GetStatus";
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

            DisplayBoard();
            CallStatusTest();

            while (Console.ReadKey().Key != ConsoleKey.Q)
                ;
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
                int columnWidth = 0;
                try
                {
                    var status = await apiInstance.ApiRequestRateLimitStatusGetStatusPostAsync();
                    var statusInfo = await apiInstance.ApiRequestRateLimitStatusGetStatusInfoPostAsync();
                    if (status != null)
                    {
                        allLines.Add($"更新時間: {status.UpdatedTime.ToLocalTime()}  Q:結束");
                        allLines.Add("Status狀態看板 + 呼叫Status效能測試");

                        int columnCount = statusInfo.TypeInfos.Count;
                        columnWidth = GetColumnWidth(consoleWidth, columnCount);

                        List<string>[] titleNameColumnsLines = new List<string>[columnCount];
                        for (int i = 0; i < titleNameColumnsLines.Length; i++)
                            titleNameColumnsLines[i] = new();
                        for (var c = 0; c < columnCount; c++)
                        {
                            var name = statusInfo.TypeInfos[c].Name;
                            var nameStrs = SplitByWidth(name, columnWidth);
                            foreach (var nameStr in nameStrs)
                                titleNameColumnsLines[c].Add(PadRight(nameStr, columnWidth));
                        }
                        allLines.AddRange(GetPrintingColumnLines(columnCount, columnWidth, titleNameColumnsLines));

                        List<string>[] titleDescriptionColumnsLines = new List<string>[columnCount];
                        for (int i = 0; i < titleDescriptionColumnsLines.Length; i++)
                            titleDescriptionColumnsLines[i] = new();
                        for (var c = 0; c < columnCount; c++)
                        {
                            var description = statusInfo.TypeInfos[c].Description;
                            var descriptionStrs = SplitByWidth(description, columnWidth);
                            foreach (var descriptionStr in descriptionStrs)
                                titleDescriptionColumnsLines[c].Add(PadRight(descriptionStr, columnWidth));
                        }
                        allLines.AddRange(GetPrintingColumnLines(columnCount, columnWidth, titleDescriptionColumnsLines));

                        allLines.Add(new string('-', consoleWidth));

                        for (int r = 0; ; r++)
                        {
                            List<string>[] containerColumnsLines = new List<string>[columnCount];
                            for (int i = 0; i < containerColumnsLines.Length; i++)
                                containerColumnsLines[i] = new();

                            for (int c = 0; c < columnCount; c++)
                            {
                                var typeInfo = statusInfo.TypeInfos[c];
                                if (typeInfo.Type == null)
                                    continue;
                                var type = typeInfo.Type;

                                var containers = status.TypesContainers[((int)type).ToString() ?? ""];
                                if (r >= containers.Count)
                                    continue;
                                var container = containers[r];
                                var idStrs = SplitByWidth($"ID [{container.Key}]", columnWidth);
                                foreach (var idStr in idStrs)
                                    containerColumnsLines[c].Add(PadRight(idStr, columnWidth));
                                foreach (var item in container.Items)
                                {
                                    string perTimeUnitName =
                                        item.Unit.HasValue ? statusInfo.UnitUnitInfos[((int)item.Unit.Value).ToString()].Name : "";
                                    var itemInfoStrs = SplitByWidth($"{item.Amount}/{item.Limit} [{perTimeUnitName}]", columnWidth);
                                    foreach (var itemInfoStr in itemInfoStrs)
                                        containerColumnsLines[c].Add(PadLeft(itemInfoStr, columnWidth));
                                }
                            }

                            if (containerColumnsLines.All(n => n.Count == 0))
                                break;
                            allLines.AddRange(GetPrintingColumnLines(columnCount, columnWidth, containerColumnsLines));
                        }

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

        private static void CallStatusTest()
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
                                tasks[j] = httpClient.PostAsync(apiUrl, null);
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