using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Tasks
{
    public class Task4 : AdventTask
    {
        public Task4()
        {
            Filename += "4.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            double result = 0;
            foreach(var line in lines)
            {
                var splitted = line.Trim().Split(':');
                var nums = splitted[1].Split("|");
                var winning = nums[0].Split(" ").ToList();
                var my = nums[1].Split(" ").ToList();

                winning = winning.Where(w => w != "").ToList();
                my = my.Where(w => w != "").ToList();

                var won = winning.Intersect(my).ToList();
                if (won.Count > 0) 
                {
                    result += Math.Pow(2, won.Count - 1);
                }
            }
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            double result = 0;
            var copies = new int[lines.Count];
            int i = 0;
            foreach (var line in lines)
            {
                var splitted = line.Trim().Split(':');
                var nums = splitted[1].Split("|");
                var winning = nums[0].Split(" ").ToList();
                var my = nums[1].Split(" ").ToList();

                winning = winning.Where(w => w != "").ToList();
                my = my.Where(w => w != "").ToList();

                var won = winning.Intersect(my).ToList();
                result += 1 + copies[i];
                for (int k = 0; k < copies[i] + 1; k++)
                {
                    for (int j = 0; j < won.Count; j++)
                    {
                        copies[i + j + 1]++;
                    }
                }
                i++;
            }
            Console.WriteLine(result);
        }
    }
}
