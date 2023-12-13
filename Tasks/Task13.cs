using System.Data;

namespace AdventOfCode2023.Tasks
{
    public class Task13 : AdventTask
    {
        public Task13()
        {
            Filename += "13.txt";
        }

        public override void Solve1(string input)
        {
            Console.WriteLine(SolveBoth(input, true));
        }

        public override void Solve2(string input)
        {
            Console.WriteLine(SolveBoth(input, false));
        }

        private long SolveBoth(string input, bool part1)
        {
            var patterns = input.Split("\r\n\r\n");
            long result = 0;
            foreach (var pattern in patterns)
            {
                var lines = GetLinesList(pattern).Select(l => l.Trim()).ToList();
                var transposedLines = new List<string>();
                for (int i = 0; i < lines.ElementAt(0).Length; i++)
                {
                    var newString = "";
                    foreach (var line in lines) { newString += line[i]; }
                    transposedLines.Add(newString);
                }
                result += CheckForReflection(transposedLines, part1);
                result += 100 * CheckForReflection(lines, part1);
            }
            return result;
        }

        private int CheckForReflection(List<string> pattern, bool part1)
        {
            var acceptedSmudges = part1 ? 0 : 1;
            for (int i = 1; i < pattern.Count; i++) 
            {
                var iterLeft = i - 1;
                var iterRight = i;
                var smudges = 0;
                while (iterLeft >= 0 && iterRight < pattern.Count)
                {
                    var left = pattern.ElementAt(iterLeft);
                    var right = pattern.ElementAt(iterRight);

                    smudges += left.Zip(right).Where(chars => chars.First != chars.Second).Count(); 

                    if (smudges > acceptedSmudges) { break; }
                    iterLeft--; iterRight++;
                }
                
                if (smudges == acceptedSmudges) { return i; }
            }
            return 0;
        }
    }
}