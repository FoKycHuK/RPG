using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Wall : ICreature
    {
        public CreatureType GetCreatureType()
        {
            return CreatureType.Wall;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(CreatureType.Wall);
        }
    }
}
