namespace IceAndFire
{
    public class TrainCommand : Command
    {
        private readonly int level;
        private readonly Position pos;

        public TrainCommand(int level, Position pos)
        {
            this.level = level;
            this.pos = pos;
        }

        protected override string MakeCmd() => $"TRAIN {level} {pos.X} {pos.Y}";

        protected override void ChangeMap(GameMap map)
        {
            MarkPosition(map, pos);
            map.HoldGold += Unit.TrainCosts[level];
            map.HoldUpkeep += Unit.UpkeepCosts[level];
            map.HoldPositions.Add(pos);
        }
    }
}