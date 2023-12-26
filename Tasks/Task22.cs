using System.Data;

namespace AdventOfCode2023.Tasks
{
    public class Task22 : AdventTask
    {
        public Task22()
        {
            Filename += "22.txt";
        }

        public override void Solve1(string input)
        {
            var bricks = GetBricks(input);
            var disintegratedBricks = new HashSet<Brick>();
            var safe = bricks.ToHashSet();
            foreach(var brick in bricks)
            {
                var supportedByCount = brick.SupportedBy.Distinct().Count();
                if (supportedByCount == 1)
                {
                    safe.Remove(brick.SupportedBy.First());
                }
                // No clue why this does not work. From what I can tell it should.
                // To my eyes its the same as the working solution above.
                //var supportingCount = brick.Supporting.Distinct().Count();
                //if (supportingCount == 0) { result++; disintegratedBricks.Add(brick); }
                //if (supportedByCount > 1) 
                //{ 
                //    result += brick.SupportedBy.Except(disintegratedBricks).Count();
                //    foreach(var b in brick.SupportedBy) { disintegratedBricks.Add(b); }
                //}
            }
            Console.WriteLine(safe.Count());
        }

        public override void Solve2(string input)
        {
            var bricks = GetBricks(input);
            var result = 0;
            foreach(var brick in bricks)
            {
                var queue = new Queue<Brick>();
                brick.Fallen = true;
                var visited = new HashSet<Brick>();
                foreach (var supported in brick.Supporting) { queue.Enqueue(supported); }
                while(queue.TryDequeue(out var supportedBrick))
                {
                    if (supportedBrick.SupportedBy.Any(b => !b.Fallen)) { continue; }
                    supportedBrick.Fallen = true;
                    foreach (var s in supportedBrick.Supporting) { queue.Enqueue(s); }
                }
                result += bricks.Where(b => b != brick && b.Fallen).Count();
                foreach (var b in bricks) { b.Fallen = false; }
            }
            Console.WriteLine(result);
        }

        private List<Brick> GetBricks(string input)
        {
            var lines = GetLinesList(input);

            var bricks = new List<Brick>();
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var minZ = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            var maxZ = int.MinValue;
            foreach (var line in lines)
            {
                var lineSplitted = line.Split("~");
                var start = lineSplitted.First().Split(",").Select(int.Parse).ToArray();
                var end = lineSplitted.Last().Split(",").Select(int.Parse).ToArray();
                var brick = new Brick(start[0], start[1], start[2], end[0], end[1], end[2]);
                minX = Math.Min(start[0], Math.Min(minX, end[0]));
                minY = Math.Min(start[1], Math.Min(minY, end[1]));
                minZ = Math.Min(start[2], Math.Min(minZ, end[2]));

                maxX = Math.Max(start[0], Math.Max(maxX, end[0]));
                maxY = Math.Max(start[1], Math.Max(maxY, end[1]));
                maxZ = Math.Max(start[2], Math.Max(maxZ, end[2]));
                bricks.Add(brick);
            }

            bricks = bricks.OrderBy(b => b.Z1).ToList();
            var filledSpots = new HashSet<(int x, int y, int z, Brick brick)>();

            var tetris = new Brick[maxX + 1, maxY + 1, maxZ + 1];

            foreach (var brick in bricks)
            {
                var currentSpots = new HashSet<(int x, int y, int z)>();
                while (brick.Z1 >= 1)
                {
                    var nextSpots = GetFilledSpots(brick);
                    var hit = false;
                    foreach (var spot in nextSpots)
                    {
                        var (x, y, z) = spot;
                        if (tetris[x, y, z] != null)
                        {
                            brick.SupportedBy.Add(tetris[x, y, z]);
                            tetris[x, y, z].Supporting.Add(brick);
                            hit = true;
                        }
                    }
                    if (hit) { break; }

                    currentSpots = nextSpots;
                    brick.Z1--;
                    brick.Z2--;
                }
                brick.Z1++;
                brick.Z2++;
                foreach (var spot in currentSpots)
                {
                    var (x, y, z) = spot;
                    tetris[x, y, z] = brick;
                }
            }
            return bricks;
        }

        private HashSet<(int x, int y, int z)> GetFilledSpots(Brick brick)
        {
            var spots = new HashSet<(int x, int y, int z)>();
            for(int x1 = brick.X1; x1 <= brick.X2; x1++)
            {
                for(int y1 = brick.Y1; y1 <= brick.Y2; y1++)
                {
                    for(int z1 = brick.Z1; z1 <= brick.Z2; z1++)
                    {
                        spots.Add((x1, y1, z1));
                    }
                }
            }

            return spots;
        }

        private class Brick(int x, int y, int z, int x2, int y2, int z2)
        {
            public int X1 = x;
            public int Y1 = y;
            public int Z1 = z;
            public int X2 = x2;
            public int Y2 = y2;
            public int Z2 = z2;
            public HashSet<Brick> SupportedBy = new HashSet<Brick>();
            public HashSet<Brick> Supporting = new HashSet<Brick>();
            public bool Fallen = false;
        }
    }
}