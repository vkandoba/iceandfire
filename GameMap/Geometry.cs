using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public static class Geometry
    {

        public static int[,] DistancesFrom(GameMap map, Tile source)
        {
            var distance = new int[GameMap.WIDTH, GameMap.HEIGHT];
            var wave = MakeWave(map, _ => true, source);
            for (int x = 0; x < GameMap.WIDTH; x++)
            {
                for (int y = 0; y < GameMap.HEIGHT; y++)
                {
                    var tile = map.Map[x, y];
                    distance[x, y] = wave.ContainsKey(tile) ? wave[tile] : 0;
                }
            }

            return distance;
        }

        public static string ShowDistances(int[,] distances)
        {
            var str = new StringBuilder();
            str.Append($"   ");
            for (int x = 0; x < GameMap.HEIGHT; x++)
            {
                str.Append($"{x.ToString().PadRight(3)}");
            }
            str.AppendLine();
            str.AppendLine();

            for (int y = 0; y < GameMap.WIDTH; y++)
            {
                str.Append($"{y.ToString().PadRight(3)}");
                for (int x = 0; x < GameMap.WIDTH; x++)
                {
                    str.Append($"{distances[x, y].ToString().PadRight(3)}");
                }
                str.AppendLine();
            }

            return str.ToString();
        }

        public static Dictionary<Tile, int> MakeWave(
            GameMap map,
            Func<Tile, bool> isFree,
            Tile source)
        {
            var queue = new Queue<Tile>();
            var dist = new Dictionary<Tile, int>();
            dist[source] = 0;
            queue.Enqueue(source);
            do
            {
                var current = queue.Dequeue();
                var area = map.Area4[current];
                foreach (var c in area)
                {
                    if (isFree(c) && !dist.ContainsKey(c))
                    {
                        dist[c] = dist[current] + 1;
                        queue.Enqueue(c);
                    }
                }
            } while (queue.Any());

            return dist;
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