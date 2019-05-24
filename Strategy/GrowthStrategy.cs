using System.Linq;

namespace IceAndFire
{
    public class GrowthStrategy : IStrategy
    {
        public void MoveUnits()
        {
            Strategies.Base.MoveUnits();
        }

        public void TrainUnits()
        {
            if (IceAndFire.game.MyGold >= IceAndFire.MINE_BUILD_COST &&
                IceAndFire.game.MyGold < IceAndFire.MINE_BUILD_COST + Unit.TrainCosts[1])
                return;

            var placesForTrain = BaseStrategy.PlacesForTrain();

            Strategies.Base.TrainBase(placesForTrain, GetSlaveTrainPlace, 1, 20);
        }

        private Position GetSlaveTrainPlace(Position[] places)
        {
            var cells = places.ToDictionary(p => p, p => IceAndFire.game.Area8(p)
                .Where(c => !c.IsOwned && !c.IsWall).Count());
            var maxCells = cells.Values.Max();
            return cells.Where(c => c.Value == maxCells).OrderByDescending(c => IceAndFire.game.MyHq.MDistanceTo(c.Key)).FirstOrDefault().Key;
        }

        public void ConstructBuildings()
        {
            Strategies.Base.ConstructMines();
        }
    }
}