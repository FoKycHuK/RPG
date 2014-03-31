using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    public class Monster:ICreature, IMonster
    {
        public Monster()
        {
            hp = Game.Stage * 7;
            attack = Game.Stage * 2;
            defence = (Game.Stage + 1) / 2;
            expGain = Game.Stage * 10;
        }
        CreatureCommand Move(int dx, int dy, int x, int y)
        {
            var res = new CreatureCommand(CreatureType.Player);
            if (!Game.InsideWorld(x + dx, y + dy) ||
                (Game.Map[x + dx, y + dy] != null &&
                Game.Map[x + dx, y + dy].GetCreatureType() != CreatureType.Player))
                return res;
            res.DeltaX += dx;
            res.DeltaY += dy;
            return res;
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.Monster;
        }
        public CreatureCommand Act(int x, int y)
        {
            var res = new CreatureCommand(CreatureType.Monster);
            var way = Game.rand.Next(0, 30);
            switch (way)
            {
                case (0):
                    return Move(1, 0, x, y);
                case (1):
                    return Move(0, 1, x, y);
                case (2):
                    return Move(-1, 0, x, y);
                case (3):
                    return Move(0, -1, x, y);
            }
            return res;
        }
        public int hp
        {
            get;
            set;
        }

        public int attack
        {
            get;
            set;
        }

        public int defence
        {
            get;
            set;
        }


        public int expGain
        {
            get;
            set;
        }
    }
}
