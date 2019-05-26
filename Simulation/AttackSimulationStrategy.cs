using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class AttackSimulationStrategy : ISimulationStrategy
    {
        private HashSet<Position>[] visited = new[]
        {
            new HashSet<Position>(), 
            new HashSet<Position>(), 
            new HashSet<Position>(), 
            new HashSet<Position>() 
        };
        private int maxRate = int.MinValue;

        public void StartSimulate(GameMap game)
        {
        }

        public bool IsGoodPlaceForTrain(GameMap game, TrainCommand cmd)
        {
            return IsGoodPlaceInternal(game, cmd.Target, visited[cmd.Level]);
        }

        public bool IsGoodPlaceForMove(GameMap game, Position target)
        {
            return IsGoodPlaceInternal(game, target);
        }

        public bool IsGoodTurnForContinue(GameMap game, TurnGenerator.PossibleTurn turn)
        {
            return turn.Rate == maxRate;
        }

        public IEnumerable<ICommand[]> PrepareMoveCommand(GameMap game, IEnumerable<ICommand[]> moveCommands)
        {
            return moveCommands.OrderByDescending(cmds => cmds.Min(c => game.OpponentHq.Position.MDistanceTo(c.Target)));
        }

        public IEnumerable<ICommand> PreparTrainCommand(GameMap game, IEnumerable<ICommand> trainCommands)
        {
            return trainCommands.Where(t => IsGoodPlaceForTrain(game, t as TrainCommand)).OrderByDescending(c => game.OpponentHq.Position.MDistanceTo(c.Target));
        }

        private bool IsGoodPlaceInternal(GameMap game, Position target, HashSet<Position> visited = null)
        {
            if (visited != null)
            {
                if (visited.Contains(target))
                    return false;

                visited.Add(target);
            }

            var tile = game.Map[target.X, target.Y];
            return !tile.IsOwned;
        }

        public int RateGame(GameMap game)
        {
            var units = game.OpponentUnits.Select(u => Unit.TrainCosts[u.Level]).Sum();
            var places = game.OpPlaces * 5;
            var rate = -(units + places);
            if (rate > maxRate)
                maxRate = rate;
            return rate;
        }

        public bool HasImprove(int previous, int next)
        {
            return next > previous;
        }
    }
}