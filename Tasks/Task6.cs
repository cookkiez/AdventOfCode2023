using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Tasks
{
    public class Task6 : AdventTask
    {
        public Task6()
        {
            Filename += "6.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            long result = 1;
            var times = GetSplittedLine(lines, 0);
            var distances = GetSplittedLine(lines, 1);
            for (int i = 0; i < times.Count(); i++)
            {
                var time = int.Parse(times.ElementAt(i).Trim());
                var dist = int.Parse(distances.ElementAt(i).Trim());
                var winningSituations = 0;
                var isEven = time % 2 == 0;
                var timeToAdd = isEven ? 0 : 1;
                for (long speed = 1; speed < time / 2 + timeToAdd; speed++)
                {
                    winningSituations += CheckIfWinning(speed, time, dist);
                }
                winningSituations *= 2;
                if (isEven)
                {
                    long speed = time / 2;
                    winningSituations += CheckIfWinning(speed, time, dist);
                }
                if (winningSituations > 0)
                    result *= winningSituations;
            }
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input); 
            long result = 1;
            var times = GetSplittedLine(lines, 0);
            var realTimes = string.Join("", times);
            var distances = GetSplittedLine(lines, 1);
            var realDistances = string.Join("", distances);
            var time = long.Parse(realTimes.Trim());
            var dist = long.Parse(realDistances.Trim());
            int winningSituations = 0;
            for (int speed = 1; speed < time; speed++)
            {
                winningSituations += CheckIfWinning(speed, time, dist);
            }
            if (winningSituations > 0)
                result *= winningSituations;

            Console.WriteLine(result);
        }

        private List<string> GetSplittedLine(List<string> lines, int lineIndex)
        {
            return lines.ElementAt(lineIndex).Split(" ").Where(d => d != "").Skip(1).ToList();
        }

        private int CheckIfWinning(long speed, long time, long dist)
        {
            var remainingTime = time - speed;
            var travelled = speed * remainingTime;
            if (travelled > dist)
                return 1;
            return 0;
        }
    }
}