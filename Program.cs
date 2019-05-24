using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public class IceAndFire
    {

        public static GameMap game;
        public static Game gameEngine;

        public const int ME = 0;
        public const int OPPONENT = 1;
        public const int NEUTRAL = -1;

        public const int MINE_BUILD_COST = 30;
        public const int TOWER_BUILD_COST = 15;


        public static void Main()
        {
            game = new GameMap();
            game.Init();
            gameEngine = new Game();

            gameEngine.Turn = 0;
            // game loop
            while (true)
            {
                gameEngine.Output.Clear();
                game.Update();
                gameEngine.Solve(game);
                Console.WriteLine(gameEngine.Output.ToString());
                gameEngine.Turn++;
            }
        }

    }

    public class Game
    {

        public readonly StringBuilder Output = new StringBuilder();

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
            strategy.MoveUnits();

            strategy.TrainUnits();

            strategy.ConstructBuildings();

        }

        private IStrategy ChoiceStrategy(GameMap gameMap)
        {
            if (Strategies.Defense.HasMenace())
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
