namespace AdventOfCode2023.Tasks
{
    public abstract class AdventTask
    {
        public string Filename = $"Inputs\\";

        public abstract void Solve1(string input);
        public abstract void Solve2(string input);

        public List<string> GetLinesList(string input)
        {
            return input.Split("\n").Select(l => l.Trim()).ToList();
        }

        public string[] GetLinesArray(string input)
        {
            return input.Split("\n").Select(l => l.Trim()).ToArray();
        }
    }
}
