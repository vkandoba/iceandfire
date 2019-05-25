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
            base.ChangeMap(map);
            map.HoldPositions.Add(target);
        }

        public override string ToString() => $"{unit.Position} -> {target}";
    }
}