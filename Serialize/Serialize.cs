using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace IceAndFire
{
    public static class Serialize
    {
        [Serializable]
        private class GameData
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

        public static GameMap Load(GameMap gameMap, string str)
        {
            using (var memory = new MemoryStream(Convert.FromBase64String(str)))
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Decompress))
                {
                    var data = (GameData)serializer.Deserialize(gzip);
                    var copy = data.MapCopy;
                    gameMap.Clear();
                    for (int x = 0; x < GameMap.WIDTH; x++)
                    {
                        for (int y = 0; y < GameMap.HEIGHT; y++)
                        {
                            var tile = copy[x][y];
                            gameMap.Map[x, y] = tile;
                            if (!tile.IsWall)
                            {
                                if (tile.IsOwned)
                                    gameMap.MyPositions.Add(tile);
                                if (tile.IsOpponent)
                                    gameMap.OpPositions.Add(tile);
                                if (tile.IsNeutral)
                                    gameMap.NeutralPositions.Add(tile);

                                if (tile.Unit != null)
                                    gameMap.Units.Add(tile.Unit);
                                if (tile.Building != null)
                                    gameMap.Buildings.Add(tile.Building);
                            }
                        }
                    }

                    gameMap.Me = data.MeState;
                    gameMap.Opponent = data.OpState;

                    // Usefull for symmetric AI
                    if (data.MeState.Team == Team.Ice)
                    {
                        gameMap.MyPositions.Reverse();
                        gameMap.OpPositions.Reverse();
                        gameMap.NeutralPositions.Reverse();
                    }
                }
            }

            return gameMap;
        }

    }
}