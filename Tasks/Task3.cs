using System.Text.RegularExpressions;

namespace AdventOfCode2023.Tasks
{
    public class Task3 : AdventTask
    {
        public Task3() 
        {
            Filename += "3.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesArray(input);
            var prevSymbols = new List<Match>();
            List<Match> currSymbols = null;
            int row = 0;
            var result = 0;
            var symbolRegex = @"((?!\.)(?!\d).)";
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                var regex = @"\d+";
                var matches = Regex.Matches(trimmed, regex).ToList();
                var doneMatches = new List<Match>();
                if (prevSymbols.Count() != 0 )
                {
                    // Check previous row
                    result = CheckDiagonal(result, prevSymbols, matches, doneMatches);
                }
                matches = matches.Where(m => !doneMatches.Contains(m)).ToList();
                if (currSymbols == null)
                {
                    currSymbols = Regex.Matches(trimmed, symbolRegex).ToList();
                }
                // Check current row                
                prevSymbols = currSymbols;
                foreach(Match match in matches)
                {
                    var value = int.Parse(match.Value);
                    if (currSymbols.Any(s => s.Index == match.Index - 1 || s.Index == match.Index + match.Length))
                    {
                        result += value;
                        doneMatches.Add(match);
                    }
                }
                matches = matches.Where(m => !doneMatches.Contains(m)).ToList();

                if (row + 1 < lines.Length)
                {
                    // Check next row
                    var nextRowTrimmed = lines[row + 1].Trim();
                    var foundSymbols = Regex.Matches(nextRowTrimmed, symbolRegex).ToList();
                    result = CheckDiagonal(result, foundSymbols, matches, doneMatches);
                    currSymbols = foundSymbols;
                }
                row++;
            }
            Console.WriteLine(result);
        }

        private int CheckDiagonal(int result, List<Match> symbols, List<Match> matches, List<Match> doneMatches) 
        {
            foreach (Match match in matches)
            {
                var value = int.Parse(match.Value);
                if (symbols.Any(s => s.Index >= match.Index - 1 && s.Index <= match.Index + match.Length))
                {
                    result += value;
                    doneMatches.Add(match);
                }
            }
            return result;
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesArray(input);
            var prevNumbers = new List<Match>();
            List<Match> currNumbers = null;
            int row = 0;
            var result = 0;
            var symbolRegex = @"((\*)+)";
            var numRegex = @"\d+";
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                var nextNumbers = new List<Match>();
                var prevAndNextNums = new List<Match>();
                var matches = Regex.Matches(trimmed, symbolRegex).ToList();  
                
                if (currNumbers == null)
                    currNumbers = Regex.Matches(trimmed, numRegex).ToList();
                
                if (prevNumbers.Count() != 0)
                    prevAndNextNums.AddRange(prevNumbers);      
                
                if (row + 1 < lines.Length)
                {
                    var nextRowTrimmed = lines[row + 1].Trim();
                    nextNumbers = Regex.Matches(nextRowTrimmed, numRegex).ToList();
                    prevAndNextNums.AddRange(nextNumbers);
                }

                foreach (Match match in matches)
                {
                    // Check current row
                    var foundNumbers = currNumbers.Where(s => match.Index == s.Index - 1 || match.Index == s.Index + s.Length).ToList();
                    // Check previous and next row
                    foundNumbers.AddRange(prevAndNextNums.Where(s => match.Index >= s.Index - 1 && match.Index <= s.Index + s.Length).ToList());
                    if (foundNumbers.Count == 2)
                        result += int.Parse(foundNumbers.FirstOrDefault().Value) * int.Parse(foundNumbers.LastOrDefault().Value);
                }
                prevNumbers = currNumbers;
                currNumbers = nextNumbers;
                
                row++;
            }
            Console.WriteLine(result);
        }
    }
}
