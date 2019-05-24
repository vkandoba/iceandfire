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

        protected override void MakeHolds(GameMap map)
        {
            map.HoldPositions.Add(pos);
        }
    }
}