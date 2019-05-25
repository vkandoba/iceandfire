using System;
using System.Linq;

namespace IceAndFire
{
    public class GrowthStrategy : IStrategy
    {
        public void MoveUnits()
        {
            var turnGenerator = new TurnGenerator(() => new GrowthSimulationStrategy());
            var possibleTurns = turnGenerator.Turns(IceAndFire.game, 3).ToArray();
            var bestTurn = possibleTurns.OrderByDescending(x => x.Rate).FirstOrDefault();
            if (bestTurn == null)
                return;
            foreach (var cmd in bestTurn.Commands)
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