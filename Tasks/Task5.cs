using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Tasks
{
    public class Task5 : AdventTask
    {
        public Task5()
        {
            Filename += "5.txt";
        }

        public override void Solve1(string input)
        {
            var lines = input.Split("\r\n\r").Where(s => s != "" || s != string.Empty).ToList();
            long result = -1;
            var seeds = lines.FirstOrDefault().Trim().Split(": ").LastOrDefault().Split(" ").Select(long.Parse).ToList();
            foreach (var seed in seeds)
            {
                var mappedSeed = seed;
                foreach (var line in lines.Skip(1))
                {
                    var trimmed = line.Trim();
                    var splitted = line.Trim().Split("\n").ToList();

                    foreach (var map in splitted.Skip(1))
                    {
                        var mapNums = map.Trim().Split(" ").Select(long.Parse);
                        var dest = mapNums.ElementAt(0);
                        var src = mapNums.ElementAt(1);
                        var rng = mapNums.ElementAt(2);

                        if (mappedSeed >= src && mappedSeed <= src + rng)
                        {
                            mappedSeed = dest + mappedSeed - src;
                            break;
                        }
                    }
                }
                if (result == -1 || result > mappedSeed)
                {
                    result = mappedSeed;
                }
            }
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = input.Split("\r\n\r").Where(s => s != "" || s != string.Empty).ToList();
            long result = -1;
            // use longs
            var seeds = lines.FirstOrDefault().Trim().Split(": ").LastOrDefault().Split(" ").Select(long.Parse).ToList();
            var ranges = new List<(long, long)>();
            for(int i = 0; i < seeds.Count - 1; i+=2)
            {
                ranges.Add((seeds.ElementAt(i), seeds.ElementAt(i + 1)));
            }

            foreach (var (start, numOfSeeds) in ranges)
            {
                var possibleRanges = new List<(long, long)>() { (start, numOfSeeds) }; 
                foreach (var line in lines.Skip(1))
                {
                    var trimmed = line.Trim();
                    var splitted = line.Trim().Split("\n").ToList();
                    var newPossibleRanges = new List<(long, long)>();
                    var tempPossibleRanges = new List<(long, long)>();
                    foreach (var map in splitted.Skip(1))
                    {
                        var mapNums = map.Trim().Split(" ").Select(long.Parse).ToList();
                        var dest = mapNums.ElementAt(0);
                        var src = mapNums.ElementAt(1);
                        var rng = mapNums.ElementAt(2);
                        var mapStart = src;
                        var mapEnd = src + rng;
                        tempPossibleRanges = new List<(long, long)>();
                        foreach (var (s, n) in possibleRanges)
                        {
                            // Three cases - overflow, inflow, underflow
                            if (s >= mapStart && s + n <= mapEnd)
                            {
                                newPossibleRanges.Add((dest + s - src, n));
                            } else if (s < mapStart && s + n < mapStart ||
                                s > mapEnd && s + n > mapEnd)
                            {
                                tempPossibleRanges.Add((s, n));
                            } else
                            {
                                // check overflow and underflow
                                if (s + n > mapEnd)
                                {
                                    var overflow = s + n - mapEnd;
                                    tempPossibleRanges.Add((mapEnd, overflow));
                                    // Have to account for start of range to be smaller that mapStart,
                                    // in this case use mapStart
                                    var newS = (s > mapStart) ? s : mapStart;
                                    var temp = dest + newS - src;
                                    newPossibleRanges.Add((temp, n - overflow));
                                } 
                                if (s < mapStart)
                                {
                                    var underflow = mapStart - s;
                                    tempPossibleRanges.Add((s, underflow));
                                    newPossibleRanges.Add((dest, n - underflow));
                                }
                            }
                            var a = 0;
                        }
                        possibleRanges = tempPossibleRanges.ToList();
                    }
                    possibleRanges = tempPossibleRanges.ToList();
                    possibleRanges.AddRange(newPossibleRanges.ToList());
                }
                var (val, _) = possibleRanges.OrderBy(x => x.Item1).FirstOrDefault();
                if (result == -1 || val < result) { result = val; }
            }
            Console.WriteLine(result);
        }
    }
}