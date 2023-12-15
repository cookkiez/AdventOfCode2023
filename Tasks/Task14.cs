namespace AdventOfCode2023.Tasks
{
    public class Task14 : AdventTask
    {
        /*
         * First part is super fast as it is pretty simple. Second part takes ~3s. Still
         * faster than some of the solutions I saw online.
         */
        public Task14()
        {
            Filename += "14.txt";
        }

        private enum Direction
        {
            North, West, South, East
        }

        public override void Solve1(string input)
        {
            var grid = GetLinesArray(input);
            long load = CheckVerticalMovement(grid, true, new List<(int, int, char)>());
            Console.WriteLine(load);
        }

        public override void Solve2(string input)
        {
            var grid = GetLinesArray(input);
            long load = 0;
            var prevRocks = new List<(int Col, int Row, char C)>();

            // Shouldn't have reused this function. Its kinda ugly, but going to leave it like this
            load = CheckVerticalMovement(grid, true, prevRocks);

            // Because of this - 1 I had a bug that I was hunting for a few hours.
            var gridRows = grid.Length - 1;
            var gridCols = grid[0].Length - 1;

            var newRocks = new List<(int Col, int Row, char C)>();
            var firstRun = true;
            var seenPatterns = new List<string>();

            int counter = 0;
            var maxPivots = 1000000000;
            while (counter < maxPivots)
            {
                counter++;
                foreach (var dir in new List<Direction> { Direction.North, Direction.West, Direction.South, Direction.East })
                {
                    load = 0;
                    var currRocks = newRocks.Count == 0 ? prevRocks.ToList() : newRocks.ToList();
                    if (!firstRun)
                    {
                        newRocks = new List<(int Col, int Row, char C)>();
                        var currentClosestRock = GetFirstRock(dir, gridRows, gridCols);
                        currRocks = OrderRocksList(currRocks, dir);
                        foreach (var rock in currRocks)
                        {
                            // we don't need to check anything else in this case
                            if (rock.C == '#')
                            {
                                currentClosestRock = rock;
                                newRocks.Add(rock);
                                continue;
                            } else
                            {
                                currentClosestRock = GetNextClosestRock(rock, currentClosestRock, dir, gridCols, gridRows);
                                load += gridRows + 1 - currentClosestRock.Row;
                                newRocks.Add(currentClosestRock);
                            }
                        }
                    }
                    firstRun = false;
                    prevRocks = currRocks.ToList();
                }
                // Make a string "hash" of the pattern for memo
                var currPattern = "";
                foreach (var rock in newRocks.OrderBy(r => r.Row).ThenBy(r => r.Col))
                {
                    currPattern += $"{rock.C}{rock.Row}{rock.Col}";
                }
                // If found a pattern that exists, skip with counter to the end.
                // Then iterate until counter equals the limit.
                if (seenPatterns.Contains(currPattern))
                {
                    var patternIndex = seenPatterns.IndexOf(currPattern) + 1;
                    var index = (maxPivots - counter) / (counter - patternIndex);
                    counter += index * (counter - patternIndex);
                }
                seenPatterns.Add(currPattern);
            }
            Console.WriteLine(load);
        }

        private (int Col, int Row, char C) GetFirstRock(Direction direction, int gridRows, int gridCols) =>
            direction switch
            {
                Direction.North => (0, -1, ' '),
                Direction.South => (0, gridRows + 1, ' '),
                Direction.West => (-1, 0, ' '),
                Direction.East => (gridCols + 1, 0, ' '),
                _ => throw new NotImplementedException(),
            };

        private List<(int Col, int Row, char C)> OrderRocksList(List<(int Col, int Row, char C)> rocks, Direction direction)
        {
            switch(direction)
            {
                case Direction.North:
                    rocks = rocks.OrderBy(r => r.Col).ThenBy(r => r.Row).ToList();
                    break;
                case Direction.South:
                    rocks = rocks.OrderBy(r => r.Col).ThenByDescending(r => r.Row).ToList();
                    break;
                case Direction.West:
                    rocks = rocks.OrderBy(r => r.Row).ThenBy(r => r.Col).ToList();
                    break;
                case Direction.East:
                    rocks = rocks.OrderBy(r => r.Row).ThenByDescending(r => r.Col).ToList();
                    break;
            }
            return rocks;
        }

        private (int Col, int Row, char C) GetNextClosestRock((int Col, int Row, char C) rock, (int Col, int Row, char C) currentClosestRock,
            Direction direction, int gridRows, int gridCols)
        {
            var (closestCol, closestRow, _) = currentClosestRock;
            switch(direction)
            {
                case Direction.North:
                    if (closestCol < rock.Col) { currentClosestRock = (rock.Col, 0, rock.C); }
                    else { currentClosestRock = (rock.Col, closestRow + 1, rock.C); }
                    break;
                case Direction.South:
                    if (closestCol < rock.Col) { currentClosestRock = (rock.Col, gridRows, rock.C); }
                    else { currentClosestRock = (rock.Col, closestRow - 1, rock.C); }
                    break;
                case Direction.West:
                    if (closestRow < rock.Row) { currentClosestRock = (0, rock.Row, rock.C); }
                    else { currentClosestRock = (closestCol + 1, rock.Row, rock.C); }
                    break;
                case Direction.East:
                    if (closestRow < rock.Row) { currentClosestRock = (gridCols, rock.Row, rock.C); }
                    else { currentClosestRock = (closestCol - 1, rock.Row, rock.C); }
                    break;
            }
           
            return currentClosestRock;
        }

        private long CheckVerticalMovement(string[] grid, bool up, List<(int, int, char)> rocks)
        {
            long load = 0;
            for (int col = 0; col < grid[0].Length; col++)
            {
                var currentClosestRock = (col, up ? -1 : grid.Length, ' ');
                for (int row = 0; row < grid.Length; row++)
                {
                    if (grid[row][col] == '#')
                    {
                        currentClosestRock = (col, row, '#');
                        rocks.Add(currentClosestRock);
                    }
                    else if (grid[row][col] == 'O')
                    {
                        var (closestCol, closestRow, _) = currentClosestRock;
                        var newClosestRow = up ? closestRow + 1 : closestRow - 1;
                        currentClosestRock = (closestCol, newClosestRow, 'O');
                        load += grid.Length - newClosestRow;
                        rocks.Add(currentClosestRock);
                    }
                }
            }
            return load;
        }
    }
}