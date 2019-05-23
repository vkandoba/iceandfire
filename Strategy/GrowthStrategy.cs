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
            if (IceAndFire.game.MyGold > IceAndFire.MINE_BUILD_COST)
            {
                var allowSpot = placesForTrain.FirstOrDefault(p => IceAndFire.game.MineSpots.Contains(p));
                if (!ReferenceEquals(allowSpot, null))
                    Command.Train(1, allowSpot);
            }

            Strategies.Base.TrainSlave(placesForTrain);
        }

        public void ConstructBuildings()
        {
            Strategies.Base.ConstructBuildings();
        }
    }
}