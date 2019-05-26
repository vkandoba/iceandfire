using System;

namespace IceAndFire
{
    public class TrainCommand : BaseCommand
    {
        public readonly int Level;
        private Random rand = new Random();
        private Entity savedDestroy = null;
        private Unit trainedUnit = null;

        public TrainCommand(int level, Position pos) : base(pos)
        {
            this.Level = level;
        }

        protected override string MakeCmd() => $"TRAIN {Level} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            savedDestroy = map.DestroyOp(target);
            base.ChangeMap(map);

            map.Me.Gold -= Unit.TrainCosts[Level];
            map.Me.Upkeep += Unit.UpkeepCosts[Level];
            trainedUnit = new Unit {Id = rand.Next(50, 100), IsTouch = true, Level = Level, Owner = Owner.ME, Position = target};
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

            game.Me.Gold += Unit.TrainCosts[Level];
            game.Me.Upkeep -= Unit.UpkeepCosts[Level];
        }

        public override string ToString() => $"{Level} -> {target}";
    }
}