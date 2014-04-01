using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyRPG
{
    public class RandomMap
    {
        static bool OneOfFour(bool a, bool b, bool c, bool d)
        {
            return (a ? 1 : 0) + (b ? 1 : 0) + (c ? 1 : 0) + (d ? 1 : 0) == 1;
        }
        static Random rand = new Random();
        static bool[,] gameMap;
        static Point[] defPoints = new Point[4] 
        {
            new Point(0, 1), 
            new Point(0, -1), 
            new Point(1, 0), 
            new Point(-1, 0) 
        };
        static bool InsideWorld(Point point)
        {
            return point.X >= 0 && point.Y >= 0 && point.X < gameMap.GetLength(0) && point.Y < gameMap.GetLength(1);
        }
        static Point GetN(Point from)
        {
            var array = defPoints.Select(z => new Point(z.X + from.X, z.Y + from.Y))
                .Where(z => InsideWorld(z) &&
                    !gameMap[z.X, z.Y] &&
                    InsideWorld(new Point(z.X + 1, z.Y)) &&
                    InsideWorld(new Point(z.X - 1, z.Y)) &&
                    InsideWorld(new Point(z.X, z.Y - 1)) &&
                    InsideWorld(new Point(z.X, z.Y + 1)) &&
                    OneOfFour(gameMap[z.X + 1, z.Y], gameMap[z.X - 1, z.Y], gameMap[z.X, z.Y + 1], gameMap[z.X, z.Y - 1]) &&
                    rand.Next(0, 1000) < 10000)
                .ToArray();
            var x = rand.Next(0, array.Length);
            if (array.Length != 0)
                return array[x];
            else
                return from;

        }
        static public void GetMap()
        {
            gameMap = new bool[Game.MapWidth, Game.MapHeight];
            int x = rand.Next(0, Game.MapWidth);
            int y = rand.Next(0, Game.MapHeight);
            var stack = new Stack<Point>();
            stack.Push(new Point(x, y));
            gameMap[x, y] = true;
            while (stack.Count != 0)
            {
                var point = GetN(stack.Peek());
                if (point == stack.Peek())
                    stack.Pop();
                else
                {
                    gameMap[point.X, point.Y] = true;
                    stack.Push(point);
                }
            }
            for (var i = 0; i < Game.MapWidth; i++)
                for (var j = 0; j < Game.MapHeight; j++)
                    if (!gameMap[i, j])
                        Game.Map[i, j] = new Creatures.Wall();
        }
    }
}
