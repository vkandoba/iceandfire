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
//            var slaves = IceAndFire.game.MyUnits.Where(x => x.Level == 1).Count();
//            var allowSpot = placesForTrain.FirstOrDefault(p => IceAndFire.game.MineSpots.Contains(p));
//            if (slaves > 3 && !ReferenceEquals(allowSpot, null))
//            {
//                if (IceAndFire.game.MyGold > IceAndFire.MINE_BUILD_COST + IceAndFire.TRAIN_COST_LEVEL_1)
//                {
//                    Command.Train(1, allowSpot);
//                }
//            }

            Strategies.Base.TrainSlave(placesForTrain, 20);
        }

        public void ConstructBuildings()
        {
            Strategies.Base.ConstructBuildings();
        }
    }
}