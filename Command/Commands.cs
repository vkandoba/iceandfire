namespace IceAndFire
{
    public static class Commands
    {
        public static void Wait() => new WaitCommand().Apply(IceAndFire.gameEngine);
        public static void Move(int id, Position position) => new MoveCommand(id, position).Apply(IceAndFire.gameEngine);
        public static void Build(BuildingType type, Position position) => new BuildCommand(type, position).Apply(IceAndFire.gameEngine);
        public static void Train(int level, Position position) => new TrainCommand(level, position).Apply(IceAndFire.gameEngine);
    }
}