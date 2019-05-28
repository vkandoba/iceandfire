using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using IceAndFire;

namespace iceandfiretests
{
    public static class Loader
    {

        private static BinaryFormatter serializer = new BinaryFormatter();

        public static GameMap Load(GameMap gameMap, string str)
        {
            using (var memory = new MemoryStream(Convert.FromBase64String(str)))
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Decompress))
                {
                    var data = (Serialize.GameData)serializer.Deserialize(gzip);
                    var copy = data.MapCopy;
                    gameMap.Clear();
                    for (int x = 0; x < GameMap.WIDTH; x++)
                    {
                        for (int y = 0; y < GameMap.HEIGHT; y++)
                        {
                            var tile = copy[x][y];
                            tile.Position = (x, y);
                            gameMap.Map[x, y] = tile;
                            if (!tile.IsWall)
                            {
                                if (tile.IsOwned && tile.Active)
                                    gameMap.MyPlaces++;

                                if (tile.IsOpponent && tile.Active)
                                    gameMap.OpPlaces++;

                                if (tile.Unit != null)
                                    gameMap.Units.Add(tile, tile.Unit);
                                if (tile.Building != null)
                                    gameMap.Buildings.Add(tile, tile.Building);
                            }
                        }
                    }
                    //update areas
                    gameMap.UpdateAreas();
                    gameMap.UpdateDistances();


                    foreach (var tile in gameMap.Buildings.Keys)
                    {
                        var b = gameMap.Buildings[tile];
                        if (tile.IsOpponent && b.IsTower)
                        {
                            var towerArea = gameMap.Area4[tile];
                            for (int a = 0; a < towerArea.Length; a++)
                            {
                                towerArea[a].IsUnderAttack = true;
                            }
                        }
                    }

                    gameMap.Me = data.MeState;
                    gameMap.Opponent = data.OpState;

                    // Usefull for symmetric AI
                    //if (data.MeState.Team == Team.Ice)
                    //{
                    //    gameMap.MyPlaces.Reverse();
                    //    gameMap.OpPositions.Reverse();
                    //    gameMap.NeutralPositions.Reverse();
                    //}
                }
            }

            return gameMap;
        }

    }
}