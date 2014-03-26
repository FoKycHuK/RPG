using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG
{
    public interface ICreature
    {
        CreatureType GetCreatureType();
        CreatureCommand Act(int x, int y);
    }
}
