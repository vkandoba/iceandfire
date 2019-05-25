using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class TurnGenerator
    {
        private readonly ISimulationStrategy strategy;

        public class PossibleTurn
        {
            public List<ICommand> Commands { get; set; }
            public int Rate { get; set; }
        }

        public TurnGenerator(ISimulationStrategy strategy)
        {
            this.strategy = strategy;
        }

        public IEnumerable<PossibleTurn> Turns(GameMap game)
        {
            strategy.StartSimulate(game);
            return GenerateNextMoves(game, new List<ICommand>(), strategy.RateGame(game));
        }

        private IEnumerable<PossibleTurn> GenerateNextMoves(GameMap game, List<ICommand> prefix, int previousState)
        {
            return BaseGenerateNext(game, prefix, previousState,
                (g) => CommandGenerator.Moves(g).Where(cmd => strategy.IsGoodPlaceForMove(g, cmd.Target)).ToList(),
                GenerateNextTrains);
        }

        private IEnumerable<PossibleTurn> GenerateNextTrains(GameMap game, List<ICommand> prefix, int previousState)
        {
            return BaseGenerateNext(game, prefix, previousState,
                (g) => CommandGenerator.Trains(g).Where(cmd => strategy.IsGoodPlaceForTrain(g, cmd.Target)).ToList(),
                (g, p, s) => new List<PossibleTurn>());
        }

        private IEnumerable<PossibleTurn> BaseGenerateNext(GameMap game, List<ICommand> prefix, int previousState,
            Func<GameMap, List<ICommand>> generateNext,
            Func<GameMap, List<ICommand>, int, IEnumerable<PossibleTurn>> continuation)
        {
            var nexts = generateNext(game);
            if (!nexts.Any())
                foreach (var turn in continuation(game, prefix, previousState))
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
                    yield return new PossibleTurn { Rate = rate, Commands = withNext };
                    chains.AddRange(BaseGenerateNext(game, withNext, rate, generateNext, continuation));
                }
                next.Unapply(game);
            }

            foreach (var chain in chains)
                yield return chain;
        }
    }
}