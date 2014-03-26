using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;


namespace MyRPG.Creatures
{
    public class Player:ICreature
    {
        CreatureCommand Move(int dx, int dy, int x, int y)
        {
            var res = new CreatureCommand
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.Player
            };
            if (Game.InsideWorld(x + dx, y + dy) &&
                (Game.Map[x + dx, y + dy] == null ||
                Game.Map[x + dx, y + dy].GetCreatureType() != CreatureType.Wall))
            {
                res.DeltaX += dx;
                res.DeltaY += dy;
                return res;
            }
            return res;

        }

        public CreatureCommand Act(int x, int y)
        {
            var res = new CreatureCommand
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.Player
            };
            if (Keyboard.IsKeyDown(Key.Right))
                return Move(1, 0, x, y);
            if (Keyboard.IsKeyDown(Key.Left))
                return Move(-1, 0, x, y);
            if (Keyboard.IsKeyDown(Key.Up))
                return Move(0, -1, x, y);
            if (Keyboard.IsKeyDown(Key.Down))
                return Move(0, 1, x, y);
            return res;
        }

        public int hp;
        public int attack;
        public int defence;
        public int exp;
        public int level;
        public CreatureType GetCreatureType()
        {
            return CreatureType.Player;
        }
    }
}
