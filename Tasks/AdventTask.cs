using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Tasks
{
   
    public abstract class AdventTask
    {
        public string Filename = $"Inputs\\";

        public abstract void Solve1(string input);
        public abstract void Solve2(string input);

        public List<string> GetLinesList(string input)
        {
            return input.Split("\n").ToList();
        }

        public string[] GetLinesArray(string input)
        {
            return input.Split("\n").ToArray();
        }
    }
}
