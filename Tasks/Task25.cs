namespace AdventOfCode2023.Tasks
{
    public class Task25 : AdventTask
    {
        public Task25()
        {
            Filename += "25.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input).Select(l => l.Trim()).ToList();
            var components = new Dictionary<string, List<string>>();

            foreach ( var line in lines )
            {
                var lineSplitted = line.Split(": ");
                var component = lineSplitted[0];
                var connections = lineSplitted[1].Split(" ").ToList();
                if (components.ContainsKey(component)) { components[component].AddRange(connections); }
                else { components.Add(component, connections); }
                foreach(var c in connections)
                {
                    if (components.ContainsKey(c)) { components[c].Add(component); }
                    else { components.Add(c, new List<string> { component }); }
                }
            }

            var flows = new Dictionary<string, long>();

            foreach (var component in components.Keys)
            {
                foreach(var conn in components[component])
                {
                    var edge = string.Join("-", new List<string> { conn, component}.Order());
                    flows[edge] = 0;
                }
                
            }

            for (int i = 0; i < 100; i++)
            //foreach(var component in components.Keys)
            {
                var ind = new Random().Next(components.Keys.Count);
                var ind2 = new Random().Next(components.Keys.Count);
                if (ind == ind2) { continue; }
                var component = components.Keys.ElementAt(ind2);
                var goal = components.Keys.ElementAt(ind);
                var queue = new Queue<(string, List<string>)>();
                var visited = new HashSet<string>();
                queue.Enqueue((component, new List<string>() { component }));
                while (queue.TryDequeue(out var currState))
                {
                    var (currComponent, path) = currState;
                    if (currComponent == goal)
                    {
                        var pathArray = path.ToArray();
                        for (int c = 0; c < pathArray.Length - 1; c++)
                        {
                            var edge = string.Join("-", new List<string> { pathArray[c], pathArray[c + 1] }.Order());
                            flows[edge]++;
                        }
                        break;
                    }
                    foreach (var conn in components[currComponent])
                    {
                        var tempPath = path.ToList();
                        if (visited.Contains(conn)) { continue; }
                        tempPath.Add(conn);
                        queue.Enqueue((conn, tempPath));
                        visited.Add(conn);
                    }
                }
            }
            var orderedFlows = flows.OrderByDescending(k => k.Value).ToList();
            var edgesToCut = orderedFlows.Select(k => k.Key).Take(3).ToList();

            foreach(var edge in edgesToCut)
            {
                var edgeSplitted = edge.Split("-").ToArray();
                var c1 = edgeSplitted[0];
                var c2 = edgeSplitted[1];
                components[c1].Remove(c2);
                components[c2].Remove(c1);
            }

            var sizes = new Dictionary<string, long>();
            foreach (var component in components.Keys)
            {
                var queue = new Queue<string>();
                var visited = new HashSet<string>();
                queue.Enqueue(component);
                while (queue.TryDequeue(out var currComponent))
                {
                    foreach (var conn in components[currComponent])
                    {
                        if (visited.Contains(conn)) { continue; }
                        queue.Enqueue(conn);
                        visited.Add(conn);
                    }
                }
                sizes[component] = visited.Count;
            }
            var vals = sizes.Select(s => s.Value).Distinct();
            var result = vals.ElementAt(0) * vals.ElementAt(1);
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            
            //Console.WriteLine(result);
        }
    }
}