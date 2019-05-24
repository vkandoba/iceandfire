namespace IceAndFire
{
    public class Tile
    {
        public bool Active;
        public bool HasMineSpot;
        public bool IsWall;

        public int Owner = IceAndFire.NEUTRAL;

        public Position Position;

        public Unit Unit;
        public Building Building;

        public int X => Position.X;
        public int Y => Position.Y;

        public bool IsOwned => Owner == IceAndFire.ME;
        public bool IsOpponent => Owner == IceAndFire.OPPONENT;
        public bool IsNeutral => Owner == IceAndFire.NEUTRAL;

        public override string ToString() => $"({X},{Y})";
    }
}