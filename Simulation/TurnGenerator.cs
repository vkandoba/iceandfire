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
            return GenerateNextTrains(game, new List<ICommand>(), strategy.RateGame(game));
        }

        private IEnumerable<PossibleTurn> GenerateNextTrains(GameMap game, List<ICommand> prefix, int previousState)
        {
            var nextTrains = CommandGenerator.Trains(game).Where(cmd => strategy.IsGoodPlace(game, cmd.Target)).ToList();
            if (!nextTrains.Any())
                yield break;

            var chains = new List<PossibleTurn>();
            foreach (var nextTrain in nextTrains)
            {
                nextTrain.Apply(game);
                var withNext = prefix.Concat(new[] { nextTrain }).ToList();
                var rate = strategy.RateGame(game);
                if (strategy.HasImprove(previousState, rate))
                {
                    yield return new PossibleTurn{Rate = rate, Commands = withNext};
                    chains.AddRange(GenerateNextTrains(game, withNext, rate));
                }
                nextTrain.Unapply(game);
            }

            foreach (var chain in chains)
                yield return chain;
        }
        
    }
}