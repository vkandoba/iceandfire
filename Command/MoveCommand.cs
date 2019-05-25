namespace IceAndFire
{
    public class MoveCommand : Command
    {
        private readonly int id;
        private readonly Position pos;

        public MoveCommand(int id, Position pos)
        {
            this.id = id;
            this.pos = pos;
        }

        protected override string MakeCmd() => $"MOVE {id} {pos.X} {pos.Y}";

        protected override void ChangeMap(GameMap map)
        {
            map.HoldPositions.Add(pos);
            MarkPosition(map, pos);
        }
    }

    public class WaitCommand : Command
    {
        protected override string MakeCmd() => "WAIT";

        protected override void ChangeMap(GameMap map)
        {
        }
    }
}