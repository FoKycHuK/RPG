using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyRPG
{

    public partial class Fight : Form
    {
        Timer timer;
        const int ElementSize = 32;
        Bitmap monsterIcon;
        Bitmap playerIcon = (Bitmap)Bitmap.FromFile("Images\\player.png");
        Creatures.Player player;
        IMonster monster;

        public Fight(Creatures.Player player, IMonster monster)
        {
            Game.FightOver += FightOver;
            StartPosition = FormStartPosition.CenterScreen;
            ControlBox = false;
            this.player = player;
            this.monster = monster;
            monsterIcon = (Bitmap)Bitmap.FromFile("Images\\" + monster.GetCreatureType().ToString() + ".png");
            ClientSize = new Size(ElementSize * 5, ElementSize * 2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Text = "FIGHT";
            DoubleBuffered = true;
            timer = new Timer();
            timer.Interval = 600;
            timer.Tick += TimerTick;
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, 0);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, ClientSize.Width, ClientSize.Height);
            e.Graphics.DrawImage(playerIcon, 0, 0);
            e.Graphics.DrawImage(monsterIcon, ClientSize.Width - ElementSize, 0);
            e.Graphics.ResetTransform();
            e.Graphics.DrawString("HP: " + player.hp.ToString(), new Font("Arial", 16), Brushes.Red, 0, ElementSize);
            e.Graphics.DrawString("HP: " + monster.hp.ToString(), new Font("Arial", 16), Brushes.Red, ClientSize.Width - ElementSize * 2 - 10 , ElementSize);
            e.Graphics.DrawString("VS", new Font("Arial", 16), Brushes.White, ElementSize * 2, 0);

        }
        void TimerTick(object sender, EventArgs args)
        {
            Game.FightAct(monster);
            Invalidate();
        }
        void FightOver()
        {
            timer.Stop();
            this.Close();
        }
    }
}
