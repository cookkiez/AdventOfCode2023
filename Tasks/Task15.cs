using System.Text.RegularExpressions;

namespace AdventOfCode2023.Tasks
{
    public class Task15 : AdventTask
    {
        public Task15()
        {
            Filename += "15.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            foreach (var line in lines)
            {
                var commands = line.Split(",").ToList();
                foreach (var command in commands)
                {
                    result += GetHash(command);
                }
            }
            
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            var map = new Dictionary<int, List<Lens>>();
            foreach (var line in lines)
            {
                var commands = line.Split(",").ToList();
                foreach (var command in commands)
                {
                    var regex = new Regex("[a-z]{2,10}");
                    var matches = regex.Match(command);
                    var box = GetHash(matches.Value);
                    if (command.Contains('-'))
                    {
                        var label = command.Split("-").First();
                        // Remove the command from the map
                        if (map.ContainsKey(box))
                        {
                            map[box] = map[box].Where(l => l.Label != label).ToList();
                        }
                    } else
                    {
                        var commandSplitted = command.Split("=");
                        var label = commandSplitted.First();
                        var length = int.Parse(commandSplitted.Last());
                        var newLens = new Lens(length, label);
                        // If box already exists in map, check if lens already exist and change it. 
                        // Else add a new box.
                        if (map.ContainsKey(box))
                        {
                            var existingLens = map[box].Where(l => l.Label == newLens.Label).FirstOrDefault();
                            if (existingLens != null) { existingLens.FocalLength = newLens.FocalLength; } 
                            else { map[box].Add(newLens); }
                        } else { map[box] = new List<Lens>() { newLens }; }
                    }
                }
            }
            foreach(var box in map.Keys)
            {
                var lenses = map[box];
                var slot = 1;
                foreach(var lens in lenses)
                {
                    result += (box + 1) * slot * lens.FocalLength;
                    slot++;
                }
            }
            Console.WriteLine(result);
        }

        private int GetHash(string command)
        {
            int currentValue = 0;
            foreach (var c in command)
            {
                currentValue += c;
                currentValue *= 17;
                currentValue = currentValue % 256;
            }
            return currentValue;
        }

        private class Lens
        {
            public long FocalLength;
            public string Label;

            public Lens(long length, string label)
            {
                Label = label;
                FocalLength = length;
            }
        }
    }
}