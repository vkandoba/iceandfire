namespace IceAndFire
{
    public static class Commands
    {
        public static void Wait() => new WaitCommand().Apply(IceAndFire.gameEngine);
        public static void Move(Unit unit, Position position) => new MoveCommand(unit, position).Apply(IceAndFire.gameEngine);
        public static void Build(BuildingType type, Position position) => new BuildCommand(type, position).Apply(IceAndFire.gameEngine);
        public static void Train(int level, Position position) => new TrainCommand(level, position).Apply(IceAndFire.gameEngine);
    }
}