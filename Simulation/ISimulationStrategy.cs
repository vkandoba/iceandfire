using System.Collections.Generic;

namespace IceAndFire
{
    public interface ISimulationStrategy
    {
        void StartSimulate(GameMap game);
        bool IsGoodPlaceForTrain(GameMap game, Position target);
        bool IsGoodPlaceForMove(GameMap game, Position target);
        bool IsGoodTurnForContinue(GameMap gaWme, TurnGenerator.PossibleTurn turn);
        IEnumerable<List<ICommand>> PrepareMoveCommand(GameMap game, IEnumerable<List<ICommand>> moveCommands);
        IEnumerable<ICommand> PreparTrainCommand(GameMap game, IEnumerable<ICommand> trainCommands);
        int RateGame(GameMap game);
        bool HasImprove(int previous, int next);
    }
}