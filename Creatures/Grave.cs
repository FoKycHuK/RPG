using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Grave : ICreature, ITreasure
    {
        public Grave()
        {
            AwardAttack = Game.rand.Next(-2, 3);
            AwardDefence = Game.rand.Next(-1, 2);
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.Grave;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(CreatureType.Grave);
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
