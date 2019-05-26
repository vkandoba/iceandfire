using System;
using System.Linq;

namespace IceAndFire
{
    public class SimStarategy
    {
        public static void Moves(Func<ISimulationStrategy> createSimulation, int deep)
        {
            var turnGenerator = new TurnGenerator(createSimulation);
            var possibleTurns = turnGenerator.Turns(IceAndFire.game, deep).ToArray();
            if (!possibleTurns.Any())
            {
                Console.Error.WriteLine("WARNING NO MOVES");
                Commands.Wait();
            }
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

    }
}