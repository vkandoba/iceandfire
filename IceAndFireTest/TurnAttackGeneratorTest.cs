using System;
using System.Linq;
using IceAndFire;
using NUnit.Framework;

namespace iceandfiretests
{
    [TestFixture]
    public class TurnAttackGeneratorTest
    {
        private GameMap gameMap;
        private TurnGenerator turnGenerator;

        [SetUp]
        public void Init()
        {
            gameMap = Loader.Load(new GameMap(), MapTest.ExampleAttack1);
            var print = gameMap.ShowMap();
            Console.WriteLine(print);
            turnGenerator = new TurnGenerator(() => new AttackSimulationStrategy());
        }

        [Test]
        public void GenerarteTurnTest()
        {
            var deep = 1;
            var turns = IceAndFire.IceAndFire.Measure("generate", 
                () => turnGenerator.Turns(gameMap, deep).ToArray());
            PrintTurns(turns);
        }

        [Test]
        public void ForProfile()
        {
            var deep = 3;
            turnGenerator.Turns(gameMap, deep).ToArray();
        }

        [Test]
        public void GenerarteOneTurnMovesTest()
        {
            var turns = IceAndFire.IceAndFire.Measure("generate", 
                () => turnGenerator.OneTurnMoves(gameMap).ToArray());
            PrintTurns(turns);
        }

        [Test]
        public void GenerarteOneTurnTrainTest()
        {
            var turns = IceAndFire.IceAndFire.Measure("generate", 
                () => turnGenerator.OneTurnTrainMoves(gameMap).ToArray());

            //foreach (var turn in turns.OrderByDescending(t => t.Rate))
            //{
            //    Console.WriteLine($"{new AttackSimulationStrategy().RateGame(gameMap)}");
            //    foreach (var command in turn.Commands)
            //    {
            //        command.Command.Apply(gameMap);
            //    }
            //    Console.WriteLine($"{new AttackSimulationStrategy().RateGame(gameMap)}");
            //}
            PrintTurns(turns);
        }

        public void PrintTurns(TurnGenerator.PossibleTurn[] turns)
        {
            Console.WriteLine(turns.Length);
            foreach (var turn in turns)
            {
                Console.WriteLine($"{turn}");
            }
        }
    }
}