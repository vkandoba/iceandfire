namespace IceAndFire
{
    public interface ICommand
    {
        Position Target { get; }
        void Apply(Game game);
    }
}