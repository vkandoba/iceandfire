namespace IceAndFire
{
    public enum Owner
    {
        ME = 1,
        NEUTRAL = 0,
        OPPONENT = 2
    }

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
    }
}