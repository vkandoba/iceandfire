namespace IceAndFire
{
    public class Entity
    {
        public int Owner;
        public Position Position;

        public bool IsOwned => Owner == IceAndFire.ME;
        public bool IsOpponent => Owner == IceAndFire.OPPONENT;

        public int X => Position.X;
        public int Y => Position.Y;

        public override string ToString() => $"Owner: {Owner} Position: {Position}";
    }
}