using AdventOfCode2023.Tasks;

string filename = $"D:\\GeniRepos\\AdventOfCode2023\\Inputs\\1.txt";
var task = new Task5();

task.Solve1(File.ReadAllText(task.Filename));
task.Solve2(File.ReadAllText(task.Filename));