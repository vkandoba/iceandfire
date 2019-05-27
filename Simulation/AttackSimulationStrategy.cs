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
            return moveCommands.OrderBy(cmds => cmds.Min(c => game.OpponentHq.Position.MDistanceTo(c.Target)));
        }

        public IEnumerable<ICommand> PreparTrainCommand(GameMap game, IEnumerable<ICommand> trainCommands)
        {
            var cmd = trainCommands.Select(t => t as TrainCommand).Where(t => IsGoodPlaceForTrain(game, t));
            var filterd = cmd.Where(t =>
            {
                var tile = game.Map[t.Target.X, t.Target.Y];
                var isAttack = (tile.Unit != null || tile.Building != null || tile.IsUnderAttack);
                return t.Level == 1 || isAttack;
            });
            var sorted = filterd.OrderBy(c => game.OpponentHq.Position.MDistanceTo(c.Target));
            return sorted;
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

            if ((game.MyIncome - game.MyUpkeep) <= 0)
                return -2000;

            var units = game.OpponentUnits.Select(u => Unit.TrainCosts[u.Level]).Sum();
            var places = game.OpPlaces * 5;
            var rate = -(units + places) - (game.Me.Upkeep / 20);
            if (rate > maxRate)
                maxRate = rate;
            return rate;
        }

        public bool HasImprove(int previous, int next)
        {
            return next >= previous;
        }
    }
}