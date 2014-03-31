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
        public static event Action FightOver;
        public static Random rand = new Random();
        public static ICreature[,] Map;
        public static int MapWidth = 30;
        public static int MapHeight = 19;
        public static Creatures.Player player;
        public static int Stage;
        public static string StageName;
        public static bool IsAdventure;

        public static void Begin()
        {
            if (player == null)
                player = new Creatures.Player() { hp = 10, attack = 2, defence = 1, level = 1, exp = 0 };
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
            for (var i = 1; i < 7; i++)
                Map[MapWidth - i, MapHeight - 2] = new Creatures.Wall();
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
                Spawner(new Creatures.Chest());
            
        }

        public static void LoadAdventureMap()
        {
            Stage++;
            StageDefination();
            StageChanged();
            Map = new ICreature[MapWidth, MapHeight];
            var file = File.ReadAllLines("Maps\\Map" + Stage % 8 + ".txt");
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
                        case 'D':
                            Map[j, i] = new Creatures.Devil();
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

            if (Map[x, y].GetCreatureType() == CreatureType.HealingPotion)
            {
                player.hp = player.level * 10;
                Map[x, y] = player;
            }
            if (Map[x, y] is ITreasure)
            {
                FoundTreasure((ITreasure)Map[x, y]);
                Map[x, y] = player;
            }
            if (nextCreature is IMonster ^ Map[x, y] is IMonster)
            {
                if (nextCreature.GetCreatureType() == CreatureType.Player)
                    Fight((Creatures.Player)nextCreature, (IMonster)Map[x, y]);
                else
                    Fight((Creatures.Player)Map[x, y], (IMonster)nextCreature);
                if (Map[x, y] is IMonster && Map[x, y].GetCreatureType() != CreatureType.Boss)
                    Map[x, y] = player;
            }
        }
        public static void FoundTreasure(ITreasure treasure)
        {
            player.attack += treasure.AwardAttack;
            player.exp += treasure.AwardExp;
            player.defence += treasure.AwardDefence;
            switch (treasure.GetCreatureType())
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
                        MessageBox.Show(string.Format("You entered in the grave and found curse/blessing! Gained: {0} Attack, {1} Defence.", treasure.AwardAttack, treasure.AwardDefence));
                        if (rand.Next(0, 1000) > 800)
                        {
                            MessageBox.Show("And you're under attack!!!");
                            Fight(player, new Creatures.PowerfulMonster());
                        }
                    }
                    else
                    {
                        player.attack -= treasure.AwardAttack;
                        player.defence -= treasure.AwardDefence;
                        var gainedExp = rand.Next(0, Stage * 50);
                        player.exp += gainedExp;
                        MessageBox.Show(string.Format("You running away and gain some exp: {0}", gainedExp));
                        CheckLevel();
                    }
                    break;
                default:
                    MessageBox.Show(string.Format("You found treasure! Gained: {0} Exp, {1} Attack, {2} Defence.", treasure.AwardExp, treasure.AwardAttack, treasure.AwardDefence));
                    CheckLevel();
                    break;
            }
        }
        public static void FightAct(IMonster monster)
        {
            if (monster.attack - player.defence > 0)
                player.hp -= monster.attack - player.defence;
            if (player.attack - monster.defence > 0)
                monster.hp -= player.attack - monster.defence;
            else
            {
                FightOver();
                MessageBox.Show("Your attack less or equal than a monster defence. You lost");
                GameOver();
            }
            if (player.hp <= 0)
            {
                FightOver();
                GameOver();
            }
            if (monster.hp <= 0)
            {
                FightOver();
                MessageBox.Show("You win! Gained exp: " + monster.expGain.ToString());
                player.exp += monster.expGain;
                CheckLevel();
                if (monster.GetCreatureType() == CreatureType.Boss)
                {
                    MessageBox.Show("You killed the boss and go to the next stage!");
                    if (IsAdventure)
                        LoadAdventureMap();
                    else
                        CreateRandomMap();
                }
                if (monster.GetCreatureType() == CreatureType.Devil)
                {
                    MessageBox.Show("You complete my game. Good job! Try survival mod. You'll save your hero.");
                    Begin();
                }
            }
        }

        public static void Fight(Creatures.Player player, IMonster monster)
        {
            var form = new Fight(player, monster);
            form.ShowDialog();
        }
        public static void CheckLevel()
        {
            while (player.exp >= player.level * 100)
            {
                Level();
                player.level++;
                player.hp = player.level * 10;
                player.exp -= (player.level - 1) * 100;
                if (player.exp < 0)
                    player.exp = 0;
            }
        }
        public static void GameOver()
        {
            MessageBox.Show("You lost. GAME OVER.");
            Environment.Exit(0);
        }
    }
}
