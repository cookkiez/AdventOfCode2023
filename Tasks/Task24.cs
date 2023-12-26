using Microsoft.Z3;

namespace AdventOfCode2023.Tasks
{
    public class Task24 : AdventTask
    {
        public Task24()
        {
            Filename += "24.txt";
        }

        private record Velocity(double dx, double dy, double dz, double x, double y, double z);

        public override void Solve1(string input)
        {
            var lines = GetLinesList(input);
            var vels = new List<Velocity>();
            foreach(var line in lines) 
            {
                var lineSplitted = line.Split(" @ ");
                var ps = lineSplitted[0].Split(", ").Select(double.Parse).ToArray();
                var vs = lineSplitted[1].Split(", ").Select(double.Parse).ToArray();
                vels.Add(new Velocity(vs[0], vs[1], vs[2], ps[0], ps[1], ps[2]));
            }

            var done = new HashSet<(Velocity, Velocity)>();
            var testMin = 200000000000000;
            var testMax = 400000000000000;
            long result = 0;

            foreach(var v in vels)
            {
                var (a1, b1, c1) = GetScalars(v);
                foreach (var vv in vels)
                {
                    if (v == vv || done.Contains((v, vv)) || done.Contains((vv, v))) { continue; }
                    done.Add((v, vv));
                    var (a2, b2, c2) = GetScalars(vv);

                    var det = a1 * b2 - a2 * b1;
                    if (det == 0) { continue; }

                    var x = (b2 * c1 - b1 * c2) / det;
                    var y = (a1 * c2 - a2 * c1) / det;

                    if (x <= testMax && x >= testMin && y <= testMax && y >= testMin) { 
                        if (IsInHistory(v, x, y) || IsInHistory(vv, x, y)) { 
                            continue; }
                        result++; }
                }
            }

            Console.WriteLine(result);
        }

        public override void Solve2(string input)
        {
            var lines = GetLinesList(input);
            var vels = new List<Velocity>();
            foreach (var line in lines)
            {
                var lineSplitted = line.Split(" @ ");
                var ps = lineSplitted[0].Split(", ").Select(double.Parse).ToArray();
                var vs = lineSplitted[1].Split(", ").Select(double.Parse).ToArray();
                vels.Add(new Velocity(vs[0], vs[1], vs[2], ps[0], ps[1], ps[2]));
            }

            // Stole this code
            // No clue why it returns exactly the correct position
            // I understand the equations and why it works
            // But no idea why it gets the correct position.
            var ctx = new Context();
            var solver = ctx.MkSolver();

            // Coordinates of the stone
            var x = ctx.MkIntConst("x");
            var y = ctx.MkIntConst("y");
            var z = ctx.MkIntConst("z");

            // Velocity of the stone
            var vx = ctx.MkIntConst("vx");
            var vy = ctx.MkIntConst("vy");
            var vz = ctx.MkIntConst("vz");


            for(var i = 0; i < Math.Min(5, vels.Count); i++)
            {
                var t = ctx.MkIntConst($"t{i}");

                var hail = vels[i];

                var px = ctx.MkInt(Convert.ToInt64(hail.x));
                var py = ctx.MkInt(Convert.ToInt64(hail.y));
                var pz = ctx.MkInt(Convert.ToInt64(hail.z));

                var pvx = ctx.MkInt(Convert.ToInt64(hail.dx));
                var pvy = ctx.MkInt(Convert.ToInt64(hail.dy));
                var pvz = ctx.MkInt(Convert.ToInt64(hail.dz));

                var xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x + t * vx
                var yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y + t * vy
                var zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z + t * vz

                var xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
                var yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
                var zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

                solver.Add(t >= 0); // time should always be positive - we don't want solutions for negative time
                solver.Add(ctx.MkEq(xLeft, xRight)); // x + t * vx = px + t * pvx
                solver.Add(ctx.MkEq(yLeft, yRight)); // y + t * vy = py + t * pvy
                solver.Add(ctx.MkEq(zLeft, zRight)); // z + t * vz = pz + t * pvz
            }

            solver.Check();
            var model = solver.Model;

            var rx = model.Eval(x);
            var ry = model.Eval(y);
            var rz = model.Eval(z);

            Console.WriteLine(Convert.ToInt64(rx.ToString()) + Convert.ToInt64(ry.ToString()) + Convert.ToInt64(rz.ToString()));

            //Console.WriteLine(result);
        }

        private bool IsInHistory(Velocity v, double x, double y)
        {
            var vy2 = v.y + v.dy;
            var vx2 = v.x + v.dx;
            return Math.Abs(x - vx2) > Math.Abs(x - v.x) || Math.Abs(y - vy2) > Math.Abs(y - v.y);
        }

        private (double, double, double) GetScalars(Velocity v)
        {
            var a1 = -v.dy;
            var b1 = v.dx;
            var c1 = a1 * v.x + b1 * v.y;
            return (a1, b1, c1);
        }
    }
}