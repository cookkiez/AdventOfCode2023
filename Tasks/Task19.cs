namespace AdventOfCode2023.Tasks
{
    public class Task19 : AdventTask
    {
        public Task19()
        {
            Filename += "19.txt";
        }

        private class Part
        {
            public long X { get; set; }
            public long M { get; set; }
            public long A { get; set; }
            public long S { get; set; }
        }


        public override void Solve1(string input)
        {
            var splitted = input.Split("\r\n\r\n").ToList();
            var workflowsSplitted = splitted.ElementAt(0).Split("\r\n").ToList();
            var partsSplitted = splitted.ElementAt(1).Split("\r\n").ToList();
            var workflows = new Dictionary<string, List<Func<Part, string>>>();
            var parts = new List<Part>();
            foreach (var workflow in workflowsSplitted)
            {
                var workflowSplitted = workflow.Trim().Replace("}", "").Split("{");
                var command = workflowSplitted.First();
                var conditions = workflowSplitted.Last().Split(",");
                var operations = new List<Func<Part, string>>();
                foreach (var cond in conditions)
                {
                    var condSplitted = cond.Split(":");
                    var nextWorkflow = condSplitted.Last();
                    var actualCond = condSplitted.First();
                    if (actualCond.Contains(">"))
                    {
                        var actualCondSplitted = actualCond.Split(">");
                        var propToCompare = actualCondSplitted.First();
                        var value = long.Parse(actualCondSplitted.Last());
                        operations.Add(p =>
                            LargerThan((long)p.GetType().GetProperty(propToCompare.ToUpper()).GetValue(p, null), value) ? nextWorkflow : ""
                        );
                    }
                    else if (actualCond.Contains("<"))
                    {
                        var actualCondSplitted = actualCond.Split("<");
                        var propToCompare = actualCondSplitted.First();
                        var value = long.Parse(actualCondSplitted.Last());
                        operations.Add(p =>
                            SmallerThan((long)p.GetType().GetProperty(propToCompare.ToUpper()).GetValue(p, null), value) ? nextWorkflow : ""
                        );
                    }
                    else
                    {
                        operations.Add(p => cond);
                    }
                }
                workflows.Add(command, operations);
            }
            foreach (var part in partsSplitted)
            {
                var partSplitted = part.Replace("{", "").Replace("}", "").Split(",");
                var p = new Part();
                foreach (var val in partSplitted)
                {
                    var valSplitted = val.Split("=");
                    var prop = valSplitted.First();
                    var value = valSplitted.Last();
                    if (prop == "x")
                    {
                        p.X = long.Parse(value);
                    }
                    if (prop == "m")
                    {
                        p.M = long.Parse(value);
                    }
                    if (prop == "a")
                    {
                        p.A = long.Parse(value);
                    }
                    if (prop == "s")
                    {
                        p.S = long.Parse(value);
                    }
                }
                parts.Add(p);
            }
            long result = 0;
            foreach (var part in parts)
            {
                if (IsPartAccepted("in", workflows, part))
                {
                    result += part.X + part.M + part.A + part.S;
                }
            }
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var splitted = input.Split("\r\n\r\n").ToList();
            var workflowsSplitted = splitted.ElementAt(0).Split("\r\n").ToList();
            var workflows = new Dictionary<string, List<(string, Func<List<int>, List<int>>, string)>>();
            foreach (var workflow in workflowsSplitted)
            {
                var workflowSplitted = workflow.Trim().Replace("}", "").Split("{");
                var command = workflowSplitted.First();
                var conditions = workflowSplitted.Last().Split(",");
                var operations = new List<(string, Func<List<int>, List<int>>, string)>();
                foreach (var cond in conditions)
                {
                    var condSplitted = cond.Split(":");
                    var nextWorkflow = condSplitted.Last();
                    var actualCond = condSplitted.First();
                    if (actualCond.Contains(">"))
                    {
                        var actualCondSplitted = actualCond.Split(">");
                        var value = long.Parse(actualCondSplitted.Last());
                        var propToCompare = actualCondSplitted.First();
                        operations.Add((propToCompare, 
                            l => l.Where(x => LargerThan(x, value)).ToList(),
                            nextWorkflow));
                    }
                    else if (actualCond.Contains("<"))
                    {
                        var actualCondSplitted = actualCond.Split("<");
                        var value = long.Parse(actualCondSplitted.Last());
                        var propToCompare = actualCondSplitted.First();
                        operations.Add((propToCompare,
                            l => l.Where(x => SmallerThan(x, value)).ToList(),
                            nextWorkflow));
                    }
                    else
                    {
                        operations.Add(("",
                            l => l,
                            nextWorkflow));
                    }
                }
                workflows.Add(command, operations);
            }
            var ranges = new Dictionary<string, List<int>>();
            foreach(var prop in new List<string> { "x", "s", "m", "a"})
            {
                var l = new List<int>();
                for(int i = 1; i <= 4000; i++)
                {
                    l.Add(i);
                }
                ranges[prop] = l;
            }
            
            Console.WriteLine(CalculateCombinations("in", workflows, ranges));
        }

        private bool IsPartAccepted(string workflowString, Dictionary<string, List<Func<Part, string>>> workflows, Part part)
        {
            if (workflowString == "A") { return true; }
            if (workflowString == "R") { return false; }

            var currentWorkflow = workflows[workflowString];
            foreach (var condition in currentWorkflow)
            {
                var newWorkflowString = condition(part);
                if (newWorkflowString == "") { continue; }
                return IsPartAccepted(newWorkflowString, workflows, part);
            }
            return false;
        }

        private bool SmallerThan(long x1, long x2)
        {
            return x1 < x2;
        }

        private bool LargerThan(long x1, long x2)
        {
            return x1 > x2;
        }

        private long CalculateCombinations(string workflowString, Dictionary<string, List<(string, Func<List<int>, List<int>>, string)>> workflows,
            Dictionary<string, List<int>> ranges)
        {
            if (workflowString == "A") {
                long res = 1;
                foreach(var key in ranges.Keys) { res *= (long)ranges[key].Count(); }
                return res;
            }  
            else if (workflowString == "R") 
            {
                return 0;
            }

            var currentRanges = new Dictionary<string, List<int>>();
            foreach (var key in ranges.Keys)
            {
                currentRanges[key] = ranges[key].ToList();
            }

            long result = 0;
            foreach(var condition in workflows[workflowString])
            {
                var (prop, rangeCondition, nextWorkflow) = condition;
                List<int> toFilter = new List<int>();
                List<int> toAdd = new List<int>();
                if (prop != "")
                {
                    toAdd = currentRanges[prop].ToList();
                    toFilter = rangeCondition(currentRanges[prop]);
                    currentRanges[prop] = toFilter.ToList();
                }
                result += CalculateCombinations(nextWorkflow, workflows, currentRanges);
                if (prop != "")
                    currentRanges[prop] = toAdd.Except(toFilter).ToList(); 
            }
            return result;
        }
    }
}