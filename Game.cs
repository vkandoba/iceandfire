using System;
using System.Text;

namespace IceAndFire
{
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

            Commands.Wait();
            strategy.MoveUnits();
            strategy.TrainUnits();
            strategy.ConstructBuildings();
        }

        private IStrategy ChoiceStrategy(GameMap gameMap)
        {
            if (gameMap.HasMenace())
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