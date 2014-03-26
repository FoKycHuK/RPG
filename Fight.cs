﻿using System;
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
        Bitmap monsterIcon = (Bitmap)Bitmap.FromFile("Images\\monsterFight.png");
        Bitmap playerIcon = (Bitmap)Bitmap.FromFile("Images\\playerFight.png");
        Creatures.Player player;
        IMonster monster;
        bool isOver = false;

        public Fight(Creatures.Player player, IMonster monster)
        {

            StartPosition = FormStartPosition.CenterScreen;
            ControlBox = false;
            this.player = player;
            this.monster = monster;
            ClientSize = new Size(ElementSize * 5, ElementSize * 2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Text = "FIGHT";
            DoubleBuffered = true;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += TimerTick;
            timer.Start();
        }

        void Act()
        {
            if (monster.attack - player.defence > 0)
                player.hp -= monster.attack - player.defence;
            if (player.attack - monster.defence > 0)
                monster.hp -= player.attack - monster.defence;
            if (monster.hp <= 0 || player.hp <= 0)
                isOver = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, 0);
            e.Graphics.FillRectangle(Brushes.Gray, 0, 0, ClientSize.Width, ClientSize.Height);
            e.Graphics.DrawImage(playerIcon, 0, 0);
            e.Graphics.DrawImage(monsterIcon, ClientSize.Width - ElementSize, 0);
            e.Graphics.ResetTransform();
            e.Graphics.DrawString("HP: " + player.hp.ToString(), new Font("Arial", 16), Brushes.Red, 0, ElementSize);
            e.Graphics.DrawString("HP: " + monster.hp.ToString(), new Font("Arial", 16), Brushes.Red, ClientSize.Width - ElementSize * 2 - 10 , ElementSize);
            e.Graphics.DrawString("VS", new Font("Arial", 16), Brushes.White, ElementSize * 2, 0);

        }
        void TimerTick(object sender, EventArgs args)
        {
            Act();
            Invalidate();
            if (isOver)
            {
                timer.Stop();
                if (player.hp <= 0)
                {
                    MessageBox.Show("You lose.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("You win! Gained exp: " + monster.expGain.ToString());
                    player.exp += monster.expGain;
                    while (player.exp >= player.level * 100)
                    {
                        Game.Level();
                        player.level++;
                        player.hp = player.level * 10;
                        player.exp -= (player.level - 1) * 100;
                        if (player.exp < 0)
                            player.exp = 0;
                    }
                    if (monster.GetCreatureType() == CreatureType.Boss)
                    {
                        MessageBox.Show("You killed boss, and going to next Stage!");
                        Game.CreateMap();
                    }
                    this.Close();
                }
            }
        }
    }
}
