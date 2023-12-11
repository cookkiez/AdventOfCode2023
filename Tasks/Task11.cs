using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2023.Tasks
{
    public class Task11 : AdventTask
    {
        /*
         * First tried with a BFS. Turned out to be a bad idea :D even the test case took like 20s (even with optimization)
         * Then I figured that I can just calculate the distances (Thankfully only after wasting an hour).
         */
        public Task11()
        {
            Filename += "11.txt";
        }

        private class Galaxy
        {
            public long Row;
            public long Column;

            public Galaxy(long row, long column)
            {
                Row = row;
                Column = column;
            }
        }

        private (char[][], List<int>, List<int>) ExpandUniverse(List<string> lines)
        {
            // Get empty rows
            var emptyRows = Enumerable.Range(0, lines.Count)
                         .Where(i => !lines[i].Contains("#"))
                         .ToList();

            // There is probably a nicer way to get empty columns but I cant find it right now. 
            var linesWithCharArray = lines.Select(l => l.Trim().ToCharArray()).ToArray();
            var emptyCols = new List<int>();
            for (var col = 0; col < linesWithCharArray[0].Length; col++)
            {
                var hasGalaxy = false;
                for (var row = 0; row < linesWithCharArray.Length; row++)
                {
                    if (linesWithCharArray[row][col] == '#')
                    {
                        hasGalaxy = true;
                        break;
                    }
                }
                if (!hasGalaxy)
                {
                    emptyCols.Add(col);
                }
            }            
            return (lines.Select(l => l.ToCharArray()).ToArray(), emptyRows, emptyCols);
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input).Select(l => l.Trim()).ToList();
            long result = 0;
            var (universe, emptyRows, emptyCols) = ExpandUniverse(lines);

            var galaxies = GetGalaxies(universe);
            result = GetDistances(1, galaxies, emptyRows, emptyCols);

            Console.WriteLine(result / 2);
        }

        private List<Galaxy> GetGalaxies(char[][] universe)
        {
            var galaxies = new List<Galaxy>();
            for (long row = 0; row < universe.Length; row++)
            {
                for (long col = 0; col < universe[0].Length; col++)
                {
                    if (universe[row][col] == '#')
                    {
                        galaxies.Add(new Galaxy(row, col));
                    }
                }
            }
            return galaxies;
        }

        private long GetDistances(long toAdd, List<Galaxy> galaxies, List<int> emptyRows, List<int> emptyCols)
        {
            // Calculates shortest paths distance between each galaxy (it's just euclidean distance)
            long distance = 0;
            foreach (var g1 in galaxies)
            {
                foreach (var g2 in galaxies)
                {
                    if (g1 != g2)
                    {
                        var r = Math.Abs(g1.Row - g2.Row);
                        var c = Math.Abs(g1.Column - g2.Column);
                        distance += r + c;
                        // Account for universe expansion (in first part by 1, in second part by 100000)
                        foreach (var row in emptyRows)
                        {
                            if (row > Math.Min(g1.Row, g2.Row) && row < Math.Max(g1.Row, g2.Row)) { distance += toAdd; }
                        }
                        foreach (var col in emptyCols)
                        {
                            if (col > Math.Min(g1.Column, g2.Column) && col < Math.Max(g1.Column, g2.Column)) { distance += toAdd; }
                        }
                    }
                }
            }
            return distance;
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input).Select(l => l.Trim()).ToList();
            long result = 0;
            var (universe, emptyRows, emptyCols) = ExpandUniverse(lines);

            var galaxies = GetGalaxies(universe);
            result = GetDistances(999999, galaxies, emptyRows, emptyCols); 

            Console.WriteLine(result / 2);
        }
    }
} 