using Course_Project.Models;

public sealed class Statistics
{
    private readonly IReadOnlyList<TotoDraw> _draws;

    public Statistics(IEnumerable<TotoDraw> draws)
    {
        _draws = draws.ToList();
    }

    /// Top N most-drawn numbers. Returns Dictionary<number, count> sorted descending.
    public Dictionary<int, int> GetTopNumbers(int n) =>
        _draws
            .SelectMany(d => d.Numbers)
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .Take(n)
            .ToDictionary(g => g.Key, g => g.Count());

    /// Top N pairs that appear together most often. Returns (num1, num2, count).
    public IEnumerable<(int a, int b, int count)> GetHotPairs(int n) =>
        _draws
            .SelectMany(d =>
            {
                var sorted = d.Numbers.OrderBy(x => x).ToList();
                return sorted.SelectMany((x, i) =>
                    sorted.Skip(i + 1).Select(y => (x, y)));
            })
            .GroupBy(p => p)
            .Select(g => (g.Key.x, g.Key.y, g.Count()))
            .OrderByDescending(t => t.Item3)
            .Take(n);

    /// Count of all drawn numbers grouped into the five decade ranges.
    public Dictionary<string, int> GetDecadeDistribution()
    {
        var ranges = new (int from, int to, string label)[]
        {
            (1,  10, "1-10"),
            (11, 20, "11-20"),
            (21, 30, "21-30"),
            (31, 40, "31-40"),
            (41, 49, "41-49"),
        };

        var all = _draws.SelectMany(d => d.Numbers);

        return ranges.ToDictionary(
            r => r.label,
            r => all.Count(x => x >= r.from && x <= r.to));
    }

    /// How many times each number 1–49 has been drawn.
    public Dictionary<int, int> GetFrequencyMap() =>
        Enumerable.Range(1, 49)
            .ToDictionary(
                n => n,
                n => _draws.SelectMany(d => d.Numbers).Count(x => x == n));
}