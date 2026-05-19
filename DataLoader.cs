using Course_Project.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;

public sealed class DataLoader
{
    private const string BaseUrl = "https://info.toto.bg";
    private const string StatsPage = "/statistika/6x49";

    private static readonly Dictionary<int, string> FileUrls = new()
    {
        { 1958, "/content/files/stats-tiraji/649_58.txt" },
        { 1959, "/content/files/stats-tiraji/649_59.txt" },
        { 1960, "/content/files/stats-tiraji/649_60.txt" },
        { 1961, "/content/files/stats-tiraji/649_61.txt" },
        { 1962, "/content/files/stats-tiraji/649_62.txt" },
        { 1963, "/content/files/stats-tiraji/649_63.txt" },
        { 1964, "/content/files/stats-tiraji/649_64.txt" },
        { 1965, "/content/files/stats-tiraji/649_65.txt" },
        { 1966, "/content/files/stats-tiraji/649_66.txt" },
        { 1967, "/content/files/stats-tiraji/649_67.txt" },
        { 1968, "/content/files/stats-tiraji/649_68.txt" },
        { 1969, "/content/files/stats-tiraji/649_69.txt" },
        { 1970, "/content/files/stats-tiraji/649_70.txt" },
        { 1971, "/content/files/stats-tiraji/649_71.txt" },
        { 1972, "/content/files/stats-tiraji/649_72.txt" },
        { 1973, "/content/files/stats-tiraji/649_73.txt" },
        { 1974, "/content/files/stats-tiraji/649_74.txt" },
        { 1975, "/content/files/stats-tiraji/649_75.txt" },
        { 1976, "/content/files/stats-tiraji/649_76.txt" },
        { 1977, "/content/files/stats-tiraji/649_77.txt" },
        { 1978, "/content/files/stats-tiraji/649_78.txt" },
        { 1979, "/content/files/stats-tiraji/649_79.txt" },
        { 1980, "/content/files/stats-tiraji/649_80.txt" },
        { 1981, "/content/files/stats-tiraji/649_81.txt" },
        { 1982, "/content/files/stats-tiraji/649_82.txt" },
        { 1983, "/content/files/stats-tiraji/649_83.txt" },
        { 1984, "/content/files/stats-tiraji/649_84.txt" },
        { 1985, "/content/files/stats-tiraji/649_85.txt" },
        { 1986, "/content/files/stats-tiraji/649_86.txt" },
        { 1987, "/content/files/stats-tiraji/649_87.txt" },
        { 1988, "/content/files/stats-tiraji/649_88.txt" },
        { 1989, "/content/files/stats-tiraji/649_89.txt" },
        { 1990, "/content/files/stats-tiraji/649_90.txt" },
        { 1991, "/content/files/stats-tiraji/649_91.txt" },
        { 1992, "/content/files/stats-tiraji/649_92.txt" },
        { 1993, "/content/files/stats-tiraji/649_93.txt" },
        { 1994, "/content/files/stats-tiraji/649_94.txt" },
        { 1995, "/content/files/stats-tiraji/649_95.txt" },
        { 1996, "/content/files/stats-tiraji/649_96.txt" },
        { 1997, "/content/files/stats-tiraji/649_97.txt" },
        { 1998, "/content/files/stats-tiraji/649_98.txt" },
        { 1999, "/content/files/stats-tiraji/649_99.txt" },
        { 2000, "/content/files/stats-tiraji/649_00.txt" },
        { 2001, "/content/files/stats-tiraji/649_01.txt" },
        { 2002, "/content/files/stats-tiraji/649_02.txt" },
        { 2003, "/content/files/stats-tiraji/649_03.txt" },
        { 2004, "/content/files/stats-tiraji/649_04.txt" },
        { 2005, "/content/files/stats-tiraji/649_2005.txt" },
        { 2006, "/content/files/stats-tiraji/649_2006.txt" },
        { 2007, "/content/files/stats-tiraji/649_2007.txt" },
        { 2008, "/content/files/stats-tiraji/649_2008.txt" },
        { 2009, "/content/files/stats-tiraji/649_2009.txt" },
        { 2010, "/content/files/stats-tiraji/649_2010.txt" },
        { 2011, "/content/files/stats-tiraji/649_2011.txt" },
        { 2012, "/content/files/stats-tiraji/649_2012.txt" },
        { 2013, "/content/files/stats-tiraji/649_2013.txt" },
        { 2014, "/content/files/stats-tiraji/649_2014.txt" },
        { 2015, "/content/files/stats-tiraji/649_2015.txt" },
        { 2016, "/content/files/stats-tiraji/649_2016.txt" },
        { 2017, "/content/files/2018/01/26/2a0952991d371ca5575a4d79e5c5e5d5.txt" },
        { 2018, "/content/files/2019/02/16/be9d1b15257f53cd749db1e501b01180.txt" },
        { 2019, "/content/files/2020/01/04/149bdb98aa8426faf31b8b57fde4c5eb.txt" },
        { 2020, "/content/files/2021/01/09/8241c0de420163c1fcfd616689d1fa33.txt" },
        { 2021, "/content/files/2022/01/02/b72d0cbe449bcc17ec8ecb19ee82233a.docx" },
        { 2022, "/content/files/2023/01/11/5f8be78ee5e2ceb7839cefe22b7d2f1b.docx" },
        { 2023, "/content/files/2024/01/08/c6283cfbdeb917bb3ba894cc38b24728.docx" },
        { 2024, "/content/files/2025/01/06/ea7643fc1635991fe4548cf57b3cf994.docx" },
        { 2025, "/content/files/2026/01/07/5026e066d4883844db5c8ab602e38858.docx" },
    };

    // Verbose format: "Тираж   1/2018, Теглене 1: 5 10 11 35 40 41"
    private static readonly Regex VerboseRegex = new Regex(
        @"(?:Тираж|[^\x00-\x7F]+)\s+(\d+)/\d{4}[^:]*?1:\s*([\d ]+)",
        RegexOptions.Compiled);

    // Numeric-only fallback — works regardless of Cyrillic encoding
    private static readonly Regex VerboseFallbackRegex = new Regex(
        @"(\d+)/(\d{4})[^\n]*?1:\s*((?:\d+[ \t]+){5}\d+)",
        RegexOptions.Compiled);

    private readonly HttpClient _http;

    public DataLoader()
    {
        var handler = new HttpClientHandler { AllowAutoRedirect = true, UseCookies = true };
        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(30),
        };
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
        _http.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        _http.DefaultRequestHeaders.Add("Accept-Language", "bg,en;q=0.9");
        _http.DefaultRequestHeaders.Add("Accept-Encoding", "identity");
    }

    public async Task<IEnumerable<TotoDraw>> LoadDrawsAsync(int fromYear, int toYear)
    {
        var result = new List<TotoDraw>();

        for (int year = fromYear; year <= toYear; year++)
        {
            Console.Write($"  {year} : ");

            if (!FileUrls.TryGetValue(year, out var path))
            {
                Console.WriteLine("няма данни");
                continue;
            }

            bool isDocx = path.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);
            List<TotoDraw>? draws = isDocx
                ? await LoadDocxAsync(path, year)
                : await LoadTxtAsync(path, year);

            if (draws != null && draws.Count > 0)
                Console.WriteLine($"{draws.Count} тиража");
            else
                Console.WriteLine("няма данни");

            if (draws != null)
                result.AddRange(draws);
        }

        return result;
    }

    private async Task<List<TotoDraw>?> LoadTxtAsync(string path, int year)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.Headers.Add("Referer", BaseUrl + StatsPage);
            var response = await _http.SendAsync(req);

            if (!response.IsSuccessStatusCode) return null;

            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var draws = ParseTxtBytes(bytes, year);

            return draws;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async Task<List<TotoDraw>?> LoadDocxAsync(string path, int year)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, path);
            req.Headers.Add("Referer", BaseUrl + StatsPage);
            var response = await _http.SendAsync(req);

            if (!response.IsSuccessStatusCode) return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            return ParseDocx(stream, year);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    // ── TXT parser ───────────────────────────────────────────────────────────

    private List<TotoDraw> ParseTxtBytes(byte[] bytes, int year)
    {
        System.Text.Encoding.RegisterProvider(
            System.Text.CodePagesEncodingProvider.Instance);

        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return ParseText(System.Text.Encoding.UTF8.GetString(bytes), year);
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            return ParseText(System.Text.Encoding.Unicode.GetString(bytes), year);

        var encodings = new[]
        {
            System.Text.Encoding.GetEncoding("windows-1251"),
            System.Text.Encoding.UTF8,
        };

        foreach (var enc in encodings)
        {
            var text = enc.GetString(bytes);
            var result = ParseText(text, year);
            if (result.Count > 0)
                return result;
        }

        return new List<TotoDraw>();
    }

    private List<TotoDraw> ParseText(string content, int year)
    {
        // 1958-2016: compact CSV format  "drawNum,n1,n2,n3,n4,n5,n6"
        // 2017+    : verbose format      "Тираж N/YYYY, Теглене 1: n1 n2 ..."
        if (year <= 2017)
            return ParseCompact(content, year);

        return ParseVerbose(content, year);
    }

    private List<TotoDraw> ParseVerbose(string content, int year)
    {
        var draws = new List<TotoDraw>();

        foreach (Match m in VerboseRegex.Matches(content))
        {
            if (!int.TryParse(m.Groups[1].Value, out var drawNum)) continue;
            var nums = SplitNumbers(m.Groups[2].Value, ' ');
            if (nums.Count == 6)
                draws.Add(new TotoDraw { Year = year, DrawNumber = drawNum, Numbers = nums });
        }

        if (draws.Count > 0) return draws;

        foreach (Match m in VerboseFallbackRegex.Matches(content))
        {
            if (!int.TryParse(m.Groups[1].Value, out var drawNum)) continue;
            var nums = SplitNumbers(m.Groups[3].Value, ' ');
            if (nums.Count == 6)
                draws.Add(new TotoDraw { Year = year, DrawNumber = drawNum, Numbers = nums });
        }

        return draws;
    }

    private List<TotoDraw> ParseCompact(string content, int year)
    {
        // Format A (1958-2002): "drawNum,n1,n2,n3,n4,n5,n6"
        // Format B (2003-2011): "drawNum-n1,n2,n3,n4,n5,n6  n1,n2,..."  (НГ = draw 0)
        // Format C (2012):      "drawNum- n1, n2,n3,...  n1,n2,..."      (space after dash)
        // Format D (2013-2017): "drawNum   n1, n2, n3,...   n1, n2,..."  (num in own column)

        var draws = new List<TotoDraw>();

        foreach (var rawLine in content.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r').Trim();
            if (line.Length == 0) continue;

            // Split into columns on 2+ whitespace/tabs
            var cols = Regex.Split(line, @"\s{2,}|\t+")
                            .Select(c => c.Trim())
                            .Where(c => c.Length > 0)
                            .ToList();

            if (cols.Count == 0) continue;

            string first = cols[0];

            // Format A: first col is "drawNum,n1,n2,n3,n4,n5,n6"
            if (Regex.IsMatch(first, @"^\d+,\d"))
            {
                foreach (var col in cols)
                {
                    var parts = col.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 7) continue;
                    if (!int.TryParse(parts[0].Trim(), out var drawNum)) continue;
                    var nums = parts.Skip(1)
                        .Select(p => int.TryParse(p.Trim(), out var n) ? n : (int?)null)
                        .Where(n => n.HasValue && n.Value >= 1 && n.Value <= 49)
                        .Select(n => n!.Value).ToList();
                    if (nums.Count == 6)
                        draws.Add(new TotoDraw { Year = year, DrawNumber = drawNum, Numbers = nums });
                }
                continue;
            }

            // Format B/C: first col matches "drawNum-..." or "НГ-..."
            // Format B: "0-8,25,30" / Format C: "0- 3,10,23"
            var dashMatch = Regex.Match(first, @"^(\d+|НГ)-\s*(.+)$");
            if (dashMatch.Success)
            {
                string prefix = dashMatch.Groups[1].Value.Trim();
                int drawNum = prefix == "НГ" ? 0 : int.TryParse(prefix, out var p) ? p : -1;
                if (drawNum < 0) continue;

                var firstNums = ParseCsvNumbers(dashMatch.Groups[2].Value);
                if (firstNums.Count == 6)
                    draws.Add(new TotoDraw { Year = year, DrawNumber = drawNum, Numbers = firstNums });

                for (int i = 1; i < cols.Count; i++)
                {
                    var nums = ParseCsvNumbers(cols[i]);
                    if (nums.Count == 6)
                        draws.Add(new TotoDraw { Year = year, DrawNumber = drawNum + i, Numbers = nums });
                }
                continue;
            }

            // Format D (2013-2017): first col is just the draw number, rest are all numbers
            // "0   2,  9, 19, 20, 23, 30   5, 16, 39, 40, 44, 48"
            // Problem: extra spaces split the numbers across multiple cols
            // Solution: join everything after the draw number and split into groups of 6
            if (int.TryParse(first, out int baseDrawNum))
            {
                // Join all remaining columns into one string and parse all numbers
                var allNums = ParseCsvNumbers(string.Join(",", cols.Skip(1)));

                // Group into draws of 6
                for (int i = 0; i + 6 <= allNums.Count; i += 6)
                {
                    draws.Add(new TotoDraw
                    {
                        Year = year,
                        DrawNumber = baseDrawNum + (i / 6),
                        Numbers = allNums.GetRange(i, 6),
                    });
                }
            }
        }

        return draws;
    }

    private static List<int> ParseCsvNumbers(string raw) =>
        raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
           .Select(p => int.TryParse(p.Trim(), out var n) ? n : (int?)null)
           .Where(n => n.HasValue && n.Value >= 1 && n.Value <= 49)
           .Select(n => n!.Value)
           .ToList();


    private List<TotoDraw> ParseDocx(Stream stream, int year)
    {
        using var doc = WordprocessingDocument.Open(stream, false);
        var body = doc.MainDocumentPart!.Document.Body!;

        var text = string.Join("\n",
            body.Descendants<Paragraph>()
                .Select(p => p.InnerText.Trim())
                .Where(t => t.Length > 0));

        return ParseVerbose(text, year);
    }

    private static List<int> SplitNumbers(string raw, params char[] separators)
    {
        var seps = separators.Length > 0 ? separators : new[] { ' ', ',' };
        return raw.Split(seps, StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => int.TryParse(t.Trim(), out var n) ? n : (int?)null)
                  .Where(n => n.HasValue && n.Value >= 1 && n.Value <= 49)
                  .Select(n => n!.Value)
                  .Take(6)
                  .ToList();
    }

    private static string Trunc(string s, int max) =>
        s.Length <= max ? s : s[..max];
}