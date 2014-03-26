using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyRPG
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Game.Begin();
            Application.Run(new MyWindow());
            //картинки не тормозят ?
        }
    }
}
