using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    public class StandingMonster : ICreature, IMonster
    {
        public StandingMonster()
        {
            hp = Game.Stage * 7;
            attack = Game.Stage * 2;
            defence = (Game.Stage + 1) / 2;
            expGain = Game.Stage * 10;
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.StandingMonster;
        }
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.StandingMonster
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
