namespace IceAndFire
{
    public interface ICommand
    {
        Position Target { get; }
        void Execute(Game game);
        void Apply(GameMap game);
        void Unapply(GameMap game);
    }
}