using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class HealingPotion:ICreature
    {
        public CreatureType GetCreatureType()
        {
            return CreatureType.HealingPotion;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(CreatureType.HealingPotion);
        }
    }
}
