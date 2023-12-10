using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.NetworkInformation;

namespace AdventOfCode2023.Tasks
{
    public class Task10 : AdventTask
    {
        public Task10()
        {
            Filename += "10.txt";
        }

        private enum Compass {
            North,
            East,
            South,
            West
        }

        private Compass GetNextDirection(Compass direction) {
            switch(direction) {
                case Compass.North:
                    return Compass.South;
                case Compass.East:
                    return Compass.West;
                case Compass.South:
                    return Compass.North;
                case Compass.West:
                    return Compass.East;
            }
            return Compass.North;
        }

        private Dictionary<char, List<Compass>> PossibleMoves = new Dictionary<char, List<Compass>> {
            {'-', new List<Compass>(){Compass.East, Compass.West}},
            {'|', new List<Compass>(){Compass.North, Compass.South}},
            {'L', new List<Compass>(){Compass.North, Compass.East}},
            {'J', new List<Compass>(){Compass.West, Compass.North}},
            {'7', new List<Compass>(){Compass.South, Compass.West}},
            {'F', new List<Compass>(){Compass.East, Compass.South}},
            {'S', new List<Compass>(){Compass.East, Compass.West, Compass.North, Compass.South} }        
        };

        private (long, long) MakeMove(long row, long col, Compass direction) {
            switch(direction) {
                case Compass.West:
                    return (row, col - 1);
                case Compass.East:
                    return (row, col + 1);
                case Compass.South:
                    return (row + 1, col);
                case Compass.North:
                    return (row - 1, col);
            }
            return (row, col);
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            var sLine = lines.Where(l => l.Contains("S")).First();
            var sRow = lines.IndexOf(sLine);
            var sCol = sLine.IndexOf("S");
            var pipeMap = lines.Select(line => line.Trim().ToCharArray()).ToArray();

            var nextMoves = new Queue<(long, long, Compass, char, long)>();
            var visitedCoords = new List<(long, long)>
            {
                (sRow, sCol)
            };

            foreach(var direction in PossibleMoves['S']) {
                var (nextRow, nextCol) = MakeMove(sRow, sCol, direction);
                if (nextRow > pipeMap.Length || nextRow < 0 || nextCol > pipeMap.Length || nextCol < 0) 
                    continue;
                var nextChar = pipeMap[nextRow][nextCol];
                if (PossibleMoves.ContainsKey(nextChar)) {
                    var nextDir = PossibleMoves[nextChar].Where(m => m == GetNextDirection(direction));
                    if (nextDir.Any()) 
                        nextMoves.Enqueue((nextRow, nextCol, PossibleMoves[nextChar].Where(m => m != GetNextDirection(direction)).First(), nextChar, 1));
                }
            }

            while(nextMoves.TryDequeue(out var move)) {
                var (row, col, dir, currChar, dist) = move;
                if (visitedCoords.Contains((row, col))){
                    result = dist;
                    break;
                }
                visitedCoords.Add((row, col));
                var (nextRow, nextCol) = MakeMove(row, col, dir);
                var nextChar = pipeMap[nextRow][nextCol];
                var nextDir = PossibleMoves[nextChar].Where(m => m != GetNextDirection(dir));
                nextMoves.Enqueue((nextRow, nextCol, nextDir.First(), nextChar, ++dist));
            }

            //var moveQueue = new Queue<>();
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            var sLine = lines.Where(l => l.Contains("S")).First();
            var sRow = lines.IndexOf(sLine);
            var sCol = sLine.IndexOf("S");
            var pipeMap = lines.Select(line => line.Trim().ToCharArray()).ToArray();

            var nextMoves = new Queue<(long, long, Compass, char, long)>();
            var visitedCoords = new List<(long, long)>
            {
                (sRow, sCol)
            };

            foreach(var direction in PossibleMoves['S']) {
                var (nextRow, nextCol) = MakeMove(sRow, sCol, direction);
                if (nextRow > pipeMap.Length || nextRow < 0 || nextCol > pipeMap[0].Length || nextCol < 0) 
                    continue;
                var nextChar = pipeMap[nextRow][nextCol];
                if (PossibleMoves.ContainsKey(nextChar)) {
                    var nextDir = PossibleMoves[nextChar].Where(m => m == GetNextDirection(direction));
                    if (nextDir.Any()) 
                        nextMoves.Enqueue((nextRow, nextCol, PossibleMoves[nextChar].Where(m => m != GetNextDirection(direction)).First(), nextChar, 1));
                }
            }

            long minRow = sRow, maxRow = sRow;
            long minCol = sCol, maxCol = sCol;
            while(nextMoves.TryDequeue(out var move)) {
                var (row, col, dir, currChar, dist) = move;
                if (visitedCoords.Contains((row, col))){
                    result = dist;
                    break;
                }
                visitedCoords.Add((row, col));
                if (row < minRow) { minRow = row; }
                else if (row > maxRow) { maxRow = row; }
                if (col < minCol) { minCol = col; }
                else if (col > maxCol) { maxCol = col; }  
                var (nextRow, nextCol) = MakeMove(row, col, dir);
                var nextChar = pipeMap[nextRow][nextCol];
                var nextDir = PossibleMoves[nextChar].Where(m => m != GetNextDirection(dir));
                nextMoves.Enqueue((nextRow, nextCol, nextDir.First(), nextChar, ++dist));
            }

            result = 0;
            var temp = new List<(long, long)> ();
            for(long row = minRow + 1; row < maxRow; row++) {
                for (long col = minCol + 1; col < maxCol; col++) {
                    var point = (row, col);
                    if (visitedCoords.Contains(point)) { continue; }
                    long counts = 0;
                    for(long newCol = col; newCol < ((maxCol < pipeMap[0].Length) ? maxCol + 1 : pipeMap.Length); newCol++){
                        if (visitedCoords.Contains((row, newCol)))
                            counts++;
                        while(visitedCoords.Contains((row, newCol))) {
                            newCol++;
                        }
                    }
                    if (counts % 2 == 1) {
                        result++;
                        temp.Add(point);
                    }
                        
                }
            }
            Console.WriteLine(result);
        }
    }
}