using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG
{
    public class CreatureCommand
    {
        public int DeltaX;
        public int DeltaY;
        public CreatureType NextState;
        public CreatureCommand(CreatureType NextState)
        {
            this.DeltaX = 0;
            this.DeltaY = 0;
            this.NextState = NextState;
        }
    }
}
