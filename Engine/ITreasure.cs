using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG
{
    public interface ITreasure:ICreature
    {
        int AwardAttack
        {
            get;
            set;
        }
        int AwardDefence
        {
            get;
            set;
        }
        int AwardExp
        {
            get;
            set;
        }
    }
}
