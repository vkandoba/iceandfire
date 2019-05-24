using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public static class AreaExtensions
    {
        public static Tile[] Area4(this GameMap gameMap, Tile tile) => Area4(gameMap, tile.Position);
        public static Tile[] Area8(this GameMap gameMap, Tile tile) => Area8(gameMap, tile.Position);

        public static Tile[] Area4(this GameMap gameMap, Position pos)
        {
            return pos.Area4().Select(p => gameMap.Map[p.X, p.Y]).ToArray();
        }

        public static Tile[] Area8(this GameMap gameMap, Position pos)
        {
            return pos.Area8().Select(p => gameMap.Map[p.X, p.Y]).ToArray();
        }
    }
}
