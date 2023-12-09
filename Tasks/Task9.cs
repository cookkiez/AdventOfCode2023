using System.Net.NetworkInformation;

namespace AdventOfCode2023.Tasks
{
    public class Task9 : AdventTask
    {
        public Task9()
        {
            Filename += "9.txt";
        }

        private List<List<long>> GetSequences (string line)
        {
            var history = line.Trim().Split(" ").Select(long.Parse).ToList();
            var sequences = new List<List<long>>() { history };
            var sequence = history;
            while (!sequence.All(x => x == 0))
            {
                var nextSequence = new List<long>();
                for (int i = 1; i < sequence.Count(); i++)
                    nextSequence.Add(sequence[i] - sequence[i - 1]);
                sequences.Add(nextSequence);
                sequence = nextSequence.ToList();
            }
            sequences.Reverse();
            return sequences;
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            
            foreach (var line in lines) 
            { 
                var sequences = GetSequences(line);
                // Sadly can't use seq.LastOrDefault / FirstOrDefault as a delegate, so this can't be moved to a function.
                long prevValue = 0;
                foreach (var seq in sequences.Skip(1))
                    prevValue = seq.LastOrDefault() + prevValue;
                
                result += prevValue;
            }
            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;

            foreach (var line in lines)
            {
                var sequences = GetSequences(line);

                long prevValue = 0;
                foreach (var seq in sequences.Skip(1))
                    prevValue = seq.FirstOrDefault() - prevValue;
                
                result += prevValue;
            }
            Console.WriteLine(result);
        }
    }
}