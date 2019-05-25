namespace IceAndFire
{
    public class MoveCommand : BaseCommand
    {
        private readonly int id;

        public MoveCommand(int id, Position pos) : base(pos)
        {
            this.id = id;
        }

        protected override string MakeCmd() => $"MOVE {id} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            base.ChangeMap(map);
            map.HoldPositions.Add(target);
        }
    }
}