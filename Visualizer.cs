public static class Visualizer
{
    private const int BarWidth = 30;

    // ── Horizontal bar chart ─────────────────────────────────────────────────

    public static void ShowTopNumbersBarChart(Dictionary<int, int> top, string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);
        Console.WriteLine(new string('─', 45));

        if (top.Count == 0)
        {
            Console.WriteLine("  (няма данни)");
            return;
        }

        int max = top.Values.Max();

        foreach (var kv in top.OrderByDescending(x => x.Value))
        {
            int barLen = max > 0 ? (int)((double)kv.Value / max * BarWidth) : 0;
            Console.WriteLine($"  {kv.Key,2} | {new string('█', barLen),-30} {kv.Value}");
        }

        Console.WriteLine();
    }

    // ── 7×7 heat map ─────────────────────────────────────────────────────────

    public static void ShowHeatMap(Dictionary<int, int> freq)
    {
        Console.WriteLine();
        Console.WriteLine("  Топлинна карта (1–49)");
        Console.WriteLine("  ░░ Студено (долни 30%)   ▒▒ Средно   ██ Горещо (горни 30%)");
        Console.WriteLine();

        var sorted = freq.OrderBy(x => x.Key).ToList();
        var counts = sorted.Select(x => x.Value).OrderBy(x => x).ToList();

        int lowThreshold = counts[(int)(counts.Count * 0.30)];
        int highThreshold = counts[(int)(counts.Count * 0.70)];

        for (int i = 0; i < 49; i++)
        {
            int num = sorted[i].Key;
            int count = sorted[i].Value;

            if (count >= highThreshold)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($" {num,2}");
            }
            else if (count <= lowThreshold)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" {num,2}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" {num,2}");
            }

            Console.ResetColor();

            if ((i + 1) % 7 == 0)
                Console.WriteLine();
        }

        Console.WriteLine();

        // Legend line with counts
        Console.WriteLine("  Брой изтегляния:");
        for (int i = 0; i < 49; i++)
        {
            int count = sorted[i].Value;

            if (count >= highThreshold) Console.ForegroundColor = ConsoleColor.Red;
            else if (count <= lowThreshold) Console.ForegroundColor = ConsoleColor.Cyan;
            else Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write($"{count,5}");
            Console.ResetColor();

            if ((i + 1) % 7 == 0)
                Console.WriteLine();
        }

        Console.WriteLine();
    }
}