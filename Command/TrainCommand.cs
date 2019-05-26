using System;

namespace IceAndFire
{
    public class TrainCommand : BaseCommand
    {
        private readonly int level;
        private Random rand = new Random();
        private Entity savedDestroy = null;
        private Unit trainedUnit = null;

        public TrainCommand(int level, Position pos) : base(pos)
        {
            this.level = level;
        }

        protected override string MakeCmd() => $"TRAIN {level} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            savedDestroy = map.DestroyOp(target);
            base.ChangeMap(map);

            map.Me.Gold -= Unit.TrainCosts[level];
            map.Me.Upkeep += Unit.UpkeepCosts[level];
            trainedUnit = new Unit {Id = rand.Next(50, 100), IsTouch = true, Level = level, Owner = Owner.ME, Position = target};
            map.Map[target.X, target.Y].Unit = trainedUnit;
            map.Units.Add(map.Map[target.X, target.Y], trainedUnit);
        }

        public override void Unapply(GameMap game)
        {
            game.Map[target.X, target.Y].Unit = null;
            game.Units.Remove(game.Map[target.X, target.Y]);
            trainedUnit = null;

            game.UnDestroyOp(target, savedDestroy);
            savedDestroy = null;
            base.Unapply(game);

            game.Me.Gold += Unit.TrainCosts[level];
            game.Me.Upkeep -= Unit.UpkeepCosts[level];
        }

        public override string ToString() => $"{level} -> {target}";
    }
}