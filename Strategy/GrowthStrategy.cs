using System.Linq;

namespace IceAndFire
{
    public class GrowthStrategy : IStrategy
    {
        public ICommand[] MoveUnits() => Strategies.Base.MoveUnits();
        public ICommand[] ConstructBuildings() => Strategies.Base.ConstructMines();

        public ICommand[] TrainUnits()
        {
            if (IceAndFire.game.MyGold >= IceAndFire.MINE_BUILD_COST &&
                IceAndFire.game.MyGold < IceAndFire.MINE_BUILD_COST + Unit.TrainCosts[1])
                return new ICommand[0];

            var placesForTrain = BaseStrategy.PlacesForTrain();

            var train = Strategies.Base.TrainBase(placesForTrain, GetSlaveTrainPlace, 1, 20);
            return train == null ? new ICommand[0] : new []{train};
        }

        private Position GetSlaveTrainPlace(Position[] places)
        {
            var cells = places.ToDictionary(p => p, p => IceAndFire.game.Area8(p)
                .Where(c => !c.IsOwned).Count());
            var maxCells = cells.Values.Max();
            return cells.Where(c => c.Value == maxCells).OrderByDescending(c => IceAndFire.game.MyHq.MDistanceTo(c.Key)).FirstOrDefault().Key;
        }
    }
}