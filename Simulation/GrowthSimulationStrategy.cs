using System.Collections.Generic;

namespace IceAndFire
{
    public class GrowthSimulationStrategy : ISimulationStrategy
    {
        private HashSet<Position> visited;

        public void StartSimulate(GameMap game)
        {
            visited = new HashSet<Position>();
        }

        public bool IsGoodPlace(GameMap game, Position target)
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