using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            gameMap.Init();
            gameEngine = new Game(gameMap);

            gameEngine.Turn = 0;
            // game loop
            while (true)
            {
                gameEngine.Output.Clear();
                gameMap.Update();
                gameEngine.Solve(gameMap);
                Console.WriteLine(gameEngine.Output.ToString());
                gameEngine.Turn++;
            }
        }

    }

    public class Game
    {

        public Game(GameMap map)
        {
            Map = map;
        }

        public readonly StringBuilder Output = new StringBuilder();
        public readonly GameMap Map;

        public int Turn { get; set; }
        /***
         * -----------------------------------------------------------
         * TODO Solve
         * -----------------------------------------------------------
         */
        public void Solve(GameMap gameMap)
        {
            // Make sur the AI doesn't timeout
            Wait();

            var strategy = ChoiceStrategy(gameMap);
            Console.Error.WriteLine($"strategy: {strategy.GetType().Name}");

            var commands = new List<ICommand>();
            commands.AddRange(strategy.MoveUnits());
            commands.AddRange(strategy.TrainUnits());
            commands.AddRange(strategy.ConstructBuildings());

            foreach (var cmd in commands)
            {
                cmd.Apply(this);
            }
        }

        private IStrategy ChoiceStrategy(GameMap gameMap)
        {
            if (Strategies.Defense.HasMenace(gameMap))
                return Strategies.Defense;
            if (gameMap.MyIncome < 30)
                return Strategies.Growth;

            return Strategies.Base;
        }

        public void Wait()
        {
            Output.Append("WAIT;");
        }
    }

}
