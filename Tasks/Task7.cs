namespace AdventOfCode2023.Tasks
{
    public class Task7 : AdventTask
    {
        public Task7()
        {
            Filename += "7.txt";
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            var types = new List<(long, int, string)>();
            foreach(var play in lines)
            {
                var playSplitted = play.Split(" ");
                var hand = playSplitted.ElementAt(0);
                // Replace characters so we can easily order in the end. 
                hand = hand.Replace("A", "Z").Replace("K", "Y").Replace("Q", "X").Replace("J", "1").Replace("T", "V");
                var bid = long.Parse(playSplitted.ElementAt(1));

                var type = CalculateDistinctCountedAndGetType(hand);
                types.Add((bid, type, hand));
            }
            var groups = types.GroupBy(t => t.Item2).OrderBy(g => g.Key).ToList();
            int rank = 1;
            foreach (var group in groups)
            {
                var ordered = group.OrderBy(g => g.Item3).ToList();
                foreach(var (bid, _, _) in ordered)
                {
                    result += rank * bid;
                    rank++;
                }
            }
            Console.WriteLine(result);
        }

        private int GetType(List<char> distinct, List<int> counted)
        {
            int type = 1;
            if (distinct.Count == 5) { type = 1; }
            else if (distinct.Count == 4) { type = 2; }
            else if (distinct.Count == 3)
            {
                var twoPair = counted.Where(c => c == 2);
                var threePair = counted.Where(c => c == 3);
                if (twoPair.Count() == 2)
                    type = 3;
                else if (threePair.Count() == 1)
                    type = 4;
            }
            else if (distinct.Count == 2)
            {
                var fourOfKind = counted.Any(c => c == 1);
                if (fourOfKind)
                    type = 6;
                else
                    type = 5;
            }
            else
                type = 7;
            return type;
        }

        private int CalculateDistinctCountedAndGetType(string hand)
        {
            var distinct = hand.Distinct().ToList();
            var counted = hand.GroupBy(c => c).Select(c => c.Count()).ToList();
            return GetType(distinct, counted);
        }

        private (int, string) TryJoker(List<char> distinct, string hand)
        {
            var bestTamperedHand = hand;
            var maxType = 0;
            foreach (var d in distinct)
            {
                var tamperedHand = hand.Replace('1', d);
                var tamperedType = CalculateDistinctCountedAndGetType(tamperedHand);
                if (tamperedType > maxType)
                {
                    bestTamperedHand = tamperedHand;
                    maxType = tamperedType;
                }
            }
            return (maxType, bestTamperedHand);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            long result = 0;
            var types = new List<(long, int, string)>();

            // Calculate type of hand for all hands
            foreach (var play in lines)
            {
                var playSplitted = play.Split(" ");
                var hand = playSplitted.ElementAt(0);
                // Replace characters so we can easily order in the end. 
                hand = hand.Replace("A", "Z").Replace("K", "Y").Replace("Q", "X").Replace("J", "1").Replace("T", "V");
                var bid = long.Parse(playSplitted.ElementAt(1));

                var bestTamperedHand = hand;
                var distinct = hand.Distinct().ToList();
                var maxType= 1;
                if (hand.Contains("1"))
                    (maxType, bestTamperedHand) = TryJoker(distinct, hand);

                var type = CalculateDistinctCountedAndGetType(bestTamperedHand);
                if (type > maxType)
                    maxType = type;
                types.Add((bid, maxType, hand));
            }

            // Calculate rank of each hand and calculate result
            var groups = types.GroupBy(t => t.Item2).OrderBy(g => g.Key).ToList();
            int rank = 1;
            foreach (var group in groups)
            {
                var ordered = group.OrderBy(g => g.Item3).ToList();
                foreach (var (bid, _, _) in ordered)
                {
                    result += rank * bid;
                    rank++;
                }
            }
            Console.WriteLine(result);
        }
    }
}