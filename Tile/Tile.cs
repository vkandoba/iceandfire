﻿using System;

namespace IceAndFire
{
    [Serializable]
    public class Tile
    {
        public bool Active;
        public bool HasMineSpot;
        public bool IsWall;

        [NonSerialized()]
        public bool IsUnderAttack;

        public Owner Owner = Owner.NEUTRAL;

        [NonSerialized()]
        public Position Position;

        public Unit Unit;
        public Building Building;

        public int X => Position.X;
        public int Y => Position.Y;

        public bool IsOwned => Owner == Owner.ME;
        public bool IsOpponent => Owner == Owner.OPPONENT;
        public bool IsNeutral => Owner == Owner.NEUTRAL;

        public override string ToString() => $"({X},{Y}, {ToChar()})";

        public string ToChar()
        {
            if (IsWall) return "x";

            if (Building == null && Unit == null || !Active)
                return Owner == Owner.NEUTRAL || !Active ? "0" : ((int)Owner).ToString();

            if (Building?.IsHq == true) return "h";
            if (Building?.IsTower == true) return "t";
            if (Building?.IsMine == true) return "m";

            if (Unit != null)
                return IsOpponent ? "e" : (Unit.Level == 1 ? "u" : (Unit.Level == 2 ? "s" : "k"));

            if (HasMineSpot) return "g";

            return "?";
        }
    }
}