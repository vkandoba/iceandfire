using System;
using System.Linq;
using IceAndFire;
using NUnit.Framework;

namespace iceandfiretests
{
    [TestFixture]
    public class TurnGrothGeneratorTest
    {
        private GameMap gameMap;
        private TurnGenerator turnGenerator;

        [SetUp]
        public void Init()
        {
            gameMap = Loader.Load(new GameMap(), MapTest.ExampleMap1);
            var print = gameMap.ShowMap();
            Console.WriteLine(print);
            turnGenerator = new TurnGenerator(() => new GrowthSimulationStrategy());
        }

        [Test]
        public void GenerarteTurnTest()
        {
            var deep = 3;
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