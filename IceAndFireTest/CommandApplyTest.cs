using System;
using System.Linq;
using IceAndFire;
using NUnit.Framework;

namespace iceandfiretests
{
    [TestFixture]
    public class CommandApplyTest
    {
        private GameMap gameMap;

        [SetUp]
        public void Init()
        {
            gameMap = Loader.Load(new GameMap(), MapTest.ExampleMap);
            var print = gameMap.ShowMap();
            Console.WriteLine(print);
        }

        [Test]
        public void MoveApplyTest()
        {
            var move = CommandGenerator.Moves(gameMap)
                                        .Where(m => !gameMap.Map[m.Target.X, m.Target.Y].IsOwned)
                                        .FirstOrDefault();
            Console.WriteLine(move);
            move.Apply(gameMap);
            var print = gameMap.ShowMap();
            Console.WriteLine(print);
        }

        [Test]
        public void TrainApplyTest()
        {
            var train = CommandGenerator.Trains(gameMap)
                                        .Where(m => !gameMap.Map[m.Target.X, m.Target.Y].IsOwned)
                                        .FirstOrDefault();
            Console.WriteLine(train);
            train.Apply(gameMap);
            var print = gameMap.ShowMap();
            Console.WriteLine(print);
        }
    }
}