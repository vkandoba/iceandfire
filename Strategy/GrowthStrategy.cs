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
            var placesForTrain = BaseStrategy.PlacesForTrain();

            Strategies.Base.TrainSlave(placesForTrain, 20);
        }

        public void ConstructBuildings()
        {
            Strategies.Base.ConstructMines();
        }
    }
}