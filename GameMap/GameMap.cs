using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class GameMap
    {
        public const int WIDTH = 12;
        public const int HEIGHT = 12;

        public Team MyTeam;
        public int MyGold => ActualGold - HoldGold;
        public int ActualGold;
        public int ActuaUpkeep;
        public int HoldGold;
        public int HoldUpkeep;
        public int MyIncome;
        public int MyUpkeep => ActuaUpkeep - HoldUpkeep;

        public int OpponentIncome;
        public int OpponentGold;

        public readonly Tile[,] Map = new Tile[WIDTH, WIDTH];

        public readonly List<Building> Buildings = new List<Building>();
        public List<Unit> Units = new List<Unit>();

        public Position MyHq => MyTeam == Team.Fire ? (0, 0) : (11, 11);
        public Position OpponentHq => MyTeam == Team.Fire ? (11, 11) : (0, 0);

        public List<Position> MyPositions = new List<Position>();
        public List<Position> OpponentPositions = new List<Position>();
        public List<Position> NeutralPositions = new List<Position>();
        public HashSet<Position> HoldPositions = new HashSet<Position>();

        public List<Unit> MyUnits => Units.Where(u => u.IsOwned).ToList();
        public List<Unit> OpponentUnits => Units.Where(u => u.IsOpponent).ToList();

        public List<Position> MineSpots = new List<Position>();

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
            HoldGold = 0;
            HoldUpkeep = 0;


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
                    Map[x, y].Owner = c.ToLower() == "o" ? IceAndFire.ME : c.ToLower() == "x" ? IceAndFire.OPPONENT : IceAndFire.NEUTRAL;
                    Map[x, y].HasMineSpot = MineSpots.Count(spot => spot == (x, y)) > 0;

                    Map[x, y].Unit = null;
                    Map[x, y].Building = null;

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
                var building = new Building
                {
                    Owner = int.Parse(inputs[0]),
                    Type = (BuildingType)int.Parse(inputs[1]),
                    Position = (int.Parse(inputs[2]), int.Parse(inputs[3]))
                };
                Buildings.Add(building);
                Map[building.X, building.Y].Building = building;
            }

            // Read Units
            var unitCount = int.Parse(Console.ReadLine());
            for (var i = 0; i < unitCount; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                var unit = new Unit
                {
                    Owner = int.Parse(inputs[0]),
                    Id = int.Parse(inputs[1]),
                    Level = int.Parse(inputs[2]),
                    Position = (int.Parse(inputs[3]), int.Parse(inputs[4]))
                };
                Units.Add(unit);
                Map[unit.X, unit.Y].Unit = unit;
            }
            ActuaUpkeep = MyUnits.Sum(u => u.Upkeep);

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
            //Debug();
            Console.Error.WriteLine(Map[0, 0].Owner);
        }

        public void Debug()
        {
            Console.Error.WriteLine($"My team: {MyTeam}");
            Console.Error.WriteLine($"My gold: {ActualGold} (+{MyIncome})");
            Console.Error.WriteLine($"Opponent gold: {OpponentGold} (+{OpponentIncome})");

            Console.Error.WriteLine("=====");
            foreach (var b in Buildings) Console.Error.WriteLine(b);
            foreach (var u in Units) Console.Error.WriteLine(u);
        }

        public static Position[] FindPathInternal(Func<Position, bool> isFree, 
            Position source,
            Position target)
        {
            var queue = new Queue<Position>();
            var path = new Dictionary<Position, Position>();
            path[source] = source;
            queue.Enqueue(source);
            do
            {
                var pos = queue.Dequeue();
                if (pos == target)
                    return BuidlPath(path, target, source);
                var area = pos.Area4().Where(isFree).Where(c => !path.ContainsKey(c));
                foreach (var c in area)
                {
                    path[c] = pos;
                    queue.Enqueue(c);
                }
            } while (queue.Any());

            return new Position[0];
        }

        private static Position[] BuidlPath(Dictionary<Position, Position> links, Position target, Position source)
        {
            var path = new List<Position>();
            var current = target;
            do
            {
                path.Add(current);
                current = links[current];
            } while (current != source);

            path.Reverse();
            return path.ToArray();
        }
    }
}
