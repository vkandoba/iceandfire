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
            var isAttack = strategy() is AttackSimulationStrategy;
            var deep = (isAttack) ? 1 : (Turn <= 5 && IceAndFire.game.MyUnits.Length < 5 ? 2 : 1);
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