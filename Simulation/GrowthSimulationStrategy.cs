using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class GrowthSimulationStrategy : ISimulationStrategy
    {
        private HashSet<Position> visitedTrain;
        private int maxRate = 0;

        public void StartSimulate(GameMap game)
        {
            visitedTrain = new HashSet<Position>();
        }

        public bool IsGoodPlaceForTrain(GameMap game, Position target)
        {
            return IsGoodPlaceInternal(game, target, visitedTrain);
        }

        public bool IsGoodPlaceForMove(GameMap game, Position target)
        {
            return true;
        }

        public bool IsGoodTurnForContinue(GameMap game, TurnGenerator.PossibleTurn turn)
        {
            return turn.Rate == maxRate;
        }

        public IEnumerable<ICommand[]> PrepareMoveCommand(GameMap game, IEnumerable<ICommand[]> moveCommands)
        {
            var moves = moveCommands.ToArray();
            for (int i = 0; i < moves.Length; i++)
            {
                var hasAttack = moves[i].Any(x => IsGoodPlaceInternal(game, x.Target, null));
                if (hasAttack)
                    moves[i] = moves[i].Where(x => IsGoodPlaceInternal(game, x.Target, null)).ToArray();
                else
                    moves[i] = moves[i].OrderBy(c => game.DistanceToOpHQ[c.Target.X, c.Target.Y]).Take(1).ToArray();
            }
            return moves.OrderByDescending(cmds => cmds.Sum(c => game.DistanceToMyHQ[c.Target.X, c.Target.Y]));
        }

        public IEnumerable<ICommand> PreparTrainCommand(GameMap game, IEnumerable<ICommand> trainCommands)
        {
            return trainCommands
                    .Where(t => IsGoodPlaceForTrain(game, t.Target) && (t as TrainCommand).Level == 1)
                    .OrderByDescending(c => game.DistanceToMyHQ[c.Target.X, c.Target.Y]);
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
            if (game.OpponentHq.IsOwned)
                return int.MaxValue;

            var rate = game.MyPlaces;
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