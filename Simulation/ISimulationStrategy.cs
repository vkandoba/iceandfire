namespace IceAndFire
{
    public interface ISimulationStrategy
    {
        void StartSimulate(GameMap game);
        bool IsGoodPlaceForTrain(GameMap game, Position target);
        bool IsGoodPlaceForMove(GameMap game, Position target);
        int RateGame(GameMap game);
        bool HasImprove(int previous, int next);
    }
}