using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MyRPG
{
    public class Game
    {
        public static event Action StageChanged;
        public static event Func<bool> Grave;
        public static event Action ChooseMode;
        public static event Action Level;
        public static Random rand = new Random();
        public static ICreature[,] Map;
        public static int MapWidth = 30;
        public static int MapHeight = 19;
        public static Creatures.Player player;
        public static int Stage;
        public static string StageName;
        public static bool FightIsOver;
        public static bool IsAdventure;

        public static void Begin()
        {
            player = new Creatures.Player() { hp = 10, attack = 2, defence = 1, level = 1, exp = 0 };
            Stage = 0;
            ChooseMode();
            if (IsAdventure)
                LoadAdventureMap();
            else
                CreateRandomMap();
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
        public static void CreateRandomMap()
        {
            Stage++;
            StageDefination();
            StageChanged();
            Map = new ICreature[MapWidth, MapHeight];
            Map[0, 0] = player;
            Map[MapWidth - 1, MapHeight - 1] = new Creatures.Boss();
            Map[MapWidth - 1, MapHeight - 2] = new Creatures.Wall();
            Map[MapWidth - 2, MapHeight - 2] = new Creatures.Wall();
            Map[MapWidth - 3, MapHeight - 2] = new Creatures.Wall();
            Map[MapWidth - 4, MapHeight - 2] = new Creatures.Wall();
            Map[MapWidth - 5, MapHeight - 2] = new Creatures.Wall();
            Map[MapWidth - 6, MapHeight - 2] = new Creatures.Wall();
            for (var i = 0; i < 2 + Stage * 3; i++)
                Spawner(new Creatures.Monster());
            if (rand.Next(0, 1000) < 600 + Stage * 25 )
                Spawner(new Creatures.Armor());
            if (rand.Next(0, 1000) < 700 - Stage * 50)
                Spawner(new Creatures.HealingPotion());
            if (rand.Next(0, 1000) < 600 + Stage * 25)
                Spawner(new Creatures.Sword());
            if (rand.Next(0, 1000) < 200 + Stage * 25)
                Spawner(new Creatures.Grave());
            if (rand.Next(0, 1000) > 300 + Stage * 10)
                Spawner(new Creatures.Chest() { });
            
        }

        public static void LoadAdventureMap()
        {
            Stage++;
            if (Stage > 8)
            {
                MessageBox.Show("You complete my game. Good job! Try survival mod!");
            }
            StageDefination();
            StageChanged();
            Map = new ICreature[MapWidth, MapHeight];
            var file = File.ReadAllLines("Images\\Map" + Stage + ".txt");
            for (var i = 0; i < MapHeight; i++)
                for (var j = 0; j < MapWidth; j++)
                    switch (file[i][j])
                    {
                        case 'W':
                            Map[j, i] = new Creatures.Wall();
                            break;
                        case 'M':
                            Map[j, i] = new Creatures.Monster();
                            break;
                        case 'C':
                            Map[j, i] = new Creatures.Chest();
                            break;
                        case 'G':
                            Map[j, i] = new Creatures.Grave();
                            break;
                        case 'H':
                            Map[j, i] = new Creatures.HealingPotion();
                            break;
                        case 'B':
                            Map[j, i] = new Creatures.Boss();
                            break;
                        case 'S':
                            Map[j, i] = new Creatures.Sword();
                            break;
                        case 'A':
                            Map[j, i] = new Creatures.Armor();
                            break;
                        case 'P':
                            Map[j, i] = player;
                            break;
                        case 'm':
                            Map[j, i] = new Creatures.StandingMonster();
                            break;
                        case '#':
                            Map[j, i] = new Creatures.PowerfulMonster();
                            break;
                        default:
                            break;

                    }

        }

        public static void StageDefination()
        {
            switch (Stage % 8)
            {
                case 1:
                    StageName = "Valley";
                    break;
                case 2:
                    StageName = "Desert";
                    break;
                case 3:
                    StageName = "Quarry";
                    break;
                case 4:
                    StageName = "Cave";
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
        }
        public static bool InsideWorld(int x, int y)
        {
            return x >= 0 && y >= 0 && x < MapWidth && y < MapHeight;
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
                                    MessageBox.Show("You found an armor better than you have. Your defence increased on 1!");
                                    break;
                                case CreatureType.Sword:
                                    MessageBox.Show("You found a sword better than you have. Your attack increased on 2!");
                                    break;
                                case CreatureType.Grave:
                                    if (Grave())
                                    {
                                        MessageBox.Show(string.Format("You entered in the grave and found curse/blessing! Gained: {0} Attack, {1} Defence.", aw.AwardAttack, aw.AwardDefence));
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
                                        MessageBox.Show(string.Format("You running away and gain some exp: {0}", gainedExp));
                                    }
                                    break;
                                default:
                                    MessageBox.Show(string.Format("You found some treasures! Gained: {0} Exp, {1} Attack, {2} Defence.", aw.AwardExp, aw.AwardAttack, aw.AwardDefence));
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
        public static void EndOfFight(IMonster monster)
        {
            if (player.hp <= 0)
                Game.GameOver();
            Game.WinInFight(monster);
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
                MessageBox.Show("You killed the boss and go to the next stage!");
                if (IsAdventure)
                    LoadAdventureMap();
                else
                    CreateRandomMap();
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
            MessageBox.Show("You lost. GAME OVER.");
            Environment.Exit(0);
        }
    }
}
