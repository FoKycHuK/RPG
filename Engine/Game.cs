using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MyRPG
{
    public class Game
    {
        public static bool InsideWorld(int x, int y)
        {
            return x >= 0 && y >= 0 && x < MapWidth && y < MapHeight;
        }
        public static event Action StageChanged;
        public static event Func<bool> Grave;
        public static Action Level;
        public static Random rand = new Random();
        public static ICreature[,] Map;
        public static int MapWidth;
        public static int MapHeight;
        public static Creatures.Player player;
        public static int Stage;
        public static string StageName;
        static bool firstCreating = true;
        public static bool FightIsOver;
        public static void Begin()
        {
            player = new Creatures.Player() { hp = 10, attack = 2, defence = 1, level = 1, exp = 0 };
            Stage = 0;
            CreateMap();
        }
        public static void Spawner(ICreature creature)
        {
            while (true)
            {
                var x = rand.Next(0, MapWidth);
                var y = rand.Next(0, MapHeight);
                if (Map[x, y] == null)
                {
                    Map[x, y] = creature;
                    break;
                }

            }
        }
        public static void CreateMap()
        {
            Stage++;
            switch (Stage % 8)
            {
                case 1:
                    StageName = "Valley";
                    break;
                case 2:
                    StageName = "Cave";
                    break;
                case 3:
                    StageName = "Quarry";
                    break;
                case 4:
                    StageName = "Desert";
                    break;
                case 5:
                    StageName = "North";
                    break;
                case 6:
                    StageName = "Dungeon";
                    break;
                case 7:
                    StageName = "Swamp";
                    break;
                case 0:
                    StageName = "Hell";
                    break;
                default:
                    MessageBox.Show("Error in stage defination");
                    break;
            }
            if (firstCreating)
                firstCreating = false;
            else
                StageChanged();
            MapWidth = 30;
            MapHeight = 19;
            Map = new ICreature[MapWidth, MapHeight];
            Map[0, 0] = player;
            Map[/*MapWidth - 1*/ 0, /*MapHeight -*/ 1] = new Creatures.Boss() { hp = 0, attack = Stage * 3, defence = Stage, expGain = Stage * 70 };
            for (var i = 0; i < 2 + Stage * 3; i++)
                Spawner(new Creatures.Monster() { hp = Stage * 7, attack = Stage * 2, defence = (Stage + 1) / 2 , expGain = Stage * 10 });
            if (rand.Next(0, 1000) < 600 + Stage * 25 )
                Spawner(new Creatures.Armor() { AwardDefence = 1 });
            if (rand.Next(0, 1000) < 700 - Stage * 50)
                Spawner(new Creatures.HealingPotion());
            if (rand.Next(0, 1000) < 600 + Stage * 25)
                Spawner(new Creatures.Sword() { AwardAttack = 2 });
            if (rand.Next(0, 1000) < 200 + Stage * 25)
                Spawner(new Creatures.Grave() { AwardAttack = rand.Next(-2, 3), AwardDefence = rand.Next(-1, 2) });
            if (rand.Next(0, 1000) > 300 + Stage * 10)
                Spawner(new Creatures.Chest() { AwardAttack = rand.Next(0, 2), AwardExp = rand.Next(Stage * 50, Stage * 100)});
        }
        public static void Conflict(ICreature nextCreature, int x, int y)
        {
            
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
                                    if (Grave())
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
        }
        public static void FightAct(IMonster monster)
        {
            if (monster.attack - player.defence > 0)
                player.hp -= monster.attack - player.defence;
            if (player.attack - monster.defence > 0)
                monster.hp -= player.attack - monster.defence;
            if (monster.hp <= 0 || player.hp <= 0)
                FightIsOver = true;
        }
        public static void WinInFight(IMonster monster)
        {
            MessageBox.Show("You win! Gained exp: " + monster.expGain.ToString());
            player.exp += monster.expGain;
            while (player.exp >= player.level * 100)
            {
                Level();
                player.level++;
                player.hp = player.level * 10;
                player.exp -= (player.level - 1) * 100;
                if (player.exp < 0)
                    player.exp = 0;
            }
            if (monster.GetCreatureType() == CreatureType.Boss)
            {
                MessageBox.Show("You killed boss, and going to next Stage!");
                CreateMap();
            }
            FightIsOver = false;
        }
        public static void Fight(Creatures.Player player, IMonster monster)
        {
            var form = new Fight(player, monster);
            form.ShowDialog();
        }
        public static void GameOver()
        {
            MessageBox.Show("You lose. GAME OVER.");
            Environment.Exit(0);
        }
    }
}
