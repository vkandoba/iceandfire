using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public class IceAndFire
    {

        public static Game game;

        public const int WIDTH = 12;
        public const int HEIGHT = 12;

        public const int ME = 0;
        public const int OPPONENT = 1;
        public const int NEUTRAL = -1;

        public const int TRAIN_COST_LEVEL_1 = 10;
        public const int TRAIN_COST_LEVEL_2 = 20;
        public const int TRAIN_COST_LEVEL_3 = 30;
        public const int MINE_BUILD_COST = 30;

        private static void Main()
        {
            game = new Game();
            game.Init();

            // game loop
            while (true)
            {
                game.Update();
                game.Solve();
                Console.WriteLine(game.Output.ToString());
            }
        }

        public class Game
        {
            public readonly List<Building> Buildings = new List<Building>();

            public readonly Tile[,] Map = new Tile[WIDTH, HEIGHT];
            public readonly StringBuilder Output = new StringBuilder();

            // Not Usefull in Wood3
            public List<Position> MineSpots = new List<Position>();

            public int MyGold => ActualGold - HoldGold;
            public int ActualGold;
            public int HoldGold;
            public int MyIncome;
            public int MyUpkeep;
            public Team MyTeam;

            public int OpponentGold;
            public int OpponentIncome;
            public int Turn;
            public List<Unit> Units = new List<Unit>();

            public List<Unit> MyUnits => Units.Where(u => u.IsOwned).ToList();
            public List<Unit> OpponentUnits => Units.Where(u => u.IsOpponent).ToList();

            public Position MyHq => MyTeam == Team.Fire ? (0, 0) : (11, 11);
            public Position OpponentHq => MyTeam == Team.Fire ? (11, 11) : (0, 0);

            public List<Position> MyPositions = new List<Position>();
            public List<Position> OpponentPositions = new List<Position>();
            public List<Position> NeutralPositions = new List<Position>();
            public List<Position> HoldPositions = new List<Position>();

            public void Init()
            {
                for (var y = 0; y < HEIGHT; y++)
                    for (var x = 0; x < WIDTH; x++)
                    {
                        Map[x, y] = new Tile
                        {
                            Position = (x, y)
                        };
                    }

                var numberMineSpots = int.Parse(Console.ReadLine());
                for (var i = 0; i < numberMineSpots; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');
                    MineSpots.Add((int.Parse(inputs[0]), int.Parse(inputs[1])));
                }
            }

            public void Update()
            {
                Units.Clear();
                Buildings.Clear();

                MyPositions.Clear();
                OpponentPositions.Clear();
                NeutralPositions.Clear();
                HoldPositions.Clear();

                Output.Clear();

                // --------------------------------------

                ActualGold = int.Parse(Console.ReadLine());
                MyIncome = int.Parse(Console.ReadLine());
                OpponentGold = int.Parse(Console.ReadLine());
                OpponentIncome = int.Parse(Console.ReadLine());

                // Read Map
                for (var y = 0; y < HEIGHT; y++)
                {
                    var line = Console.ReadLine();
                    for (var x = 0; x < WIDTH; x++)
                    {
                        var c = line[x] + "";
                        Map[x, y].IsWall = c == "#";
                        Map[x, y].Active = "OX".Contains(c);
                        Map[x, y].Owner = c.ToLower() == "o" ? ME : c.ToLower() == "x" ? OPPONENT : NEUTRAL;
                        Map[x, y].HasMineSpot = MineSpots.Count(spot => spot == (x, y)) > 0;

                        Position p = (x, y);
                        if (Map[x, y].IsOwned)
                            MyPositions.Add(p);
                        else if (Map[x, y].IsOpponent)
                            OpponentPositions.Add(p);
                        else if (!Map[x, y].IsWall)
                        {
                            NeutralPositions.Add(p);
                        }
                    }
                }

                // Read Buildings
                var buildingCount = int.Parse(Console.ReadLine());
                for (var i = 0; i < buildingCount; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');
                    Buildings.Add(new Building
                    {
                        Owner = int.Parse(inputs[0]),
                        Type = (BuildingType)int.Parse(inputs[1]),
                        Position = (int.Parse(inputs[2]), int.Parse(inputs[3]))
                    });
                }

                // Read Units
                var unitCount = int.Parse(Console.ReadLine());
                for (var i = 0; i < unitCount; i++)
                {
                    var inputs = Console.ReadLine().Split(' ');
                    Units.Add(new Unit
                    {
                        Owner = int.Parse(inputs[0]),
                        Id = int.Parse(inputs[1]),
                        Level = int.Parse(inputs[2]),
                        Position = (int.Parse(inputs[3]), int.Parse(inputs[4]))
                    });
                }

                MyUpkeep = MyUnits.Sum(u => u.Upkeep);

                // --------------------------------

                // Get Team
                MyTeam = Buildings.Find(b => b.IsHq && b.IsOwned).Position == (0, 0) ? Team.Fire : Team.Ice;

                // Usefull for symmetric AI
                if (MyTeam == Team.Ice)
                {
                    MyPositions.Reverse();
                    OpponentPositions.Reverse();
                    NeutralPositions.Reverse();
                }

                // --------------------------------

                // Debug
                Debug();
            }

            public void Debug()
            {
                Console.Error.WriteLine($"Turn: {Turn}");
                Console.Error.WriteLine($"My team: {MyTeam}");
                Console.Error.WriteLine($"My gold: {ActualGold} (+{MyIncome})");
                Console.Error.WriteLine($"Opponent gold: {OpponentGold} (+{OpponentIncome})");

                Console.Error.WriteLine("=====");
                foreach (var b in Buildings) Console.Error.WriteLine(b);
                foreach (var u in Units) Console.Error.WriteLine(u);
            }

            /***
             * -----------------------------------------------------------
             * TODO Solve
             * -----------------------------------------------------------
             */
            public void Solve()
            {
                // Make sur the AI doesn't timeout
                Wait();

                IStrategy strategy = Strategies.Growth;
                strategy.MoveUnits();

                strategy.TrainUnits();

                strategy.ConstructBuildings();

                Turn++;
            }


            public void Wait()
            {
                IceAndFire.game.Output.Append("WAIT;");
            }
        }


        public class Unit : Entity
        {

            public static readonly int[] UpkeepCosts = {0, 1, 4, 20};

            public int Id;
            public int Level;
            public int Upkeep => UpkeepCosts[Level];

            public override string ToString() => $"Unit => {base.ToString()} Id: {Id} Level: {Level}";
        }

        public class Building : Entity
        {
            public BuildingType Type;

            public bool IsHq => Type == BuildingType.Hq;
            public bool IsTower => Type == BuildingType.Tower;
            public bool IsMine => Type == BuildingType.Mine;

            public override string ToString() => $"Building => {base.ToString()} Type: {Type}";
        }

        public class Entity
        {
            public int Owner;
            public Position Position;

            public bool IsOwned => Owner == ME;
            public bool IsOpponent => Owner == OPPONENT;

            public int X => Position.X;
            public int Y => Position.Y;

            public override string ToString() => $"Owner: {Owner} Position: {Position}";
        }
    }
}
