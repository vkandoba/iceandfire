using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class DefenseStrategy : IStrategy
    {
        public void MoveUnits() => Strategies.Base.MoveUnits();

        public void TrainUnits()
        {
            TrainKiller();
        }

        public bool TrainKiller()
        {
            var placesForTrain = IceAndFire.game.PlacesForTrain();

            var opKiller = placesForTrain
                .Where(t => t.Unit != null)
                .FirstOrDefault(t => t.Unit.IsOpponent && t.Unit.Level == 3);
            if (opKiller != null && IceAndFire.game.MyGold >= Unit.TrainCosts[3])
            {
                Commands.Train(3, opKiller.Position);
                return true;
            }

            return Strategies.Base.TrainKiller(placesForTrain);
        }

        public void ConstructBuildings()
        {
            while (ConstructTower())
            {
            }
            Strategies.Base.ConstructMines();
            var placesForTrain = IceAndFire.game.PlacesForTrain();
            var neutralPlaces = placesForTrain.Where(c => IceAndFire.game.Map[c.X, c.Y].IsNeutral).ToArray();
            if (neutralPlaces.Any())
                Strategies.Base.TrainSlave(neutralPlaces, 20);
            while (TrainKiller())
            {
                
            }
        }

        public bool ConstructTower()
        {
            if (IceAndFire.game.MyGold < IceAndFire.TOWER_BUILD_COST)
                return false;

            var places = IceAndFire.game.PlacesForTower();
            var placesUnderAttact =
                places.ToDictionary(p => p, p => IceAndFire.game.Area8(p).Count(c => c.Unit?.IsOpponent == true));
            if (placesUnderAttact.Values.Max() > 0)
            {
                var towerPlace = placesUnderAttact.OrderByDescending(p => p.Value).First().Key;
                Commands.Build(BuildingType.Tower, towerPlace.Position);
                return true;
            }

            return false;
        }
    }
}
