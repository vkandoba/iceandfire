using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace IceAndFire
{
    public static class Serialize
    {
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

            byte[] bytes = null;
            using (var memory = new MemoryStream())
            {
                serializer.Serialize(memory, copy);
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

        public static GameMap Load(GameMap gameMap, string str)
        {
            using (var memory = new MemoryStream(Convert.FromBase64String(str)))
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Decompress))
                {
                    var copy = (Tile[][])serializer.Deserialize(gzip);
                    for (int x = 0; x < GameMap.WIDTH; x++)
                    {
                        for (int y = 0; y < GameMap.HEIGHT; y++)
                        {
                            gameMap.Map[x, y] = copy[x][y];
                        }
                    }
                }
            }

            return gameMap;
        }

    }
}