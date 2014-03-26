using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Chest:ICreature, ITreasure
    {

        public CreatureType GetCreatureType()
        {
            return CreatureType.Chest;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand()
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.Chest
            };
        }


        public int AwardAttack
        {
            get;
            set;
        }

        public int AwardDefence
        {
            get;
            set;
        }

        public int AwardExp
        {
            get;
            set;
        }
    }
}
