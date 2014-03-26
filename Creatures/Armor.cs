using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Armor : ICreature, ITreasure
    {
        public CreatureType GetCreatureType()
        {
            return CreatureType.Armor;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand()
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.Armor
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
