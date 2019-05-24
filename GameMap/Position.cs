using System;
using System.Linq;

namespace IceAndFire
{
    public class Position
    {
        protected bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public int X;
        public int Y;

        public static implicit operator Position(ValueTuple<int, int> cell) => new Position
        {
            X = cell.Item1,
            Y = cell.Item2
        };

        public Position[] Area4()
        {
            var ns = new Position[] {
                (X+1, Y),
                (X-1, Y),
                (X, Y-1),
                (X, Y+1)
            };
            return ns.Where(n => n.X >= 0 && n.X < 12 && n.Y >= 0 && n.Y < 12).ToArray();
        }

        public Position[] Area8()
        {
            var ns = new Position[] {
                (X+1, Y-1),
                (X, Y-1),
                (X-1, Y-1),
                (X-1, Y),
                (X-1, Y+1),
                (X, Y+1),
                (X+1, Y),
                (X+1, Y+1)
            };
            return ns.Where(n => n.X >= 0 && n.X < 12 && n.Y >= 0 && n.Y < 12).ToArray();
        }

        public override string ToString() => $"({X},{Y})";

        public static bool operator ==(Position obj1, Position obj2) => obj1?.Equals(obj2) ?? ReferenceEquals(obj2, null);
        public static bool operator !=(Position obj1, Position obj2) => !(obj1 == obj2);

        public double MDistanceTo(Position p) => Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
    }
}