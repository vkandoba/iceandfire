namespace IceAndFire
{
    public static class Commands
    {
        public static void Wait() => new WaitCommand().Execute(IceAndFire.gameEngine);
        public static void Move(Unit unit, Position position) => new MoveCommand(unit, position).Execute(IceAndFire.gameEngine);
        public static void Build(BuildingType type, Position position) => new BuildCommand(type, position).Execute(IceAndFire.gameEngine);
        public static void Train(int level, Position position) => new TrainCommand(level, position).Execute(IceAndFire.gameEngine);
    }
}