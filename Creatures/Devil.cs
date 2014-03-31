using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Devil : ICreature, IMonster
    {
        public Devil()
        {
            hp = 100;
            attack = 40;
            defence = 5;
            expGain = 9999;
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.Devil;
        }
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(CreatureType.Devil);
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
