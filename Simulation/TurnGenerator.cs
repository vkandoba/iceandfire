using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class TurnGenerator
    {
        private readonly Func<ISimulationStrategy> createStrategy;

        public class PossibleTurn
        {
            public List<ICommand> Commands { get; set; }
            public int TurnDeep { get; set; }
            public int Rate { get; set; }
        }

        public TurnGenerator(Func<ISimulationStrategy> createStrategy)
        {
            this.createStrategy = createStrategy;
        }

        public IEnumerable<PossibleTurn> Turns(GameMap game, int deep = 1)
        {
            var strategy = createStrategy();
            strategy.StartSimulate(game);
            return GenerateNextMoves(game, strategy, new List<ICommand>(), 1, deep, strategy.RateGame(game));
        }

        private IEnumerable<PossibleTurn> GenerateNextMoves(GameMap game, ISimulationStrategy strategy, List<ICommand> prefix, int deep, int deepLimit, int previousState)
        {
            return BaseGenerateNext(game, strategy, prefix, deep, deepLimit, previousState,
                (g) => CommandGenerator.Moves(g).Where(cmd => strategy.IsGoodPlaceForMove(g, cmd.Target)).ToList(),
                GenerateNextTrains);
        }

        private IEnumerable<PossibleTurn> GenerateNextTrains(GameMap game, ISimulationStrategy strategy, List<ICommand> prefix, int deep, int deepLimit, int previousState)
        {
            return BaseGenerateNext(game, strategy, prefix, deep, deepLimit, previousState,
                (g) => CommandGenerator.Trains(g).Where(cmd => strategy.IsGoodPlaceForTrain(g, cmd.Target)).ToList(),
                (g, _, p, d, dlimit, s) =>
                {
                    if (d + 1 > dlimit)
                        return new List<PossibleTurn>();
                    g.UpTurn();
                    var newStrategy = createStrategy();
                    newStrategy.StartSimulate(game);
                    var turns = GenerateNextMoves(g, newStrategy, p, d + 1, dlimit, s);
                    g.DownTurn();
                    return turns;
                });
        }

        private IEnumerable<PossibleTurn> BaseGenerateNext(GameMap game, ISimulationStrategy strategy, List<ICommand> prefix, int deep, int deepLimit, int previousState,
            Func<GameMap, List<ICommand>> generateNext,
            Func<GameMap, ISimulationStrategy, List<ICommand>, int, int, int, IEnumerable<PossibleTurn>> continuation)
        {
            var nexts = generateNext(game);
            if (!nexts.Any())
                foreach (var turn in continuation(game, strategy, prefix, deep, deepLimit, previousState))
                {
                    yield return turn;
                }

            var chains = new List<PossibleTurn>();
            foreach (var next in nexts)
            {
                next.Apply(game);
                var withNext = prefix.Concat(new[] { next }).ToList();
                var rate = strategy.RateGame(game);
                if (strategy.HasImprove(previousState, rate))
                {
                    if (deep == deepLimit)
                        yield return new PossibleTurn { Rate = rate, Commands = withNext, TurnDeep = deep};
                    chains.AddRange(BaseGenerateNext(game, strategy, withNext, deep, deepLimit, rate, generateNext, continuation));
                }
                next.Unapply(game);
            }

            foreach (var chain in chains)
                yield return chain;
        }
    }
}