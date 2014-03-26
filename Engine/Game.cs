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
        public static void Fight(Creatures.Player player, IMonster monster)
        {
            var form = new Fight(player, monster);
            form.ShowDialog();
            if (player.hp <= 0) 
                GameOver();
        }
        public static void GameOver()
        {
            MessageBox.Show("GAME OVER.");
            Environment.Exit(0);
        }
    }
}
