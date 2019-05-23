﻿namespace IceAndFire
{
    public class Unit : Entity
    {

        public static readonly int[] UpkeepCosts = { 0, 1, 4, 20 };

        public int Id;
        public int Level;
        public int Upkeep => UpkeepCosts[Level];

        public override string ToString() => $"Unit => {base.ToString()} Id: {Id} Level: {Level}";
    }
}