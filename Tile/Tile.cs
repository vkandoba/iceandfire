using System;

namespace IceAndFire
{
    [Serializable]
    public class Tile
    {
        public bool Active;
        public bool HasMineSpot;
        public bool IsWall;

        public Owner Owner = Owner.NEUTRAL;

        public Position Position;

        public Unit Unit;
        public Building Building;

        public int X => Position.X;
        public int Y => Position.Y;

        public bool IsOwned => Owner == Owner.ME;
        public bool IsOpponent => Owner == Owner.OPPONENT;
        public bool IsNeutral => Owner == Owner.NEUTRAL;

        public override string ToString() => $"({X},{Y})";

        public string ToChar()
        {
            if (IsWall) return "x";

            if (Building == null && Unit == null)
                return Owner == Owner.NEUTRAL || !Active ? "0" : ((int)Owner).ToString();

            if (Building?.IsHq == true) return "h";
            if (Building?.IsTower == true) return "t";
            if (Building?.IsMine == true) return "m";

            if (Unit != null)
                return Unit.IsTouch ? "f" : "u";

            if (HasMineSpot) return "g";

            return "?";
        }
    }
}