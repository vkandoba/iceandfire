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
            return moves.OrderBy(cmds => cmds.Min(c => game.DistanceToOpHQ[c.Target.X, c.Target.Y]));
        }

        public IEnumerable<ICommand> PreparTrainCommand(GameMap game, IEnumerable<ICommand> trainCommands)
        {
            var cmd = trainCommands.Select(t => t as TrainCommand).Where(t => IsGoodPlaceForTrain(game, t)).ToArray();
            var hasAttack = cmd.Any(t => game.Map[t.Target.X, t.Target.Y].IsOpponent && game.Map[t.Target.X, t.Target.Y].Active);
            if (hasAttack)
                cmd = cmd.Where(t => game.Map[t.Target.X, t.Target.Y].IsOpponent && game.Map[t.Target.X, t.Target.Y].Active).ToArray();
            var filterd = cmd.Where(t =>
            {
                var tile = game.Map[t.Target.X, t.Target.Y];
                var isAttack = (tile.Unit != null || tile.Building != null || tile.IsUnderAttack);
                return t.Level == 1 || isAttack;
            });
            var sorted = filterd.OrderBy(c => game.DistanceToOpHQ[c.Target.X, c.Target.Y]);
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

            if ((game.MyUpkeep - game.MyIncome) > 0)
                return -2000;

            var actualPlaces = Geometry.MakeWave(game, t => t.Active && t.IsOpponent, game.OpponentHq);
            var actual_units = game.Units.Keys.Except(actualPlaces.Keys);
            var units = actual_units.Select(k => game.Units[k]).Where(u => u.IsOpponent)
                            .Select(u => Unit.TrainCosts[u.Level] + 5).Sum();
            var places = actualPlaces.Count * 5;
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