namespace AdventOfCode2023.Tasks
{
    public class Task23 : AdventTask
    {
        public Task23()
        {
            Filename += "23.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            var startCol = lines.First().IndexOf(".");
            var startPos = (0, startCol);

            var grid = lines.Select(l => l.ToCharArray()).ToArray();

            var visited = new Dictionary<(int Row, int Col), long>();
            var queue = new Queue<((int Row, int Col), Direction dir, List<(int, int)> path)>();
            queue.Enqueue((startPos, Direction.South, new List<(int, int)> { startPos }));

            var specialMoves = new List<char> { '>', '<', 'v', '^' };

            var finished = new List<long>();
            var paths = new List<List<(int, int)>>();

            while (queue.TryDequeue(out var state))
            {
                var ((row, col), movingDir, path) = state;

                var cameFrom = GetPreviousDirection(movingDir);
                // Iterate over all directions in the Enum - excluding the direction that we came from.
                foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d => d != cameFrom && d != Direction.None))
                {
                    var (nextRow, nextCol) = MakeMove((row, col), dir);
                    if (nextRow < 0 || nextRow >= grid.Length || nextCol < 0 || nextCol >= grid[0].Length) { continue; }
                    if (grid[nextRow][nextCol] == '#') { continue; }
                    var tempPath = path.ToList();
                    if (tempPath.Contains((nextRow, nextCol))) { continue; }

                    if (specialMoves.Contains(grid[nextRow][nextCol]))
                    {
                        var (nextRow1, nextCol1) = MakeMove((nextRow, nextCol), GetSpecialDirection(grid[nextRow][nextCol]));
                        tempPath.Add((nextRow, nextCol));
                        if (tempPath.Contains((nextRow1, nextCol1))) { continue; }
                        nextRow = nextRow1;
                        nextCol = nextCol1;
                    }

                    tempPath.Add((nextRow, nextCol));
                    if (nextRow == grid.Length - 1) { finished.Add(tempPath.Count); }

                    queue.Enqueue(((nextRow, nextCol), dir, tempPath));
                }
            }

            Console.WriteLine(finished.OrderDescending().First());
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            var startCol = lines.First().IndexOf(".");
            var startPos = (0, startCol);
            var endPos = (lines.Count() - 1, lines[0].Length - 2);
            var graph = new Dictionary<(int, int), List<((int, int), int)>> ();
            var grid = lines.Select(l => l.ToCharArray()).ToArray();

            graph.Add(startPos, new List<((int, int), int)>());

            var visited = new HashSet<(int, int)> ();
            var queue = new Queue<((int, int), Direction, (int, int))> ();
            queue.Enqueue((startPos, Direction.South, startPos));
            visited.Add(startPos);
            while (queue.TryDequeue(out var currState))
            {
                var (currPos, currDir, startingPos) = currState;
                var cameFrom = GetPreviousDirection(currDir);
                var validDirs = CheckValidMoves(grid, cameFrom, currPos.Item1, currPos.Item2, new List<(int, int)>());
                var (nextRow, nextCol) = currPos;
                var dist = currPos == startPos ? 0 : 1;
                while (validDirs.Count == 1)
                {
                    var nextDir = validDirs.First();
                    (nextRow, nextCol) = MakeMove((nextRow, nextCol), nextDir);
                    visited.Add((nextRow, nextCol));
                    validDirs= CheckValidMoves(grid, GetPreviousDirection(nextDir), nextRow, nextCol, new List<(int, int)>());
                    dist++;
                }

                AddToGraph(graph, (nextRow, nextCol), startingPos, dist);
                AddToGraph(graph, startingPos, (nextRow, nextCol), dist);
                visited.Add((nextRow, nextCol));
                foreach (var dir in validDirs)
                {
                    var (nRow, nCol) = MakeMove((nextRow, nextCol), dir);
                    if (visited.Contains((nRow, nCol))) { continue; }
                    visited.Add((nRow, nCol));
                    queue.Enqueue(((nRow, nCol), dir, (nextRow, nextCol)));
                }
            }
            Console.WriteLine(GetLongestPath(graph, startPos, endPos, 0, new HashSet<(int, int)>()));
        }

        private int GetLongestPath(Dictionary<(int, int), List<((int, int), int)>> graph, 
            (int, int) currentNode, (int, int) endNode, int length, HashSet<(int, int)> visited)
        {
            if (currentNode == endNode) { 
                return length; }

            var maxLen = 0;
            visited.Add(currentNode);
            foreach(var neigh in graph[currentNode])
            {
                var (neighPos, dist) = neigh;
                if (visited.Contains(neighPos)) { continue; }
                maxLen = Math.Max(maxLen, GetLongestPath(graph, neighPos, endNode, length + dist, visited));
            }
            visited.Remove(currentNode);
            return maxLen;
        }

        private void AddToGraph(Dictionary<(int, int), List<((int, int), int)>> graph, (int, int) key, (int, int) toAdd, int dist)
        {
            if (graph.ContainsKey(key))
            {
                var tempL = graph[key];
                if (!tempL.Any(p => p.Item1 == toAdd)) { tempL.Add((toAdd, dist)); }
            } else
            {
                graph.Add(key, new List<((int, int), int)> { (toAdd, dist) });
            }
        }

        private List<Direction> CheckValidMoves(char[][] grid, Direction cameFrom, int row, int col, List<(int, int)> path)
        {
            var directions = new List<Direction>();
            // Iterate over all directions in the Enum - excluding the direction that we came from.
            foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d => d != cameFrom && d != Direction.None))
            {
                var (nextRow, nextCol) = MakeMove((row, col), dir);
                if (nextRow < 0 || nextRow >= grid.Length || nextCol < 0 || nextCol >= grid[0].Length) { continue; }
                if (grid[nextRow][nextCol] == '#') { continue; }
                if (path.Contains((nextRow, nextCol))) { continue; }
                directions.Add(dir);
            }
            return directions;
        }

        private Direction GetSpecialDirection(char specialMove) =>
            specialMove switch
            {
                '<' => Direction.West,
                '>' => Direction.East,
                'v' => Direction.South,
                '^' => Direction.North,
                _ => throw new Exception()
            };
    }
}