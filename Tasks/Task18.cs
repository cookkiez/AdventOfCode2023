namespace AdventOfCode2023.Tasks
{
    public class Task18 : AdventTask
    {
        public Task18()
        {
            Filename += "18.txt";
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
            var commands = GetLinesList(input);
            var path = new List<(long Row, long Col)> { (0, 0) };
            (long Row, long Col) currentPosition = (0, 0);
            long pathArea = 0;
            foreach (var line in commands)
            {
                var lineSplitted = line.Split(" ");
                var (command, numSteps) = GetNumStepsAndCommand(part1, lineSplitted);
                currentPosition = GetNextPosition(currentPosition, command, numSteps, path);
                pathArea += numSteps;
            }
            return CalculateArea(pathArea, path);   
        }

        private (long Row, long Col) GetNextPosition((long Row, long Col) currentPosition, string command, 
            long numSteps, List<(long, long)> path)
        {
            var nextPosition = MakeMove(currentPosition, GetDirectionFromChar(command), numSteps);
            path.Add(nextPosition);
            return nextPosition;
        }

        private (string, long) GetNumStepsAndCommand(bool part1, string[] lineSplitted)
        {
            // Part 1 is simpler, in part two we have to decode a hex number to get the values
            if (part1)
            {
                var command = lineSplitted[0];
                var numSteps = long.Parse(lineSplitted[1]);
                return (command, numSteps);
            } else
            {
                var hex = lineSplitted[2].Replace("(", "").Replace(")", "");
                var command = char.ToString(hex[hex.Length - 1]);
                var hexNum = hex.Substring(1, hex.Length - 2);
                var numSteps = Convert.ToInt64(hexNum, 16);
                return (command, numSteps);
            }
        }

        private long CalculateArea(long pathArea, List<(long, long)> path)
        {
            long area = 0;
            int j = path.Count - 1;
            for (var i = 0; i < path.Count; i++)
            {
                var (r1, c1) = path.ElementAt(i);
                var (r2, c2) = path.ElementAt(j);
                area += (r1 + r2) * (c2 - c1);
                j = i;
            }
            area += pathArea;
            // Calculate the area again. For some reason the calculated value is not correct again. Have to add 1 to it.
            area = Math.Abs(area / 2) + 1;
            return area;
        }

        // In second part we have to use numbers as R, L, U and D
        private Direction GetDirectionFromChar(string ch) =>
            ch switch
            {
                "0" or "R" => Direction.East,
                "3" or "U" => Direction.North,
                "1" or "D" => Direction.South,
                "2" or "L" => Direction.West,
                _ => throw new Exception()
            };

        private (long Row, long Col) MakeMove((long Row, long Col) position, Direction movingDirection, long steps) =>
            movingDirection switch
            {
                Direction.West => (position.Row, position.Col - steps),
                Direction.East => (position.Row, position.Col + steps),
                Direction.South => (position.Row + steps, position.Col),
                Direction.North => (position.Row - steps, position.Col),
                _ => throw new Exception()
            };
    }
}