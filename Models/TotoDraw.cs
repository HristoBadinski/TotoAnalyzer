namespace Course_Project.Models
{
    public sealed class TotoDraw
    {
        public int Year { get; init; }
        public int DrawNumber { get; init; }
        public IReadOnlyList<int> Numbers { get; init; } = Array.Empty<int>();
    }
}