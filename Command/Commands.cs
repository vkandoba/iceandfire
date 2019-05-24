namespace IceAndFire
{
    public static class Commands
    {
        public static MoveCommand Move(int id, Position position) => new MoveCommand(id, position);
        public static BuildCommand Build(BuildingType type, Position position) => new BuildCommand(type, position);
        public static TrainCommand Train(int level, Position position) => new TrainCommand(level, position);
    }
}