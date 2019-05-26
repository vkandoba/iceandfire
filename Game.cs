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
            Commands.Wait();

            var strategy = ChoiceStrategy(gameMap);
            var deep = IceAndFire.game.MyUnits.Length < 3  || strategy() is AttackSimulationStrategy ? 3 : 2;
            Console.Error.WriteLine($"strategy: {strategy().GetType().Name} deep: {deep}");

            SimStarategy.Moves(strategy, deep);
        }

        private Func<ISimulationStrategy> ChoiceStrategy(GameMap gameMap)
        {
            if (gameMap.HasMenace())
                return () => new AttackSimulationStrategy();

            return () => new GrowthSimulationStrategy();
        }
    }
}