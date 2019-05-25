using System;

namespace IceAndFire
{
    [Serializable]
    public class Entity
    {
        public Owner Owner; 
        public Position Position;

        public bool IsOwned => Owner == Owner.ME;
        public bool IsOpponent => Owner == Owner.OPPONENT;

        public int X => Position.X;
        public int Y => Position.Y;

        public override string ToString() => $"Owner: {Owner} Position: {Position}";
    }
}