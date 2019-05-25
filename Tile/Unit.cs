using System;

namespace IceAndFire
{
    [Serializable]
    public class Unit : Entity
    {

        public static readonly int[] UpkeepCosts = { 0, 1, 4, 20 };
        public static readonly int[] TrainCosts = { 0, 10, 20, 30 };

        public int Id;
        public int Level;
        public int Upkeep => UpkeepCosts[Level];

        public override string ToString() => $"Unit => {base.ToString()} Id: {Id} Level: {Level}";
    }
}   