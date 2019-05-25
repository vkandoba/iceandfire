namespace IceAndFire
{
    public class TrainCommand : BaseCommand
    {
        private readonly int level;

        public TrainCommand(int level, Position pos) : base(pos)
        {
            this.level = level;
        }

        protected override string MakeCmd() => $"TRAIN {level} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            base.ChangeMap(map);
            map.HoldGold += Unit.TrainCosts[level];
            map.HoldUpkeep += Unit.UpkeepCosts[level];
            map.HoldPositions.Add(target);
        }

        public override string ToString() => $"{level} -> {target}";
    }
}