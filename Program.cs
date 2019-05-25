using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class IceAndFire
    {
        public static GameMap game => gameEngine.Map;

        public static Game gameEngine;

        public const int ME = 0;
        public const int OPPONENT = 1;
        public const int NEUTRAL = -1;

        public const int MINE_BUILD_COST = 30;
        public const int TOWER_BUILD_COST = 15;

        public static void Main()
        {
            var gameMap = new GameMap();
            ReadAndInitMap(gameMap);
            gameEngine = new Game(gameMap);

            gameEngine.Turn = 0;
            // game loop
            while (true)
            {
                gameEngine.Output.Clear();
                ReadAndUpdateMap(gameEngine.Map);
                gameEngine.Solve(gameMap);
                Console.WriteLine(gameEngine.Output.ToString());
                gameEngine.Turn++;
            }
        }

        public static void ReadAndInitMap(GameMap gameMap)
        {
            for (var y = 0; y < GameMap.HEIGHT; y++)
            for (var x = 0; x < GameMap.WIDTH; x++)
            {
                gameMap.Map[x, y] = new Tile
                {
                    Position = (x, y)
                };
            }

            var numberMineSpots = int.Parse(Console.ReadLine());
            for (var i = 0; i < numberMineSpots; i++)
            {
                var inputs = Console.ReadLine().Split(' ');
                gameMap.MineSpots.Add((int.Parse(inputs[0]), int.Parse(inputs[1])));
            }
        }

        public static void ReadAndUpdateMap(GameMap gameMap, bool printDebug = false)
        {
            gameMap.Units.Clear();
            gameMap.Buildings.Clear();

            gameMap.MyPositions.Clear();
            gameMap.OpponentPositions.Clear();
            gameMap.NeutralPositions.Clear();
            gameMap.HoldPositions.Clear();
            gameMap.HoldGold = 0;
            gameMap.HoldUpkeep = 0;


            // --------------------------------------

            gameMap.ActualGold = int.Parse(Console.ReadLine());
            gameMap.MyIncome = int.Parse(Console.ReadLine());
            gameMap.OpponentGold = int.Parse(Console.ReadLine());
            gameMap.OpponentIncome = int.Parse(Console.ReadLine());

            // Read Map
            for (var y = 0; y < GameMap.HEIGHT; y++)
            {
                var line = Console.ReadLine();
                for (var x = 0; x < GameMap.WIDTH; x++)
                {
                    var c = line[x] + "";
                    var cell = gameMap.Map[x, y];
                    cell.IsWall = c == "#";
                    cell.Active = "OX".Contains(c);
                    cell.Owner = c.ToLower() == "o" ? IceAndFire.ME : c.ToLower() == "x" ? IceAndFire.OPPONENT : IceAndFire.NEUTRAL;
                    cell.HasMineSpot = gameMap.MineSpots.Count(spot => spot == (x, y)) > 0;

                    cell.Unit = null;
                    cell.Building = null;

                    if (cell.IsOwned && cell.Active)
                        gameMap.MyPositions.Add(cell);
                    else if (cell.IsOpponent && cell.Active)
                        gameMap.OpponentPositions.Add(cell);
                    else if (!cell.IsWall)
                    {
                        gameMap.NeutralPositions.Add(cell);
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
                gameMap.Buildings.Add(building);
                gameMap.Map[building.X, building.Y].Building = building;
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
                gameMap.Units.Add(unit);
                gameMap.Map[unit.X, unit.Y].Unit = unit;
            }
            gameMap.ActuaUpkeep = gameMap.MyUnits.Sum(u => u.Upkeep);

            // --------------------------------

            // Get Team
            gameMap.MyTeam = gameMap.Buildings.Find(b => b.IsHq && b.IsOwned).Position == (0, 0) ? Team.Fire : Team.Ice;

            // Usefull for symmetric AI
            if (gameMap.MyTeam == Team.Ice)
            {
                gameMap.MyPositions.Reverse();
                gameMap.OpponentPositions.Reverse();
                gameMap.NeutralPositions.Reverse();
            }

            // --------------------------------

            // Debug
            if (printDebug)
                Debug(gameMap);
        }

        public static void Debug(GameMap gameMap)
        {
            Console.Error.WriteLine($"My team: {gameMap.MyTeam}");
            Console.Error.WriteLine($"My gold: {gameMap.ActualGold} (+{gameMap.MyIncome})");
            Console.Error.WriteLine($"Opponent gold: {gameMap.OpponentGold} (+{gameMap.OpponentIncome})");

            Console.Error.WriteLine("=====");
            foreach (var b in gameMap.Buildings) Console.Error.WriteLine(b);
            foreach (var u in gameMap.Units) Console.Error.WriteLine(u);
        }
    }
}
