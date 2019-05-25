using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public static class Geometry
    {
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