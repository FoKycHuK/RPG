using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyRPG
{
    public class MyWindow : Form
    {
        Creatures.Player player;
        Timer timer;
        const int ElementSize = 32;
        Dictionary<CreatureType, Bitmap> bitmaps = new Dictionary<CreatureType, Bitmap>();
        List<CreatureAnimation> animations = new List<CreatureAnimation>();
        int tickCount;
        int restorer;

        public MyWindow()
        {
            Game.Print += PrintSomething;
            Game.ChooseMode += ChooseMode;
            Game.StageChanged += ChangeStage;
            Game.Level += LevelUp;
            Game.Grave += InGrave;
            Game.Begin();
            player = Game.player;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(ElementSize * Game.MapWidth, ElementSize * Game.MapHeight + ElementSize);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Text = "RPG";
            DoubleBuffered = true;
            foreach (var e in Enum.GetValues(typeof(CreatureType)))
                if ((CreatureType)e != CreatureType.Wall)
                    bitmaps[(CreatureType)e] = (Bitmap)Bitmap.FromFile("Images\\" + e.ToString() + ".png");
            timer = new Timer();
            timer.Interval = 5;
            timer.Tick += TimerTick;
            timer.Start();
        }

        void Act()
        {
            animations.Clear();
            for (int x = 0; x < Game.MapWidth; x++)
                for (int y = 0; y < Game.MapHeight; y++)
                {
                    var creature = Game.Map[x, y];
                    if (creature == null) continue;
                    var command = creature.Act(x, y);
                    animations.Add(new CreatureAnimation
                    {
                        Command = command,
                        Creature = creature,
                        Location = new Point(x * ElementSize, y * ElementSize)
                    });
                }
            animations = animations.OrderByDescending(z => (int)z.Creature.GetCreatureType()).ToList();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Gray, 0, 0, ElementSize * Game.MapWidth, ElementSize);
            e.Graphics.TranslateTransform(0, ElementSize);
            foreach (var a in animations)
                e.Graphics.DrawImage(bitmaps[a.Creature.GetCreatureType()], a.Location);
            e.Graphics.ResetTransform();
            if (player != null)
                e.Graphics.DrawString(String.Format("Level:{0}  Exp:{1}/{2}  HP:{3}  Attack:{4}  Defence:{5}  Stage:{6}#{7}",
                    player.level, 
                    player.exp, 
                    player.level * 100, 
                    player.hp,
                    player.attack,
                    player.defence,
                    Game.StageName,
                    Game.Stage/8+1), 
                new Font("Arial", 16), Brushes.White, 0, 0);
        }

        void TimerTick(object sender, EventArgs args)
        {
            if (tickCount == 0) Act();
            foreach (var e in animations)
                e.Location = new Point(e.Location.X + 4 * e.Command.DeltaX, e.Location.Y + 4 * e.Command.DeltaY);
            if (tickCount == 7)
            {
                for (int x = 0; x < Game.MapWidth; x++) for (int y = 0; y < Game.MapHeight; y++) Game.Map[x, y] = null;
                foreach (var e in animations)
                {
                    var x = e.Location.X / ElementSize;
                    var y = e.Location.Y / ElementSize;
                    var nextCreature = e.Creature;
                    if (Game.Map[x, y] == null)
                        Game.Map[x, y] = nextCreature;
                    else
                    {
                        timer.Stop();
                        Game.Conflict(nextCreature, x, y);
                        timer.Start();
                    }
                }
            }
            tickCount++;
            if (tickCount == 8)
            {
                tickCount = 0;
                restorer++;
                if (restorer == 50)
                {
                    if (player.hp < player.level * 10)
                        player.hp+= player.level;
                    restorer = 0;
                }
            }
            Invalidate();
        }
        void ChangeStage()
        {
            Game.Stage++;
            Game.StageDefination();
            BackgroundImage = (Bitmap)Bitmap.FromFile("Images\\" + Game.StageName + ".jpg");
            bitmaps[CreatureType.Wall] = (Bitmap)Bitmap.FromFile("Images\\Walls\\Wall-" + Game.StageName + ".png");
        }
        void PrintSomething(string s)
        {
            MessageBox.Show(s);
        }
        bool InGrave()
        {
            var answer = true;
            var form = new Form()
            {
                ControlBox = false,
                Size = new Size(350, 150),
                Text = "Grave",
                StartPosition = FormStartPosition.CenterScreen
            };
            var lab = new Label()
            {
                Text = "You find grave. Your choise:",
                Top = 10,
                Left = form.Size.Width / 4,
                Size = new Size(300, 25)
            };
            var buttonAttack = new Button()
            {
                Text = "Go inside!",
                Left = 15,
                Top = 50
            };
            buttonAttack.Click += (sender, e) => form.Close();
            var buttonDefence = new Button()
            {
                Text = "Leave",
                Left = form.Size.Width - buttonAttack.Width - 50,
                Top = 50
            };
            buttonDefence.Click += (sender, e) =>
            {
                form.Close();
                answer = false;
            };
            form.Controls.Add(lab);
            form.Controls.Add(buttonAttack);
            form.Controls.Add(buttonDefence);
            form.ShowDialog();
            return answer;
        }
        void LevelUp()
        {
            var form = new Form()
            {
                ControlBox = false,
                Size = new Size(350, 150),
                Text = "Level UP",
                StartPosition = FormStartPosition.CenterScreen
            };
            var lab = new Label()
            {
                Text = "Level up! Choose your boost:",
                Top = 10,
                Left = form.Size.Width / 4,
                Size = new Size(300, 25)
            };
            var buttonAttack = new Button()
            {
                Text = "Attack",
                Left = 15,
                Top = 50
            };
            buttonAttack.Click += (sender, e) =>
            {
                player.attack += 2;
                form.Close();
            };

            var buttonDefence = new Button()
            {
                Text = "Defence",
                Left = form.Size.Width - buttonAttack.Width - 50,
                Top = 50
            };
            buttonDefence.Click += (sender, e) =>
            {
                player.defence += 1;
                form.Close();
            };
            form.Controls.Add(lab);
            form.ShowDialog();
        }
        int ChooseMode()
        {
            var form = new Form()
            {
                ControlBox = false,
                Size = new Size(350, 150),
                Text = "CHOOSE YOUR DESTENY!",
                StartPosition = FormStartPosition.CenterScreen
            };
            var ans = 0;
            var lab = new Label()
            {
                Text = "This is RPG. You can choose one of game modes:",
                Top = 10,
                Left = form.Size.Width / 10,
                Size = new Size(300, 25)
            };
            var button1 = new Button()
            {
                Text = "Adventure",
                Left = 15,
                Top = 50
            };
            button1.Click += (sender, e) =>
                {
                    ans = 0;
                    form.Close();
                };
            var button2 = new Button()
            {
                Text = "Labyrint",
                Left = form.Size.Width - button1.Width * 2 - 70,
                Top = 50
            };
            button2.Click += (sender, e) =>
            {
                ans = 1;
                form.Close();
            };
            var button3 = new Button()
            {
                Text = "Survival",
                Left = form.Size.Width - button1.Width - 30,
                Top = 50
            };
            button3.Click += (sender, e) =>
            {
                ans = 2;
                form.Close();
            };
            form.Controls.Add(lab);
            form.Controls.Add(button1);
            form.Controls.Add(button2);
            form.Controls.Add(button3);
            form.ShowDialog();
            return ans;
        }
    }
}
