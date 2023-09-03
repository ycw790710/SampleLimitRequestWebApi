using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

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
                int preShowLines = 0;
                while (_alive)
                {
                    try
                    {
                        var status = await apiInstance.ApiRequestRateLimitStatusGetStatusPostAsync();
                        List<string> allLines = new();
                        int consoleWidth = GetConsoleWidth();
                        if (status != null)
                        {
                            allLines.Add($"更新時間: {status.UpdatedTime.ToLocalTime()}  Q:結束");

                            int columnCount = status.ContainerTypeInfos.Count;

                            int columnWidth = (consoleWidth - columnCount - 1) / columnCount;

                            List<string>[] titleNameColumnsLines = new List<string>[columnCount];
                            for (int i = 0; i < titleNameColumnsLines.Length; i++)
                                titleNameColumnsLines[i] = new();
                            for (var c = 0; c < columnCount; c++)
                            {
                                var name = status.ContainerTypeInfos[c].Name;
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
                                var description = status.ContainerTypeInfos[c].Description;
                                var descriptionStrs = SplitByWidth(description, columnWidth);
                                foreach (var descriptionStr in descriptionStrs)
                                    titleDescriptionColumnsLines[c].Add(PadRight(descriptionStr, columnWidth));
                            }
                            allLines.AddRange(GetPrintingColumnLines(columnCount, columnWidth, titleDescriptionColumnsLines));

                            allLines.Add(new string('-', consoleWidth));

                            Dictionary<RequestRateLimitStatusPerTimeUnit, RequestRateLimitStatusPerTimeUnitInfo> perTimeUnit_infos = new();
                            foreach (var perUnitInfo in status.PerUnitInfos)
                            {
                                if (perUnitInfo.PerTimeUnit == null)
                                    continue;
                                perTimeUnit_infos.Add(perUnitInfo.PerTimeUnit.Value, perUnitInfo);
                            }
                            for (int r = 0; ; r++)
                            {
                                List<string>[] containerColumnsLines = new List<string>[columnCount];
                                for (int i = 0; i < containerColumnsLines.Length; i++)
                                    containerColumnsLines[i] = new();

                                for (int c = 0; c < columnCount; c++)
                                {
                                    var typeInfo = status.ContainerTypeInfos[c];
                                    if (typeInfo.Type == null)
                                        continue;
                                    var type = typeInfo.Type;

                                    var containers = status.ContainerTypesContainers[((int)type).ToString() ?? ""];
                                    if (r >= containers.Count)
                                        continue;
                                    var container = containers[r];
                                    var idStrs = SplitByWidth($"ID [{container.Key}]", columnWidth);
                                    foreach (var idStr in idStrs)
                                        containerColumnsLines[c].Add(PadRight(idStr, columnWidth));
                                    foreach (var item in container.Items)
                                    {
                                        string perTimeUnitName =
                                            item.PerTimeUnit.HasValue ? perTimeUnit_infos[item.PerTimeUnit.Value].Name : "";
                                        var itemInfoStrs = SplitByWidth($"{item.Capacity}/{item.LimitTimes} [{perTimeUnitName}]", columnWidth);
                                        foreach (var itemInfoStr in itemInfoStrs)
                                            containerColumnsLines[c].Add(PadLeft(itemInfoStr, columnWidth));
                                    }
                                }

                                if (containerColumnsLines.All(n => n.Count == 0))
                                    break;
                                allLines.AddRange(GetPrintingColumnLines(columnCount, columnWidth, containerColumnsLines));
                            }

                        }
                        Console.Clear();
                        for (int i = 0; i < allLines.Count; i++)
                            Console.WriteLine(allLines[i]);
                        for (int i = allLines.Count; i < preShowLines; i++)
                            Console.WriteLine(new string(' ', consoleWidth));
                        preShowLines = allLines.Count;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    SpinWait.SpinUntil(() => !_alive, 100);
                }
            });
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

    }
}