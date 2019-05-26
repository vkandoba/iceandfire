using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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
            public CommandWithTurn[] Commands { get; set; }
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

        private class GenerateState
        {
            public CommandWithTurn[] Prefix;
            public ISimulationStrategy Strategy;
            public int Deep;
            public int PreviousRate;
        }

        public TurnGenerator(Func<ISimulationStrategy> createStrategy)
        {
            this.createStrategy = createStrategy;
        }

        public IEnumerable<PossibleTurn> Turns(GameMap game, int deep = 1)
        {
            var strategy = createStrategy();
            strategy.StartSimulate(game);
            var state = new GenerateState
            {
                Deep = 1,
                Strategy = strategy,
                Prefix = new List<CommandWithTurn>(),
                PreviousRate = strategy.RateGame(game)
            };
            return GenerateNextMoves(game, deep, state);
        }

        private IEnumerable<List<CommandWithTurn>> OneTurnMovesInternal(GameMap game, GenerateState state)
        {
            var unitMoves = CommandGenerator.MovesByUnit(game, p => state.Strategy.IsGoodPlaceForMove(game, p));
            var prepared = state.Strategy.PrepareMoveCommand(game, unitMoves).ToArray();
            return CrossCommands(prepared)
                .Where(x => x.Select(c => c.Target).Distinct().Count() == x.Length)
                .Select(commands => commands.Select(c => new CommandWithTurn {Command = c, TurnDeep = state.Deep}).ToList());
        }

        private IEnumerable<PossibleTurn> GenerateNextMoves(GameMap game, int deepLimit, GenerateState state)
        {
            var currentTurnMoves = OneTurnMovesInternal(game, state).ToList();
            var result = new List<PossibleTurn>();
            if (!currentTurnMoves.Any())
                result.AddRange(GenerateNextTrains(game, deepLimit, state));
            foreach (var moves in currentTurnMoves)
            {
                foreach (var command in moves)
                {
                    command.Command.Apply(game);
                }

                var rate = state.Strategy.RateGame(game);
                if (state.Strategy.HasImprove(state.PreviousRate, rate))
                {
                    var withNext = state.Prefix.Concat(moves).ToList();
                    var nextState = new GenerateState
                    {
                        Deep = state.Deep,
                        Strategy = state.Strategy,
                        PreviousRate = rate,
                        Prefix = withNext
                    };
                    var continuations = GenerateNextTrains(game, deepLimit, nextState);
                    result.AddRange(continuations);
                }

                moves.Reverse();
                foreach (var command in moves)
                {
                    command.Command.Unapply(game);
                }

            }

            foreach (var possibleTurn in result)
            {
                yield return possibleTurn;
            }
        }

        private IEnumerable<PossibleTurn> GenerateNextTrains(GameMap game, int deepLimit, GenerateState state)
        {
            return BaseGenerateNext(game, deepLimit, state,
                (g) => state.Strategy.PreparTrainCommand(game, CommandGenerator.Trains(g).Where(cmd => state.Strategy.IsGoodPlaceForTrain(g, cmd.Target)).ToList()).ToList(),
                (g, dlimit, s) =>
                {
                    var possibleTurn = new PossibleTurn{Commands = s.Prefix, Rate = s.PreviousRate};
                    if (s.Deep == dlimit)
                        return new List<PossibleTurn>{possibleTurn};
                    if (s.Deep > dlimit || !s.Strategy.IsGoodTurnForContinue(g, possibleTurn))
                        return new List<PossibleTurn>();
                    g.UpTurn();
                    var newStrategy = createStrategy();
                    newStrategy.StartSimulate(game);
                    var newState = new GenerateState
                    {
                        Prefix = s.Prefix,
                        Deep = s.Deep + 1,
                        Strategy = newStrategy,
                        PreviousRate = s.PreviousRate
                    };
                    var turns = GenerateNextMoves(g, dlimit, newState);
                    g.DownTurn();
                    return turns;
                });
        }

        private IEnumerable<PossibleTurn> BaseGenerateNext(GameMap game, int deepLimit, GenerateState state,
            Func<GameMap, List<ICommand>> generateNext,
            Func<GameMap, int, GenerateState, IEnumerable<PossibleTurn>> continuation)
        {
            var nexts = generateNext(game);
            if (!nexts.Any())
                foreach (var turn in continuation(game, deepLimit, state))
                {
                    yield return turn;
                }

            var chains = new List<PossibleTurn>();
            foreach (var next in nexts)
            {
                next.Apply(game);
                var rate = state.Strategy.RateGame(game);
                if (state.Strategy.HasImprove(state.PreviousRate, rate))
                {
                    var withNext = state.Prefix.Concat(new[] { new CommandWithTurn { Command = next, TurnDeep = state.Deep } }).ToList();
                    var nextState = new GenerateState
                    {
                        Deep = state.Deep,
                        Strategy = state.Strategy,
                        PreviousRate = rate,
                        Prefix = withNext
                    };
                    chains.AddRange(BaseGenerateNext(game, deepLimit, nextState, generateNext, continuation));
                }
                next.Unapply(game);
            }

            foreach (var chain in chains)
                yield return chain;
        }

        private ICommand[][] CrossCommands(ICommand[][] sets)
        {
            if (sets.Length == 0)
                return sets;
            if (sets.Length == 1)
            {
                return sets[0].Select(x => new[] { x }).ToArray();
            }

            ICommand[] head = sets[0];
            ICommand[][] tail = CrossCommands(sets.Skip(1).ToArray());
            List<ICommand[]> product = new List<ICommand[]>();
            for (int i = 0; i < head.Length; i++)
            {
                for (int j = 0; j < tail.Length; j++)
                {
                    ICommand[] item = new ICommand[tail[j].Length + 1];
                    item[0] = head[i];
                    Array.Copy(tail[j], 0, item, 1, tail[j].Length);
                    product.Add(item);
                }
            }
            return product.ToArray();
        }

        public IEnumerable<PossibleTurn> OneTurnMoves(GameMap game)
        {
            var strategy = createStrategy();
            strategy.StartSimulate(game);
            var state = new GenerateState
            {
                Deep = 1,
                Strategy = strategy,
                Prefix = new List<CommandWithTurn>(),
                PreviousRate = strategy.RateGame(game)
            };
            return GenerateNextMoves(game, 1, state);
        }

        public IEnumerable<PossibleTurn> OneTurnTrainMoves(GameMap game)
        {
            var strategy = createStrategy();
            strategy.StartSimulate(game);
            var state = new GenerateState
            {
                Deep = 1,
                Strategy = strategy,
                Prefix = new List<CommandWithTurn>(),
                PreviousRate = strategy.RateGame(game)
            };
            return GenerateNextTrains(game, 1, state);
        }
    }
}