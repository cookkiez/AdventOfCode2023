using System.Net.NetworkInformation;

namespace AdventOfCode2023.Tasks
{
    public class Task8 : AdventTask
    {
        public Task8()
        {
            Filename += "8.txt";
        }

        internal class Node
        {
            public string Value;
            public Node Left;
            public Node Right;
            public Node(string value, Node left, Node right)
            {
                Value = value;
                Left = left;
                Right = right;
            }

            public Node(string value)
            {
                Value = value;
            }
        }

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            var instructions = lines.ElementAt(0).Trim().ToCharArray();
            long result = 0;

            var nodesList = BuildNodes(lines.Skip(2).ToList());

            var currentNode = nodesList.Where(n => n.Value == "AAA").First();

            while (currentNode.Value != "ZZZ")
            {
                foreach (var instruction in instructions)
                {
                    currentNode = ExecuteInstruction(instruction, currentNode);
                    result++;
                }
            }

            Console.WriteLine(result);
        }

        private List<Node> BuildNodes(List<string> lines)
        {
            var nodesList = new List<Node>();
            var linesSplitted = new List<List<string>>();
            foreach (var line in lines)
            {
                var splitted = line.Trim().Split(" = ").ToList();
                var nodeVal = splitted.ElementAt(0);
                linesSplitted.Add(splitted);
                nodesList.Add(new Node(nodeVal));
            }

            foreach (var splittedLine in linesSplitted)
            {
                var node = nodesList.Where(n => n.Value == splittedLine.ElementAt(0)).First();
                var nextNodes = splittedLine.ElementAt(1).Replace("(", "").Replace(")", "").Split(", ");
                node.Right = nodesList.Where(n => n.Value == nextNodes.ElementAt(1)).First();
                node.Left = nodesList.Where(n => n.Value == nextNodes.ElementAt(0)).First();
            }

            return nodesList;
        }

        private Node ExecuteInstruction(char instruction, Node currentNode)
        {
            if (instruction == 'L')
                currentNode = currentNode.Left;
            else
                currentNode = currentNode.Right;
            return currentNode;
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            var instructions = lines.ElementAt(0).Trim().ToCharArray();
            long result = 0;
            var nodesList = BuildNodes(lines.Skip(2).ToList());
            var startNodes = nodesList.Where(n => n.Value.EndsWith('A'));

            var completedNodesFull = new List<Node>();
            var numOfStartNodes = startNodes.Count();
            var lengths = new List<long>();
            long steps = 0;
            while (startNodes.Where(n => n.Value.EndsWith('Z')).Count() != numOfStartNodes)
            {
                foreach (var instructionStart in instructions)
                {
                    var newStartNodes = new List<Node>();
                    foreach (var node in startNodes) { newStartNodes.Add(ExecuteInstruction(instructionStart, node)); }
                    steps++;
                    startNodes = newStartNodes.ToList();

                    var completedNodes = startNodes.Where(n => n.Value.EndsWith('Z'));
                    completedNodesFull.AddRange(completedNodes);
                    if (completedNodes.Any()) { lengths.Add(steps); } 

                    startNodes = startNodes.Where(n => !completedNodesFull.Contains(n));
                }
                if (completedNodesFull.Count == numOfStartNodes)  { break; }
            }
            var lcm = lengths.Aggregate((a, b) => Math.Abs(a * b) / GCD(a, b));
            result = lcm;
            Console.WriteLine(result);
        }

        private long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}