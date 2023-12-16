using System.Data;

namespace AdventOfCode2023.Tasks
{
    public class Task13_WithNums : AdventTask
    {
        /*
            More "elegant" solution based on bitwise operations.
            Encode the lines as numbers and do bitwise operations on them. 
            I think I like the first solution more. Both have the same speed,
            however the first is easier to debug and to understand. 
        */
        public Task13_WithNums()
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
                var normalNumbers = GetNumbers(lines);
                var transposedNumbers = GetNumbers(transposedLines);
                result += CheckForReflection(transposedNumbers, part1);
                result += 100 * CheckForReflection(normalNumbers, part1);
            }
            return result;
        }

        private int CheckForReflection(List<long> pattern, bool part1)
        {
            var acceptedSmudges = part1 ? 0 : 1;
            for (int i = 1; i < pattern.Count; i++)
            {
                var iterLeft = i - 1;
                var iterRight = i;
                long smudges = 0;
                while (iterLeft >= 0 && iterRight < pattern.Count)
                {
                    var left = pattern.ElementAt(iterLeft);
                    var right = pattern.ElementAt(iterRight);

                    var xor = left ^ right;
                    smudges += CountBits(xor);

                    if (smudges > acceptedSmudges) { break; }
                    iterLeft--; iterRight++;
                }

                if (smudges == acceptedSmudges) { return i; }
            }
            return 0;
        }

        public static int CountBits(long value)
        {
            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }
            return count;
        }

        private List<long> GetNumbers(List<string> lines)
        {
            var numbers = new List<long>();
            foreach (var line in lines)
            {
                var pow = 0;
                long newNum = 0;
                foreach (var c in line)
                {
                    if (c == '#') { newNum += (long)Math.Pow(2, pow); }
                    pow++;
                }
                numbers.Add(newNum);
            }
            return numbers;
        }
    }
}