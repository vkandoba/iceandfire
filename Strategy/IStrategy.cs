namespace IceAndFire
{
    public interface IStrategy
    {
        ICommand[] MoveUnits();
        ICommand[] TrainUnits();
        ICommand[] ConstructBuildings();
    }
}