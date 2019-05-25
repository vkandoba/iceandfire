using System.Collections.Generic;

namespace IceAndFire
{
    public class GrowthSimulationStrategy : ISimulationStrategy
    {
        private HashSet<Position> visitedMove;
        private HashSet<Position> visitedTrain;

        public void StartSimulate(GameMap game)
        {
            visitedMove = new HashSet<Position>();
            visitedTrain = new HashSet<Position>();
        }

        public bool IsGoodPlaceForTrain(GameMap game, Position target)
        {
            return IsGoodPlaceInternal(game, target, visitedTrain);
        }

        public bool IsGoodPlaceForMove(GameMap game, Position target)
        {
            return IsGoodPlaceInternal(game, target, visitedMove);
        }

        private bool IsGoodPlaceInternal(GameMap game, Position target, HashSet<Position> visited)
        {
            if (visited.Contains(target))
                return false;

            visited.Add(target);
            var tile = game.Map[target.X, target.Y];
            return !tile.IsOwned;
        }

        public int RateGame(GameMap game)
        {
            return game.MyPositions.Count;
        }

        public bool HasImprove(int previous, int next)
        {
            return next > previous;
        }
    }
}