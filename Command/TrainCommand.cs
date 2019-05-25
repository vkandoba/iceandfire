using System;

namespace IceAndFire
{
    public class TrainCommand : BaseCommand
    {
        private readonly int level;
        private Random rand = new Random();

        public TrainCommand(int level, Position pos) : base(pos)
        {
            this.level = level;
        }

        protected override string MakeCmd() => $"TRAIN {level} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            map.DestroyOp(target);
            map.MarkPositionIsMe(target);

            map.Me.Gold -= Unit.TrainCosts[level];
            map.Me.Upkeep += Unit.UpkeepCosts[level];
            var unit = new Unit {Id = rand.Next(50, 100), IsTouch = true, Level = level, Owner = Owner.ME, Position = target};
            map.Units.Add(unit);
            map.Map[target.X, target.Y].Unit = unit;
        }

        public override string ToString() => $"{level} -> {target}";
    }
}