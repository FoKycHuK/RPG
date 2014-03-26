﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG.Creatures
{
    class Boss:ICreature,IMonster
    {
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

        public CreatureType GetCreatureType()
        {
            return CreatureType.Boss;
        }

        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand
            {
                DeltaX = 0,
                DeltaY = 0,
                NextState = CreatureType.Boss
            };
        }
    }
}
