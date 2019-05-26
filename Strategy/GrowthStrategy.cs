using System;
using System.Linq;

namespace IceAndFire
{
    public class GrowthStrategy : IStrategy
    {
        public int Deep => IceAndFire.game.MyUnits.Length < 3 ? 3 : 2;

        public void MoveUnits()
        {
            var turnGenerator = new TurnGenerator(() => new GrowthSimulationStrategy());
            var possibleTurns = turnGenerator.Turns(IceAndFire.game, Deep).ToArray();
            var bestRate = possibleTurns.Max(x => x.Rate);
            var bestTurns = possibleTurns.Where(x => bestRate == x.Rate).ToArray();
            var best = bestTurns.FirstOrDefault();
            if (best == null)
                return;
            Console.Error.WriteLine(best);
            foreach (var cmd in best.Commands.Where(c => c.TurnDeep == 1).Select(c => c.Command))
            {
                cmd.Execute(IceAndFire.gameEngine);
            }
        }

        public void TrainUnits()
        {
        }

        public void ConstructBuildings()
        {
        }
    }
}