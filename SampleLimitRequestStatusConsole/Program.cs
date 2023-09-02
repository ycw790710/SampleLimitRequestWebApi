using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using System.Diagnostics;

namespace SampleLimitRequestStatusConsole
{
    internal class Program
    {
        static bool _alive = true;
        static string _basePath = "https://localhost:7212";

        static void Main(string[] args)
        {
            DisplayBoard();

            while (Console.ReadKey().Key != ConsoleKey.Q)
                ;
        }

        static void DisplayBoard()
        {
            Configuration config = new();
            config.BasePath = _basePath;
            var apiInstance = new RequestRateLimitStatusApi(config);

            Task.Run(async () =>
            {
                while (_alive)
                {
                    try
                    {
                        var status = await apiInstance.ApiRequestRateLimitStatusGetStatusPostAsync();
                        if (status != null)
                        {
                            Console.Clear();
                            Console.WriteLine($"更新時間: {status.UpdatedTime.ToLocalTime()}  Q:結束");
                            int consoleWidth = Console.WindowWidth - 2;

                            int columnWidth = (consoleWidth - status.ContainerTypeInfos.Count - 1) / status.ContainerTypeInfos.Count;
                            Console.WriteLine("|" + string.Join('|',
                                 status.ContainerTypeInfos.Select(n => PadLeft(n.Name, columnWidth))
                                 ) + "|"
                                );
                            Console.WriteLine("|" + string.Join('|',
                                status.ContainerTypeInfos.Select(n => PadLeft(n.Description, columnWidth))
                                ) + "|"
                               );
                            Console.WriteLine(new string('-', consoleWidth));

                            Dictionary<RequestRateLimitStatusPerTimeUnit, RequestRateLimitStatusPerTimeUnitInfo> perTimeUnit_infos = new();
                            foreach (var perUnitInfo in status.PerUnitInfos)
                            {
                                if (perUnitInfo.PerTimeUnit == null)
                                    continue;
                                perTimeUnit_infos.Add(perUnitInfo.PerTimeUnit.Value, perUnitInfo);
                            }
                            for (int r = 0; ; r++)
                            {
                                List<string>[] columnsLines = new List<string>[status.ContainerTypeInfos.Count];
                                for (int i = 0; i < columnsLines.Length; i++)
                                    columnsLines[i] = new();

                                for (int c = 0; c < status.ContainerTypeInfos.Count; c++)
                                {
                                    var typeInfo = status.ContainerTypeInfos[c];
                                    if (typeInfo.Type == null)
                                        continue;
                                    var type = typeInfo.Type;

                                    var containers = status.ContainerTypesContainers[((int)type).ToString() ?? ""];
                                    if (r >= containers.Count)
                                        continue;
                                    var container = containers[r];
                                    columnsLines[c].Add(PadRight($"ID [{container.Key}]", columnWidth));
                                    foreach (var item in container.Items)
                                    {
                                        string perTimeUnitName =
                                            item.PerTimeUnit.HasValue ? perTimeUnit_infos[item.PerTimeUnit.Value].Name : "";
                                        columnsLines[c].Add(
                                            PadLeft($"{item.Capacity}/{item.LimitTimes} [{perTimeUnitName}]", columnWidth));
                                    }
                                }

                                if (columnsLines.All(n => n.Count == 0))
                                    break;

                                for (int colRow = 0; ; colRow++)
                                {
                                    List<string> lines = new();
                                    int countEmpty = 0;
                                    for (int c = 0; c < status.ContainerTypeInfos.Count; c++)
                                    {
                                        if (colRow >= columnsLines[c].Count)
                                        {
                                            lines.Add(PadRight("", columnWidth));
                                            countEmpty++;
                                        }
                                        else
                                            lines.Add(columnsLines[c][colRow]);
                                    }
                                    if (countEmpty == status.ContainerTypeInfos.Count)
                                        break;

                                    Console.WriteLine("|" + string.Join('|',
                                        lines
                                        ) + "|"
                                       );
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    SpinWait.SpinUntil(() => !_alive, 200);
                }
            });
        }

        private static string PadRight(string text, int width)
        {
            var count = text.Sum(c => c < 256 ? 1 : 2);
            var offset = count - text.Length;
            return text.PadRight(width - offset);
        }

        private static string PadLeft(string text, int width)
        {
            var count = text.Sum(c => c < 256 ? 1 : 2);
            var offset = count - text.Length;
            return text.PadLeft(width - offset);
        }

    }
}