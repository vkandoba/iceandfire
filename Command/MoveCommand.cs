namespace IceAndFire
{
    public class MoveCommand : BaseCommand
    {
        private readonly Unit unit;

        public MoveCommand(Unit unit, Position target) : base(target)
        {
            this.unit = unit;
        }

        protected override string MakeCmd() => $"MOVE {unit.Id} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            DestroyOp(map, target);
            base.ChangeMap(map);

            var old = unit.Position;
            map.Map[old.X, old.Y].Unit = null;
            unit.Position = target;
        }

        public override string ToString() => $"{unit.Position} -> {target}";
    }
}