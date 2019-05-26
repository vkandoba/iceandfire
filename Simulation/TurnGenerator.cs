using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public class TurnGenerator
    {
        private readonly Func<ISimulationStrategy> createStrategy;

        public class CommandWithTurn
        {
            public ICommand Command { get; set; }
            public int TurnDeep { get; set; }
        }

        public class PossibleTurn
        {
            public List<CommandWithTurn> Commands { get; set; }
            public int Rate { get; set; }

            public override string ToString()
            {
                var chain = new StringBuilder();
                var deep = 1;
                foreach (var cmd in Commands)
                {
                    var add = cmd.TurnDeep > deep ? "||" : "";
                    chain.Append($"|{add}{cmd.Command} ");
                    deep = cmd.TurnDeep;
                }
                return $"{Rate}: {chain}";
            }
        }

        public TurnGenerator(Func<ISimulationStrategy> createStrategy)
        {
            this.createStrategy = createStrategy;
        }

        public IEnumerable<PossibleTurn> Turns(GameMap game, int deep = 1)
        {
            var strategy = createStrategy();
            strategy.StartSimulate(game);
            return GenerateNextMoves(game, strategy, new List<CommandWithTurn>(), 1, deep, strategy.RateGame(game));
        }

        public IEnumerable<PossibleTurn> OneTurnMoves(GameMap game)
        {
            var strategy = createStrategy();
            strategy.StartSimulate(game);
            var unitMoves = CommandGenerator.Moves(game).Select(cmd => cmd as MoveCommand).Where(cmd => strategy.IsGoodPlaceForMove(game, cmd.Target)).ToList();
            var groubByUnit = unitMoves.GroupBy(cmd => cmd.Unit).Select(g => g.ToList()).ToList();
            return CrossCommands(groubByUnit)
                .Where(x => x.Select(c => c.Target).Distinct().Count() == x.Count)
                .Select(x => new PossibleTurn
                {
                    Commands = x.Select(c => new CommandWithTurn
                    {
                        Command = c,
                        TurnDeep = 1
                    }).ToList()
                });
        }

        private IEnumerable<List<MoveCommand>> CrossCommands(List<List<MoveCommand>> sets)
        {
            if (sets.Count == 0)
                return sets;
            if (sets.Count == 1)
                return sets.First().Select(x => new List<MoveCommand>(new []{ x }));

            List<MoveCommand> head = sets.First();
            List<List<MoveCommand>> tail = CrossCommands(sets.Skip(1).ToList()).ToList();
            List<List<MoveCommand>> product = new List<List<MoveCommand>>();
            for (int i = 0; i < head.Count; i++)
            {
                for (int j = 0; j < tail.Count; j++)
                {
                    List<MoveCommand> item = new List<MoveCommand>();
                    item.Add(head[i]);
                    item = item.Concat(tail[j]).ToList();
                    product.Add(item);
                }
            }
            return product;
        }

        private IEnumerable<PossibleTurn> GenerateNextMoves(GameMap game, ISimulationStrategy strategy, List<CommandWithTurn> prefix, int deep, int deepLimit, int previousState)
        {
            return BaseGenerateNext(game, strategy, prefix, deep, deepLimit, previousState,
                (g) => CommandGenerator.Moves(g).Where(cmd => strategy.IsGoodPlaceForMove(g, cmd.Target)).ToList(),
                GenerateNextTrains);
        }

        private IEnumerable<PossibleTurn> GenerateNextTrains(GameMap game, ISimulationStrategy strategy, List<CommandWithTurn> prefix, int deep, int deepLimit, int previousState)
        {
            return BaseGenerateNext(game, strategy, prefix, deep, deepLimit, previousState,
                (g) => CommandGenerator.Trains(g).Where(cmd => strategy.IsGoodPlaceForTrain(g, cmd.Target)).ToList(),
                (g, oldStrategy, p, d, dlimit, s) =>
                {
                    if (d + 1 > dlimit || !oldStrategy.IsGoodTurnForContinue(g, new PossibleTurn{Commands = p, Rate = s}))
                        return new List<PossibleTurn>();
                    g.UpTurn();
                    var newStrategy = createStrategy();
                    newStrategy.StartSimulate(game);
                    var turns = GenerateNextMoves(g, newStrategy, p, d + 1, dlimit, s);
                    g.DownTurn();
                    return turns;
                });
        }

        private IEnumerable<PossibleTurn> BaseGenerateNext(GameMap game, ISimulationStrategy strategy, List<CommandWithTurn> prefix, int deep, int deepLimit, int previousState,
            Func<GameMap, List<ICommand>> generateNext,
            Func<GameMap, ISimulationStrategy, List<CommandWithTurn>, int, int, int, IEnumerable<PossibleTurn>> continuation)
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
                var withNext = prefix.Concat(new[] { new CommandWithTurn{Command = next, TurnDeep = deep} }).ToList();
                var rate = strategy.RateGame(game);
                if (strategy.HasImprove(previousState, rate))
                {
                    if (deep == deepLimit)
                        yield return new PossibleTurn { Rate = rate, Commands = withNext};
                    chains.AddRange(BaseGenerateNext(game, strategy, withNext, deep, deepLimit, rate, generateNext, continuation));
                }
                next.Unapply(game);
            }

            foreach (var chain in chains)
                yield return chain;
        }
    }
}