using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class PowerfulMonster: ICreature, IMonster
    {
        public PowerfulMonster()
        {
            hp = Game.Stage * 12;
            attack = Game.Stage * 2 + 1;
            defence = (Game.Stage + 2) / 2;
            expGain = Game.Stage * 20;
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.PowerfulMonster;
        }
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.PowerfulMonster
            };
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
