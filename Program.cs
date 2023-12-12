using AdventOfCode2023.Tasks;

var task = new Task12();
var task1Watch = new System.Diagnostics.Stopwatch();
var task2Watch = new System.Diagnostics.Stopwatch();

Console.WriteLine("Solving First Task:");
task1Watch.Start();
task.Solve1(File.ReadAllText(task.Filename));
task1Watch.Stop();
Console.WriteLine($"Execution Time: {task1Watch.ElapsedMilliseconds} ms");
Console.WriteLine();
Console.WriteLine($"-----------------------------");
Console.WriteLine();

Console.WriteLine("Solving Second Task:");
task2Watch.Start();
task.Solve2(File.ReadAllText(task.Filename));
task2Watch.Stop();
Console.WriteLine($"Execution Time: {task2Watch.ElapsedMilliseconds} ms");