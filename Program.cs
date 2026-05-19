using Course_Project.Models;

internal class Program
{
    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var loader = new DataLoader();
        IEnumerable<TotoDraw>? draws = null;
        int fromYear = 0, toYear = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("============================================");
            Console.WriteLine("              ТОТО АНАЛИЗАТОР              ");
            Console.WriteLine("============================================");
            Console.WriteLine(" [1]  Избери период (от година – до година)");
            Console.WriteLine(" [2]  Топ N най-чести числа");
            Console.WriteLine(" [3]  Горещи двойки");
            Console.WriteLine(" [4]  Разпределение по десетици");
            Console.WriteLine(" [5]  Топлинна карта (Heat Map)");
            Console.WriteLine(" [0]  Изход");
            Console.WriteLine("============================================");

            if (draws != null)
                Console.WriteLine($" Заредени тиражи: {draws.Count()}  ({fromYear}–{toYear})");

            Console.Write(" Избор: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                ShowError("Невалиден избор. Въведи число.");
                continue;
            }

            if (choice == 0) break;

            // ── Option 1: load period ────────────────────────────────────────
            if (choice == 1)
            {
                fromYear = ReadInt(" От година: ");
                toYear = ReadInt(" До година: ");

                if (fromYear <= 0 || toYear <= 0 || fromYear > toYear)
                {
                    ShowError("Невалиден период.");
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("Зареждане...");
                draws = await loader.LoadDrawsAsync(fromYear, toYear);
                int cnt = draws.Count();
                Console.WriteLine();
                Console.WriteLine($"Заредени {cnt} тиража за периода {fromYear}–{toYear}.");
                Pause();
                continue;
            }

            // ── Options 2–5 require data ─────────────────────────────────────
            if (draws == null)
            {
                ShowError("Първо избери период (опция 1)!");
                continue;
            }

            var stats = new Statistics(draws);

            switch (choice)
            {
                case 2:
                    {
                        int n = ReadInt(" Въведи N (брой числа): ");
                        if (n <= 0) { ShowError("N трябва да е положително число."); break; }
                        var top = stats.GetTopNumbers(n);
                        Console.Clear();
                        Console.WriteLine($"  Топ {n} най-чести числа ({fromYear}–{toYear}):");
                        Visualizer.ShowTopNumbersBarChart(top,
                            $"  Топ {n} числа ({fromYear}–{toYear})");
                        Pause();
                        break;
                    }

                case 3:
                    {
                        int n = ReadInt(" Въведи N (брой двойки): ");
                        if (n <= 0) { ShowError("N трябва да е положително число."); break; }
                        var pairs = stats.GetHotPairs(n).ToList();
                        Console.Clear();
                        Console.WriteLine($"  Топ {n} горещи двойки ({fromYear}–{toYear}):");
                        Console.WriteLine();
                        Console.WriteLine($"  {"Двойка",-10} {"Съвместни изтегляния",22}");
                        Console.WriteLine("  " + new string('─', 34));
                        foreach (var (a, b, count) in pairs)
                            Console.WriteLine($"  {a,2} – {b,2}       {count,10}");
                        Console.WriteLine();
                        Pause();
                        break;
                    }

                case 4:
                    {
                        var dist = stats.GetDecadeDistribution();
                        Console.Clear();
                        Console.WriteLine($"  Разпределение по десетици ({fromYear}–{toYear}):");
                        Visualizer.ShowTopNumbersBarChart(
                            dist.ToDictionary(kv => int.Parse(kv.Key.Split('-')[0]), kv => kv.Value),
                            $"  Десетици ({fromYear}–{toYear})");

                        Console.WriteLine("  Точни стойности:");
                        foreach (var kv in dist)
                            Console.WriteLine($"    {kv.Key,-6}: {kv.Value}");
                        Console.WriteLine();
                        Pause();
                        break;
                    }

                case 5:
                    {
                        var freq = stats.GetFrequencyMap();
                        Console.Clear();
                        Console.WriteLine($"  Топлинна карта ({fromYear}–{toYear}):");
                        Visualizer.ShowHeatMap(freq);
                        Pause();
                        break;
                    }

                default:
                    ShowError("Невалиден избор.");
                    break;
            }
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        int.TryParse(Console.ReadLine(), out int val);
        return val;
    }

    private static void ShowError(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($" ✖ {msg}");
        Console.ResetColor();
        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine(" Натисни произволен клавиш...");
        Console.ReadKey(intercept: true);
    }
}