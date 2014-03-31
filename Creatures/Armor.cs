using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Armor : ICreature, ITreasure
    {
        public Armor()
        {
            AwardDefence = 1;
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.Armor;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(CreatureType.Armor);
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
