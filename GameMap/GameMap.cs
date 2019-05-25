using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace IceAndFire
{
    public class GameMap
    {
        public const int WIDTH = 12;
        public const int HEIGHT = 12;

        public Team MyTeam;
        public int MyGold => ActualGold - HoldGold;
        public int ActualGold;
        public int ActuaUpkeep;
        public int HoldGold;
        public int HoldUpkeep;
        public int MyIncome;
        public int MyUpkeep => ActuaUpkeep - HoldUpkeep;

        public int OpponentIncome;
        public int OpponentGold;

        public readonly Tile[,] Map = new Tile[WIDTH, WIDTH];

        public readonly List<Building> Buildings = new List<Building>();
        public List<Unit> Units = new List<Unit>();

        public Position MyHq => MyTeam == Team.Fire ? (0, 0) : (11, 11);
        public Position OpponentHq => MyTeam == Team.Fire ? (11, 11) : (0, 0);

        public List<Tile> MyPositions = new List<Tile>();
        public List<Tile> OpponentPositions = new List<Tile>();
        public List<Tile> NeutralPositions = new List<Tile>();
        public HashSet<Position> HoldPositions = new HashSet<Position>();

        public List<Unit> MyUnits => Units.Where(u => u.IsOwned).ToList();
        public List<Unit> OpponentUnits => Units.Where(u => u.IsOpponent).ToList();

        public List<Position> MineSpots = new List<Position>();

        public Tile[] PlacesForTrain(int level = 1)
        {
            var territory = MyPositions.Concat(MyPositions.SelectMany(this.Area4)).Distinct();
            return territory.Where(t => AllowMove(t, level)).ToArray();
        }

        public Tile[] PlacesForTower()
        {
            return MyPositions.Where(AllowBuilldTower).ToArray();
        }

        public bool AllowMove(Tile tile, int level = 1)
        {
            return !(HoldPositions.Contains(tile.Position) ||
                     (tile.Building != null && tile.IsOwned) ||
                     tile.Unit?.IsOwned == true || (tile.Unit?.Level ?? 0) >= level);
        }

        private bool AllowBuilldTower(Tile tile)
        {
            return tile.Unit == null && tile.Building == null && !tile.HasMineSpot && !HoldPositions.Contains(tile.Position);
        }

        public bool HasMenace()
        {
            var around = OpponentUnits.SelectMany(op => this.Area8(op.Position));
            return around.Where(p => p.IsOwned).Any();
        }

        public string Serialize()
        {
            var copy = new Tile[WIDTH][];
            for (int x = 0; x < WIDTH; x++)
            {
                copy[x] = new Tile[HEIGHT];
                for (int y = 0; y < HEIGHT; y++)
                {
                    copy[x][y] = Map[x, y];
                }
            }

            byte[] bytes = null;
            using (var memory = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memory, copy);
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

        public GameMap Deserialize(string str)
        {
            using (var memory = new MemoryStream(Convert.FromBase64String(str)))
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Decompress))
                {
                    var binaryFormatter = new BinaryFormatter();
                    var copy = (Tile[][])binaryFormatter.Deserialize(gzip);
                    for (int x = 0; x < WIDTH; x++)
                    {
                        for (int y = 0; y < HEIGHT; y++)
                        {
                            Map[x, y] = copy[x][y];
                        }
                    }
                }
            }

            return this;
        }

        public string ShowMap()
        {
            var str = new StringBuilder();
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    str.Append($"{Map[x, y].ToChar()} ");
                }

                str.AppendLine();
            }

            return str.ToString();
        }

        public static Position[] FindPathInternal(Func<Position, bool> isFree, 
            Position source,
            Position target)
        {
            var queue = new Queue<Position>();
            var path = new Dictionary<Position, Position>();
            path[source] = source;
            queue.Enqueue(source);
            do
            {
                var pos = queue.Dequeue();
                if (pos == target)
                    return BuidlPath(path, target, source);
                var area = pos.Area4().Where(isFree).Where(c => !path.ContainsKey(c));
                foreach (var c in area)
                {
                    path[c] = pos;
                    queue.Enqueue(c);
                }
            } while (queue.Any());

            return new Position[0];
        }

        private static Position[] BuidlPath(Dictionary<Position, Position> links, Position target, Position source)
        {
            var path = new List<Position>();
            var current = target;
            do
            {
                path.Add(current);
                current = links[current];
            } while (current != source);

            path.Reverse();
            return path.ToArray();
        }
    }
}
