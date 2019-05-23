using System.Linq;

namespace IceAndFire
{
    public class GrowthStrategy : IStrategy
    {
        public void MoveUnits()
        {
            var slaves = IceAndFire.game.MyUnits.Where(u => u.Level == 1).ToArray();
            foreach (var unit in slaves)
            {
                var om = Strategies.Base.GetOccupationMove(unit) ?? IceAndFire.game.OpponentHq;
                Command.Move(unit.Id, om);
            }
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

            Strategies.Base.TrainSlave(placesForTrain);
        }

        public void ConstructBuildings()
        {
            Strategies.Base.ConstructBuildings();
        }
    }
}