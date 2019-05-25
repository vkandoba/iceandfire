using System;

namespace IceAndFire
{
    [Serializable]
    public class PlayerState
    {
        public Team Team;
        public int Gold;
        public int Income;
        public int Upkeep;

        public override string ToString()
        {
            return $"{Team.ToString().PadRight(4)} Gold: {Gold} Income: {Income} Upkeep: {Upkeep}";
        }
    }
}