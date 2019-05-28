using System;
using System.Linq;
using IceAndFire;
using NUnit.Framework;

namespace iceandfiretests
{
    [TestFixture]
    public class CommandGeneratorTest
    {
        private GameMap gameMap;

        [SetUp]
        public void Init()
        {
            gameMap = Loader.Load(new GameMap(), MapTest.ExampleAttack);
            var print = gameMap.ShowMap();
            Console.WriteLine(print);
        }

        [Test]
        public void GenerarteMoveTest()
        {
            var moves = CommandGenerator.Moves(gameMap).ToArray();
            Console.WriteLine("moves");
            var print = gameMap.ShowMap(moves.Select(m => m.Target));
            Console.WriteLine(print);
            foreach (var move in moves)
            {
                Console.WriteLine(move);
            }
        }

        [Test]
        public void GenerarteTrainTest()
        {
            var trains = CommandGenerator.Trains(gameMap);
            var print = gameMap.ShowMap(trains.Select(m => m.Target));
            Console.WriteLine(print);
            foreach (var train in trains)
            {
                Console.WriteLine(train);
            }
        }
    }
}