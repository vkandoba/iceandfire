using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace IceAndFire
{
    public static class Serialize
    {
        [Serializable]
        public class GameData
        {
            public Tile[][] MapCopy;
            public PlayerState MeState;
            public PlayerState OpState;
        }

        private static BinaryFormatter serializer = new BinaryFormatter();

        public static string Save(GameMap gameMap)
        {
            var copy = new Tile[GameMap.HEIGHT][];
            for (int x = 0; x < GameMap.WIDTH; x++)
            {
                copy[x] = new Tile[GameMap.WIDTH];
                for (int y = 0; y < GameMap.HEIGHT; y++)
                {
                    copy[x][y] = gameMap.Map[x, y];
                }
            }

            var data = new GameData {MapCopy = copy, MeState = gameMap.Me, OpState = gameMap.Opponent};

            byte[] bytes = null;
            using (var memory = new MemoryStream())
            {
                serializer.Serialize(memory, data);
                bytes = memory.ToArray();
            }

            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Compress))
                {
                    new MemoryStream(bytes).CopyTo(gzip);
                }
                return Convert.ToBase64String(memory.ToArray());
            }
        }
    }
}