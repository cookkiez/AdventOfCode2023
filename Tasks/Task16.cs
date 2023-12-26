namespace AdventOfCode2023.Tasks
{
    public class Task16 : AdventTask
    {
        public Task16()
        {
            Filename += "16.txt";
        }

        private class Beam
        {
            public int Row;
            public int Col;
            public Direction Dir;

            public Beam(int row, int col, Direction dir)
            {
                Row = row;
                Col = col;
                Dir = dir;
            }
        }

        // This is kind of ugly. Maybe a (c, dir) switch would be nicer. No time though :D
        private List<Direction> GetNextDirection(Direction dir, char c) =>
            c switch
            {
                '.' => new List<Direction> { dir },
                '|' => (dir == Direction.North || dir == Direction.South) ?
                        new List<Direction> { dir } : new List<Direction> { Direction.North, Direction.South },
                '\\' => new List<Direction> {
                        (dir == Direction.North) ? Direction.West :
                        (dir == Direction.South) ? Direction.East :
                        (dir == Direction.West) ? Direction.North :
                        Direction.South },
                '/' => new List<Direction> {
                        (dir == Direction.North) ? Direction.East :
                        (dir == Direction.South) ? Direction.West :
                        (dir == Direction.West) ? Direction.South :
                        Direction.North },
                '-' => (dir == Direction.West || dir == Direction.East) ?
                        new List<Direction> { dir } : new List<Direction> { Direction.West, Direction.East },
                _ => new List<Direction> { dir }
            };

        public override void Solve1(string input)
        {
            var grid = GetLinesList(input).Select(l => l.ToCharArray()).ToArray();
            long result = 0;
            var beams = new Queue<Beam>();
            beams.Enqueue(new Beam(0, 0, Direction.East));
            result = GetEnergized(beams, grid);
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            // This takes ~2 seconds. Better optimization would be some kind of path memoization. Didn't have time tho
            // First time I get to use multithreading - very nice
            var grid = GetLinesList(input).Select(l => l.ToCharArray()).ToArray();
            long result = 0;
            var tasks = new List<Task<long>>();
            for (int row = 0; row < grid.Length; row++)
            {
                for (int col = 0; col < grid[0].Length; col++)
                {   
                    // Filter out rows and columns that we don't want to consider.
                    if (row == 0 || row == grid.Length - 1 || col == 0 || col == grid[0].Length - 1)
                    {
                        var beams = new Queue<Beam>();
                        beams.Enqueue(GetStartingBeam(new Beam(row, col, Direction.North), row, col, grid));
                        tasks.Add(Task.Run(() => { return GetEnergized(beams, grid); }));
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
            foreach(var task in tasks) 
                if (task.Result > result) { result = task.Result; }
        
            Console.WriteLine(result);
        }

        private Beam GetStartingBeam(Beam beam, int row, int col, char[][] grid)
        {
            // Cases in GetBaseDirection hold for these if and else ifs as well.
            beam = GetBaseDirection(beam, col, grid);
            if (row == 0) { beam.Dir = Direction.South; }
            else if (row == grid.Length - 1)  { beam.Dir = Direction.North; }
            return beam;
        }

        private Beam GetBaseDirection(Beam beam, int col, char[][] grid) {
            if (col == 0) { beam.Dir = Direction.East; }
            else if (col == grid[0].Length - 1) { beam.Dir = Direction.West; }
            return beam;
        }

        private long GetEnergized(Queue<Beam> beams, char[][] grid)
        {
            var visited = new List<(int Row, int Col, Direction Dir)>();
            while (beams.TryDequeue(out var currentBeam))
            {
                var currentState = (currentBeam.Row, currentBeam.Col, currentBeam.Dir);
                if (visited.Contains(currentState)) { continue; }
                if (currentBeam.Row < 0 || currentBeam.Row >= grid.Length ||
                    currentBeam.Col < 0 || currentBeam.Col >= grid[0].Length) { continue; }
                visited.Add(currentState);
                var currentChar = grid[currentBeam.Row][currentBeam.Col];
                //Console.WriteLine($"{currentBeam.Row},{currentBeam.Col} - {currentChar} ");
                var nextDirs = GetNextDirection(currentBeam.Dir, currentChar);
                foreach (var dir in nextDirs)
                {
                    var nextBeam = new Beam(currentBeam.Row, currentBeam.Col, dir);
                    nextBeam = MakeMove(nextBeam);
                    beams.Enqueue(nextBeam);
                }
            }
            return visited.DistinctBy(v => (v.Row, v.Col)).Count();
        }

        private Beam MakeMove(Beam beam)
        {
            // Make a move according to the direction we are moving. 
            switch (beam.Dir)
            {
                case Direction.West:
                    beam.Col -= 1;
                    return beam;
                case Direction.East:
                    beam.Col += 1;
                    return beam;
                case Direction.South:
                    beam.Row += 1;
                    return beam;
                case Direction.North:
                    beam.Row -= 1;
                    return beam;
            }
            return beam;
        }
    }
}