namespace AdventOfCode2023.Tasks
{
    public class Task17 : AdventTask
    {
        public Task17()
        {
            Filename += "17.txt";
        }

        private record Block(int Row, int Col, Direction Dir, int NumStraight, int Distance);

        public override void Solve1(string input)
        {
            var grid = GetLinesList(input).Select(l => l.ToCharArray()).ToArray();
            long result = GetMinimumHeat(grid, 1, 3);
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var grid = GetLinesList(input).Select(l => l.ToCharArray()).ToArray();
            long result = GetMinimumHeat(grid, 4, 10);
            Console.WriteLine(result);
        }

        private int GetMinimumHeat(char[][] grid, int minStraight, int maxStraight)
        {
            var visited = new HashSet<(int, int, int, Direction)>();
            var rows = grid.Length - 1;
            var cols = grid[0].Length - 1;
            int result = -1;

            // Starting positions. Have to use south and east because of the minStraight constraint.
            Block down = new(0, 0, Direction.South, 0, 0);
            Block right = new(0, 0, Direction.East, 0, 0);
            var queue = new PriorityQueue<Block, int>();
            queue.Enqueue(down, 0);
            queue.Enqueue(right, 0);           
            while (queue.TryDequeue(out var currentBlock, out var _))
            {
                // Found the last block. Have to check the minStraight parameter.
                if (currentBlock.Row == rows && currentBlock.Col == cols && currentBlock.NumStraight >= minStraight) 
                { 
                    result = currentBlock.Distance; 
                    break;
                }
                var cameFrom = GetPreviousDirection(currentBlock.Dir);
                // Iterate over all directions in the Enum - excluding the direction that we came from.
                foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(d => d != cameFrom))
                {
                    var (nextRow, nextCol) = MakeMove(currentBlock, dir);
                    if (nextRow < 0 || nextCol < 0 || nextRow > rows || nextCol > cols) { continue; }

                    // If same direction, make sure that we don't make too many moves in it.
                    var numStraight = 1;
                    if (dir == currentBlock.Dir)
                    {
                        numStraight = currentBlock.NumStraight + 1;
                        if (numStraight > maxStraight) { continue; }
                    }

                    var cost = (int)char.GetNumericValue(grid[nextRow][nextCol]);
                    Block nextBlock = new(nextRow, nextCol, dir, numStraight, currentBlock.Distance + cost);

                    // If trying to change direction, make sure that we have made enough moves in this direction
                    if (nextBlock.Dir != currentBlock.Dir && currentBlock.NumStraight < minStraight)
                    {
                        continue;
                    }

                    var blockKey = (nextBlock.Row, nextBlock.Col, numStraight, nextBlock.Dir);
                    if (visited.Contains(blockKey)) { continue; }
                    visited.Add(blockKey);
                    queue.Enqueue(nextBlock, nextBlock.Distance);
                }
            }
            return result;
        }

        private (int Row, int Col) MakeMove(Block block, Direction movingDirection) =>
            movingDirection switch
            {
                Direction.West => (block.Row, block.Col - 1),
                Direction.East => (block.Row, block.Col + 1),
                Direction.South => (block.Row + 1, block.Col),
                Direction.North => (block.Row - 1, block.Col),
                _ => throw new Exception()
            };
    }
}