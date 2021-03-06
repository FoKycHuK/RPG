﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Sword : ICreature, ITreasure
    {
        public Sword()
        {
            AwardAttack = 2;
        }
        public CreatureType GetCreatureType()
        {
            return CreatureType.Sword;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand(CreatureType.Sword);
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
