namespace IceAndFire
{
    public class MoveCommand : BaseCommand
    {
        public Unit Unit => unit;

        private readonly Unit unit;
        private readonly Position start;
        private Entity savedDestroy = null;

        public MoveCommand(Unit unit, Position target) : base(target)
        {
            this.unit = unit;
            this.start = unit.Position;
        }

        protected override string MakeCmd() => $"MOVE {unit.Id} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            savedDestroy = map.DestroyOp(target);
            base.ChangeMap(map);

            unit.IsTouch = true;
            var old = unit.Position;
            map.Map[old.X, old.Y].Unit = null;
            unit.Position = target;
            map.Map[target.X, target.Y].Unit = unit;
        }

        public override void Unapply(GameMap game)
        {
            game.Map[target.X, target.Y].Unit = null;

            game.UnDestroyOp(target, savedDestroy);
            savedDestroy = null;

            base.Unapply(game);
        }

        public override string ToString() => $"{unit.Id} {start} -> {target}";
    }
}