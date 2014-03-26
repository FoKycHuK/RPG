using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyRPG
{

    public class CreatureAnimation
    {
        public ICreature Creature;
        public CreatureCommand Command;
        public Point Location;
    }

    public class MyWindow : Form
    {
        Creatures.Player player;
        Timer timer;
        const int ElementSize = 32;
        Dictionary<CreatureType, Bitmap> bitmaps = new Dictionary<CreatureType, Bitmap>();
        static List<CreatureAnimation> animations = new List<CreatureAnimation>();


        public MyWindow()
        {
            Game.StageChanged += ChangeBackground;
            BackgroundImage = (Bitmap)Bitmap.FromFile("Images\\" + Game.StageName + ".jpg");
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(ElementSize * Game.MapWidth, ElementSize * Game.MapHeight + ElementSize);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Text = "RPG";
            DoubleBuffered = true;
            foreach (var e in Enum.GetValues(typeof(CreatureType)))
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
                    if (creature.GetCreatureType() == CreatureType.Player)
                        player = (Creatures.Player)creature;
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
        void ChangeBackground()
        {
            this.BackgroundImage = (Bitmap)Bitmap.FromFile("Images\\" + Game.StageName + ".jpg");
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.DrawImage((Bitmap)Bitmap.FromFile("Images\\" + Game.StageName + ".jpg"), 0, 0);
            e.Graphics.FillRectangle(Brushes.Black, 0, 0, ElementSize * Game.MapWidth, ElementSize);
            e.Graphics.TranslateTransform(0, ElementSize);
            foreach (var a in animations)
                e.Graphics.DrawImage(bitmaps[a.Creature.GetCreatureType()], a.Location);
            e.Graphics.ResetTransform();
            if (player != null)
                e.Graphics.DrawString(String.Format("Level:{0}  Exp:{1}/{2}  HP:{3}  Attack:{4}  Defence:{5}  Stage:{6}",
                    player.level, 
                    player.exp, 
                    player.level * 100, 
                    player.hp,
                    player.attack,
                    player.defence,
                    Game.StageName), 
                new Font("Arial", 16), Brushes.Green, 0, 0);
        }

        int tickCount = 0;

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
                    var x = e.Location.X / 32;
                    var y = e.Location.Y / 32;
                    var nextCreature = e.Creature;
                    if (Game.Map[x, y] == null)
                        Game.Map[x, y] = nextCreature;
                    else
                    {
                        timer.Stop();
                        if (Game.Map[x, y].GetCreatureType() == CreatureType.HealingPotion)
                        {
                            player.hp = player.level * 10;
                            Game.Map[x, y] = player;
                        }
                        if (Game.Map[x, y] is ITreasure)
                        {
                            var aw = (ITreasure)Game.Map[x, y];
                            Game.Map[x, y] = player;
                            player.attack += aw.AwardAttack;
                            player.exp += aw.AwardExp;
                            player.defence += aw.AwardDefence;
                            switch (aw.GetCreatureType())
                            {
                                case CreatureType.Armor:
                                    MessageBox.Show("You find armor, better, then you have. You're defence increased on 1!");
                                    break;
                                case CreatureType.Sword:
                                    MessageBox.Show("You find sword, better, then you have. You're attack increased on 2!");
                                    break;
                                case CreatureType.Grave:
                                    if (Game.InGrave())
                                    {
                                        MessageBox.Show(string.Format("You enter in the grave, and find curse/blessing! Gained: {0} Attack, {1} Defence.", aw.AwardAttack, aw.AwardDefence));
                                        if (Game.rand.Next(0, 1000) > 800)
                                        {
                                            MessageBox.Show("And you're under attack!!!");
                                            Game.Fight(player, new Creatures.Monster() { 
                                                hp = Game.Stage * 14, 
                                                attack = Game.Stage * 3, 
                                                defence = (Game.Stage + 1) / 2, 
                                                expGain = Game.Stage * 20 
                                            });
                                        }
                                    }
                                    else
                                    {
                                        player.attack -= aw.AwardAttack;
                                        player.defence -= aw.AwardDefence;
                                        var gainedExp = Game.rand.Next(0, Game.Stage * 50);
                                        player.exp += gainedExp;
                                        MessageBox.Show(string.Format("You running away, and gain some exp: {0}", gainedExp));
                                    }
                                    break;
                                default:
                                    MessageBox.Show(string.Format("You find some treasures! Gained: {0} Exp, {1} Attack, {2} Defence.", aw.AwardExp, aw.AwardAttack, aw.AwardDefence));
                                    break;
                            }
                        }
                        if (nextCreature is IMonster ^ Game.Map[x,y] is IMonster)
                            if (nextCreature.GetCreatureType() == CreatureType.Player)
                            {
                                Game.Fight(
                                    (Creatures.Player)nextCreature,
                                    (IMonster)Game.Map[x, y]
                                    );
                                if (Game.Map[x,y] != null && Game.Map[x, y].GetCreatureType() != CreatureType.Boss)
                                    Game.Map[x, y] = player;
                            }
                            else
                            {
                                Game.Fight(
                                    (Creatures.Player)Game.Map[x, y],
                                    (IMonster)nextCreature
                                    );
                                if (nextCreature.GetCreatureType() != CreatureType.Boss)
                                    Game.Map[x, y] = player;
                            }
                        timer.Start();
                    }
                }
            }
            tickCount++;
            if (tickCount == 8) tickCount = 0;
            Invalidate();
        }
    }
}
