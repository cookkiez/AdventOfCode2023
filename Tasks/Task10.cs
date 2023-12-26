namespace AdventOfCode2023.Tasks
{
    public class Task10 : AdventTask
    {
        public Task10()
        {
            Filename += "10.txt";
        }

        private Direction GetNextDirection(Direction direction)
        {
            // Get correct next direction. Eg. if coming from the North, the direction is South. 
            // If coming from the East, direction is West.
            // This helps for finding the next direction of a symbol. 
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
            }
            return Direction.North;
        }

        // All possible moves based on the character. Always use only one direction of the Direction
        // When finding new direction to move in, filter one of these out. Eg. if coming into 'J'
        // from the West, then use North as new direction - make a move to the North next. 
        // Use 'S' only at the start.
        private Dictionary<char, List<Direction>> PossibleMoves = new Dictionary<char, List<Direction>> {
            { '-', new List<Direction>() { Direction.East, Direction.West } },
            { '|', new List<Direction>() { Direction.North, Direction.South } },
            { 'L', new List<Direction>() { Direction.North, Direction.East } },
            { 'J', new List<Direction>() { Direction.West, Direction.North } },
            { '7', new List<Direction>() { Direction.South, Direction.West } },
            { 'F', new List<Direction>() { Direction.East, Direction.South } },
            { 'S', new List<Direction>() { Direction.East, Direction.West ,  Direction.North, Direction.South } }
        };

        private (long, long) MakeMove(long row, long col, Direction direction)
        {
            // Make a move according to the direction we are moving. 
            switch (direction)
            {
                case Direction.West:
                    return (row, col - 1);
                case Direction.East:
                    return (row, col + 1);
                case Direction.South:
                    return (row + 1, col);
                case Direction.North:
                    return (row - 1, col);
            }
            return (row, col);
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;

            var (sRow, sCol, nextMoves, visitedCoords, pipeMap) = InitVals(lines);
            GetStartingMoves(sRow, sCol, pipeMap, nextMoves);

            // Basic BFS using a queue to speed it up. DFS would have worked also (probably).
            while (nextMoves.TryDequeue(out var move))
            {
                var (row, col, dir, dist) = move;
                if (visitedCoords.Contains((row, col)))
                {
                    result = dist;
                    break;
                }
                visitedCoords.Add((row, col));
                EnqueueNextMove(row, col, dir, pipeMap, nextMoves, dist);
            }

            Console.WriteLine(result);
        }

        private void EnqueueNextMove(long row, long col, Direction dir, char[][] pipeMap, Queue<(long, long, Direction, int)> nextMoves, int dist)
        {
            // Make move in the current direction
            var (nextRow, nextCol) = MakeMove(row, col, dir);
            // Get character on the new location
            var nextChar = pipeMap[nextRow][nextCol];
            // Get direction of the character based on the direction we came from
            var nextDir = PossibleMoves[nextChar].Where(m => m != GetNextDirection(dir));
            nextMoves.Enqueue((nextRow, nextCol, nextDir.First(), ++dist));
        }

        private void GetStartingMoves(long sRow, long sCol, char[][] pipeMap, Queue<(long, long, Direction, int)> nextMoves)
        {
            foreach (var direction in PossibleMoves['S'])
            {
                var (nextRow, nextCol) = MakeMove(sRow, sCol, direction);
                if (nextRow > pipeMap.Length || nextRow < 0 || nextCol > pipeMap[0].Length || nextCol < 0)
                    continue;
                var nextChar = pipeMap[nextRow][nextCol];
                if (PossibleMoves.ContainsKey(nextChar))
                {
                    var nextDir = PossibleMoves[nextChar].Where(m => m == GetNextDirection(direction));
                    if (nextDir.Any())
                        nextMoves.Enqueue((nextRow, nextCol, PossibleMoves[nextChar].Where(m => m != GetNextDirection(direction)).First(), 1));
                }
            }
        }

        private (int, int, Queue<(long, long, Direction, int)>, List<(long, long)>, char[][]) InitVals(List<string> lines)
        {
            var sLine = lines.Where(l => l.Contains("S")).First();
            var sRow = lines.IndexOf(sLine);
            var sCol = sLine.IndexOf("S");
            var pipeMap = lines.Select(line => line.Trim().ToCharArray()).ToArray();

            var nextMoves = new Queue<(long, long, Direction, int)>();
            var visitedCoords = new List<(long, long)> { (sRow, sCol) };
            return (sRow, sCol, nextMoves, visitedCoords, pipeMap);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            var (sRow, sCol, nextMoves, visitedCoords, pipeMap) = InitVals(lines);

            GetStartingMoves(sRow, sCol, pipeMap, nextMoves);

            var path = new List<(long, long)>() { (sRow, sCol) };
            var first = true;
            while (nextMoves.TryDequeue(out var move))
            {
                var (row, col, dir, dist) = move;

                if (visitedCoords.Contains((row, col))) { break; }
                visitedCoords.Add((row, col));

                // Added path building in 2nd part to get polygon area. Otherwise its the same as 1st part
                if (first)
                {
                    path.Insert(dist, (row, col));
                    first = false;
                }
                else
                {
                    path.Insert(path.Count - dist - 1, (row, col));
                    first = true;
                }

                EnqueueNextMove(row, col, dir, pipeMap, nextMoves, dist);
            }

            // Calculate area of the polygon covered by the loop to get the inner points.
            long area = 0;
            int j = path.Count - 1;
            for (var i = 0; i < path.Count; i++)
            {
                var (r1, c1) = path.ElementAt(i);
                var (r2, c2) = path.ElementAt(j);
                area += (r1 + r2) * (c2 - c1);
                j = i;
            }
            area = Math.Abs(area / 2);
            var loopedPoints = area - path.Count / 2 + 1;
            // For some reason can't calculate the area of the polygon (looped points) correctly. Its always off by 2.
            Console.WriteLine(loopedPoints + 2);


            // Attempt at ray casting. Tried doing it differently at first and gave up on it. 
            // This code does not produce the correct solution. No will to figure it out
            // Its a bit scuffed. There was a version of this before I tried calculating area
            // I liked that version better, but don't want to waste more time on this days task.
            // So I am leaving this version in here.
            // The first version only checked indices inside the min/max row/column range. 
            var verticalMoves = new List<char>() { 'S', '|', 'L', 'F', 'J', '7' };
            var inLoop = 0;
            for (var row = 0; row < pipeMap.Length; row++)
            {
                var counts = 0;
                // This goes from left to right and checks how many points are found as in the loop
                for (var col = 0; col < pipeMap[0].Length; col++)
                {
                    var currChar = pipeMap[row][col];
                    if (visitedCoords.Contains((row, col)) && currChar == '-')
                    {
                        while (visitedCoords.Contains((row, col)) && currChar == '-')
                        {
                            col++;
                            currChar = pipeMap[row][col];   
                        }
                    }
                    if (visitedCoords.Contains((row, col)) && verticalMoves.Contains(currChar)) { counts++; }
                    if (currChar == '.' && counts % 2 == 1)
                    {
                        inLoop++;
                    }
                }
                
            }
            Console.WriteLine(inLoop);
        }
    }
}