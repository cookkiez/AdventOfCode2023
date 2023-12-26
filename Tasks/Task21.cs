using System.Data;

namespace AdventOfCode2023.Tasks
{
    public class Task21 : AdventTask
    {
        public Task21()
        {
            Filename += "21.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            var grid = lines.Select(l => l.ToCharArray()).ToArray();
            var visited = new HashSet<(int, int)>();
            var positionQueue = new Queue<((int Row, int Col), long, Direction)>();
            var sLine = lines.Where(l => l.Contains('S')).First();
            var startPosition = (lines.IndexOf(sLine), sLine.IndexOf('S'));
            positionQueue.Enqueue((startPosition, 0, Direction.None));
            var maxSteps = 64;
            var rows = grid.Length - 1;
            var cols = grid[0].Length - 1;

            var currentPositions = new HashSet<(int Row, int Col)> { startPosition };
            for(int step = 0; step < maxSteps; step++)
            {
                var nextPositions = new HashSet<(int Row, int Col)>();
                foreach (var position in currentPositions)
                {
                    foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d =>  d != Direction.None))
                    {
                        var (nextRow, nextCol) = MakeMove(position, dir);
                        if (nextRow < 0 || nextCol < 0 || nextRow > rows || nextCol > cols) { continue; }
                        if (grid[nextRow][nextCol] == '#') { continue; }
                        nextPositions.Add((nextRow, nextCol));
                    }
                }
                currentPositions = nextPositions.ToHashSet();
            }
            Console.WriteLine(currentPositions.Count);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            var grid = lines.Select(l => l.ToCharArray()).ToArray();
            var positionQueue = new Queue<((int Row, int Col), long, Direction)>();
            var sLine = lines.Where(l => l.Contains('S')).First();
            var startPosition = (lines.IndexOf(sLine), sLine.IndexOf('S'));
            positionQueue.Enqueue((startPosition, 0, Direction.None));
            var rows = grid.Length - 1;
            var cols = grid[0].Length - 1;

            var positions = new Dictionary<string, long>();
            var positionsMap = new Dictionary<string, string>();
            var visited = new Dictionary<(int, int), List<(int, int)>>();
            var currentPositions = new HashSet<(int Row, int Col)> { startPosition };
            long result = 0;
            var gridLen = grid.Length;
            var temp = new List<long>();
            for (int step = 0; step < gridLen * 3; step++)
            {
                if (step % gridLen == 65) { temp.Add(currentPositions.Count); }
                var nextPositions = new HashSet<(int Row, int Col)>();
                foreach (var position in currentPositions)
                {
                    if (visited.ContainsKey(position)) { foreach (var p in visited[position]) { nextPositions.Add(p); } continue; }
                    var viablePositions = new List<(int, int)>();
                    foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d => d != Direction.None))
                    {
                        var (nextRow, nextCol) = MakeMove(position, dir);
                        var (_, _, viable, overflown) = CheckGrid(nextRow, nextCol, grid);
                        if (!viable) { continue; }
                        viablePositions.Add((nextRow, nextCol));
                    }           
                    foreach (var p in viablePositions) { nextPositions.Add(p); }
                    visited.Add(position, viablePositions);
                }
                currentPositions = nextPositions.ToHashSet();
            }

            // Calculate polynomial
            var a = temp.First();
            var b = temp.ElementAt(1);
            var c = temp.Last();

            var b1 = b - a;
            var c1 = c - b;
            long n = 26501365 / gridLen;
            result = a + b1 * n + (n * (n - 1) / 2) * (c1 - b1);
            Console.WriteLine(result);
        }

        private (int, int, bool, bool) CheckGrid(int row, int col, char[][] grid)
        {
            var overflown = false;
            if (row >= grid.Length)
            {
                row = (row % grid.Length);
                overflown = true;
            }
            if (row < 0)
            {
                var modulo = (Math.Abs(row) % grid.Length);
                if (modulo == 0) { row = 0; } 
                else { row = grid.Length - modulo; }
                overflown = true;
            }
            if (col >= grid[0].Length)
            {
                col = (col % grid[0].Length);
                overflown = true;
            }
            if (col < 0)
            {
                var modulo = (Math.Abs(col) % grid[0].Length);
                if (modulo == 0) { col = 0; }
                else { col = grid[0].Length - modulo; }
                overflown = true;
            }
            var isViable = true;
            if (grid[row][col] == '#') { isViable = false; }
            return (row, col, isViable, overflown);
        }
    }
}