using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Chest:ICreature, ITreasure
    {
        public Chest()
        {
            AwardAttack = Game.rand.Next(0, 2);
            AwardExp = Game.rand.Next(Game.Stage * 50, Game.Stage * 100);
        }
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
