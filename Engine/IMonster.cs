using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRPG
{
    public interface IMonster : ICreature
    {
        int hp
        {
            get;
            set;
        }
        int attack
        {
            get;
            set;
        }
        int defence
        {
            get;
            set;
        }
        int expGain
        {
            get;
            set;
        }
    }
}
