using System;
using System.Text;
using IceAndFire;
using NUnit.Framework;

namespace iceandfiretests
{
    [TestFixture]
    public class FindPathTest
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
        public void TestFind()
        {
            var path = Geometry.FindPathInternal((p) => true, (0, 0), (1, 1));
            Console.WriteLine(path.Length);
            foreach (var p in path)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void TestWave()
        {
            var distances = Geometry.DistancesFrom(gameMap, gameMap.MyHq);

            Console.WriteLine(Geometry.ShowDistances(distances));
        }
    }
}