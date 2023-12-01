﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Task1
{
    public class Task1
    {
        public void Solve1(string input)
        {
            var splitted = input.Split("\n").ToList();
            var sum = 0;
            foreach (var line in splitted)
            {
                var prevDigit = "";
                var toSum = "";
                foreach (var ch in line)
                {
                    if (Char.IsNumber(ch))
                    {
                        if (prevDigit == "")
                        {
                            toSum += ch;
                        }

                        prevDigit = Char.ToString(ch);
                    }
                }
                if (prevDigit != "")
                {
                    toSum += prevDigit;
                    sum += int.Parse(toSum);
                }   
            }
            Console.WriteLine(sum);
            var a = 0;
        }

        public void Solve2(string input)
        {
            var splitted = input.Split("\n").ToList();
            var sum = 0;
            var validStringDigits = new List<(string, string)> { 
                ("one", "1"), 
                ("two", "2"), 
                ("three", "3"), 
                ("four", "4"), 
                ("five", "5"),
                ("six", "6"), 
                ("seven", "7"), 
                ("eight", "8"), 
                ("nine", "9"),
                ("1", "1"),
                ("2", "2"),
                ("3", "3"),
                ("4", "4"),
                ("5", "5"),
                ("6", "6"),
                ("7", "7"),
                ("8", "8"),
                ("9", "9"),
            };
            foreach (var line in splitted)
            {
                var results = new List<(int, string)>();
                foreach (var (vsd, value) in validStringDigits)
                {
                    fun(line, vsd, value, 0, results);
                }
                results.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                var num = string.Concat(results.FirstOrDefault().Item2, results.LastOrDefault().Item2);
                sum += int.Parse(num);
                var a = 0;
            }

            Console.WriteLine(sum);
        }

        private void fun(string line, string vsd, string value, int startingPosition, List<(int, string)> toReturn)
        {
            var index = line.IndexOf(vsd, startingPosition);
            if (index >= 0)
            {
                toReturn.Add((index, value));
                fun(line, vsd, value, index + 1, toReturn);
            }
            return;
        }
    }
}
