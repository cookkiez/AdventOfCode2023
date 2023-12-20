using System.Data;

namespace AdventOfCode2023.Tasks
{
    public class Task20 : AdventTask
    {
        public Task20()
        {
            Filename += "20.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            var (modules, _, _) = GetModulesAndInputsOutputsAndRxPredecessor(lines);
            var lowPulses = 0;
            var highPulses = 0;
            var cycleLength = 0;
            for (int i = 1; i <= 1000; i++)
            {
                lowPulses++;
                var moduleQueue = new Queue<(string, bool)>();
                moduleQueue.Enqueue(("broadcaster", false));
                while (moduleQueue.TryDequeue(out var currentState))
                {
                    var (currentModuleKey, pulse) = currentState;
                    if (!modules.ContainsKey(currentModuleKey)) { continue; }
                    var (_, outputs) = modules[currentModuleKey];
                    (lowPulses, highPulses) = EnqueueNextModules(outputs, highPulses, lowPulses, pulse, currentModuleKey, modules, moduleQueue);
                }
                var looped = true;
                foreach(var (_, (module, _)) in modules)
                {
                    if (module.On) { 
                        looped = false; break; }
                }
                if (looped) { cycleLength = i;  break; }
            }
            var toMultiply = (cycleLength == 0) ? 1 : (1000 / cycleLength);
            lowPulses *= toMultiply;
            highPulses *= toMultiply;
            long result = lowPulses * highPulses;
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            var (modules, inputsOutputs, rxPredecesor) = GetModulesAndInputsOutputsAndRxPredecessor(lines);
            foreach (var (m1, m2) in inputsOutputs)
            {
                if (!modules.ContainsKey(m2)) { continue; }
                var (module, _) = modules[m2];
                if (module.Conjuction) { module.RememberedPulses[m1] = false; }
            }

            // The rx predecesor is a conjunction and we need to get the cycles of its predecesors.
            var mKeys = modules[rxPredecesor].Item1.RememberedPulses.Keys.ToList();
            var modulesToLookFor = new Dictionary<string, long>();

            for (long i = 1; i <= 10000000000; i++)
            {
                var moduleQueue = new Queue<(string, bool)>();
                moduleQueue.Enqueue(("broadcaster", false));
                while (moduleQueue.TryDequeue(out var currentState))
                {
                    var (currentModuleKey, pulse) = currentState;
                    if (!modules.ContainsKey(currentModuleKey)) { continue; }
                    var (_, outputs) = modules[currentModuleKey];
                    // Store the cycles of rx conjunction predecesors, then compute the LCM
                    if (mKeys.Contains(currentModuleKey) && !modulesToLookFor.ContainsKey(currentModuleKey) && pulse)
                    {
                        modulesToLookFor[currentModuleKey] = i;
                    }
                    var (_, _) = EnqueueNextModules(outputs, 0, 0, pulse, currentModuleKey, modules, moduleQueue);
                }
                if (modulesToLookFor.Keys.Count == mKeys.Count) {  break; }
            }
            var lcm = modulesToLookFor.Values.Aggregate((a, b) => Math.Abs(a * b) / GCD(a, b));
            Console.WriteLine(lcm);
        }

        private (int, int) EnqueueNextModules(List<string> outputs, int highPulses, int lowPulses, bool pulse, string currentModuleKey,
            Dictionary<string, (Module, List<string>)> modules, Queue<(string, bool)> moduleQueue)
        {
            foreach (var output in outputs)
            {
                if (pulse) { highPulses++; }
                else { lowPulses++; }
                if (!modules.ContainsKey(output)) { continue; }
                var (outputModule, _) = modules[output];
                var (nextPulse, shouldSend) = outputModule.GotPulse(pulse, currentModuleKey);
                if (shouldSend) { moduleQueue.Enqueue((output, nextPulse)); }
            }
            return (lowPulses, highPulses);
        }

        private (Dictionary<string, (Module, List<string>)>, List<(string, string)>, string) GetModulesAndInputsOutputsAndRxPredecessor(List<string> lines)
        {
            var modules = new Dictionary<string, (Module, List<string>)>();
            var inputsOutputs = new List<(string, string)>();
            var rxPredecesor = "";
            foreach (var line in lines)
            {
                var module = new Module();
                var lineSplitted = line.Split(" -> ");
                var moduleName = lineSplitted[0];
                var nextModules = lineSplitted[1].Split(", ").ToList();
                if (moduleName == "broadcaster") { module.Broadcast = true; }
                if (moduleName.Contains("%")) { module.FlipFlop = true; moduleName = moduleName.Replace("%", ""); }
                if (moduleName.Contains("&")) { module.Conjuction = true; moduleName = moduleName.Replace("&", ""); }
                foreach (var m in nextModules) { inputsOutputs.Add((moduleName, m)); }
                if (nextModules.Contains("rx")) { rxPredecesor = moduleName; }
                modules.Add(moduleName, (module, nextModules));
            }

            foreach (var (m1, m2) in inputsOutputs)
            {
                if (!modules.ContainsKey(m2)) { continue; }
                var (module, _) = modules[m2];
                if (module.Conjuction) { module.RememberedPulses[m1] = false; }
            }
            return (modules, inputsOutputs, rxPredecesor);
        }

        private long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        private class Module
        {
            // False pulse means low pulse
            public bool Pulse = false;
            public bool FlipFlop = false;
            public bool Conjuction = false;
            public bool Broadcast = false;
            public bool On = false;
            public Dictionary<string, bool> RememberedPulses = new Dictionary<string, bool>();

            public (bool, bool) GotPulse(bool pulse, string input)
            {
                // returns next pulse and if need to send it forward
                if (Broadcast) { return (false, true);  }
                else if(FlipFlop)
                {
                    if (!pulse)
                    {
                        On = !On; 
                        if (On) { return (true, true); }
                        else { return (false, true); }
                    } else { return (pulse, false); }
                } else
                {
                    RememberedPulses[input] = pulse;
                    if (RememberedPulses.Values.Where(p => p).Count() == RememberedPulses.Values.Count)
                    {
                        return (false, true);
                    } else { return (true, true); }
                }
            }
        }
    }
}