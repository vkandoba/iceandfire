namespace IceAndFire
{
    public interface ISimulationStrategy
    {
        void StartSimulate(GameMap game);
        bool IsGoodPlace(GameMap game, Position target);
        int RateGame(GameMap game);
        bool HasImprove(int previous, int next);
    }
}