using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Tasks
{
    public class Task12 : AdventTask
    {
        /*
         * Had trouble with implementing combinations. But eventually figured it out.
         */
        public Task12()
        {
            Filename += "12.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input).Select(l => l.Trim().Split(" ")).ToList();
            long result = 0;
            foreach (var line in lines)
            {
                var springs = line.ElementAt(0);
                var nums = line.ElementAt(1);
                var numsSplitted = nums.Split(",").Select(int.Parse).ToList();
                var sumSprings = numsSplitted.Sum();

                result += GetAllCombinations(springs, numsSplitted.ToArray(), 0, sumSprings, new Dictionary<(string, int), long>());
            }
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            // Runs at around ~2s
            var lines = GetLinesList(input).Select(l => l.Trim().Split(" ")).ToList();
            long result = 0;
            foreach (var line in lines)
            {
                var springs = line.ElementAt(0);
                var nums = line.ElementAt(1);
                var numsSplitted = nums.Split(",").Select(int.Parse).ToList();

                var newSprings = springs;
                var newNums = numsSplitted.ToList();
                for (int i = 0; i < 4; i++)
                {
                    newSprings = springs + "?" + newSprings;
                    newNums.AddRange(numsSplitted);
                }
                var sumSprings = newNums.Sum();
                result += GetAllCombinations(newSprings, newNums.ToArray(), 0, sumSprings, new Dictionary<(string, int), long>());
            }
            Console.WriteLine(result);
        }

        public static string ReplaceFirst(string str, string term, string replace)
        {
            int position = str.IndexOf(term);
            if (position < 0)
            {
                return str;
            }
            str = str.Substring(0, position) + replace + str.Substring(position + term.Length);
            return str;
        }

        private long GetAllCombinations(string springs, int[] numsSplitted, int numIndex, int sumSprings, Dictionary<(string, int), long> buildStrings)
        {
            // If at the end of the string - have to covered all springs
            if (springs.Length == 0)
            {
                if (numIndex == numsSplitted.Length || sumSprings == 0) { return 1; }
                else { return 0; }
            }
            // If covered all springs, check if there are any '#' left.
            if (numIndex >= numsSplitted.Length) { 
                if (sumSprings == 0 && !springs.Any(s => s == '#')) { return 1; }
                return 0; 
            }
            // Memoization
            if (buildStrings.ContainsKey((springs, numIndex))) { return buildStrings[(springs, numIndex)]; }

            // Check if we can even cover all of the strings with the remaining numbers that are available
            var possibleSprings = springs.ToCharArray().Count(c => c == '#' || c == '?');
            var currentGroupSize = numsSplitted[numIndex];
            if (sumSprings > possibleSprings || sumSprings > possibleSprings + currentGroupSize) { return 0; }
            // If there are not enoug springs in the string - stop
            if (springs.Length < sumSprings) { return 0; }

            var currChar = springs[0];
            if (currChar == '.')
            {
                // Just skip '.' - its not interesting by itself
                buildStrings[(springs, numIndex)] = GetAllCombinations(springs.Remove(0, 1), numsSplitted, numIndex, sumSprings, buildStrings); 
                return buildStrings[(springs, numIndex)];
            } else if (currChar == '?')
            {
                // Try strings with replacing '.' and '#'. 
                buildStrings[(springs, numIndex)] = GetAllCombinations(ReplaceFirst(springs, "?", "#"), numsSplitted, numIndex, sumSprings, buildStrings) +
                    GetAllCombinations(ReplaceFirst(springs, "?", "."), numsSplitted, numIndex, sumSprings, buildStrings);
                return buildStrings[(springs, numIndex)];

            } else if (currChar == '#')
            {
                // If found a spring and there are no more nums available - return 0
                if (sumSprings <= 0) { return 0; }
            
                var currentGroup = springs.Take(currentGroupSize);
                // Check if current group contains '.' - this is unviable
                if (currentGroup.Any(c => c == '.')) { return 0; }

                var charsToRemove = currentGroupSize;
                if (charsToRemove < springs.Length) { 
                    // If next character after group is '#' - unviable. 
                    if (springs[charsToRemove] == '#') { return 0; }
                    charsToRemove++;
                }
                // Start new spring group, removing the current group (containing also the trailing '.')
                buildStrings[(springs, numIndex)] = GetAllCombinations(springs.Remove(0, charsToRemove), numsSplitted, numIndex + 1, sumSprings - currentGroupSize, buildStrings);
                return buildStrings[(springs, numIndex)];
            }
            return 0;
        }
    }
} 