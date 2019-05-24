using System;
using System.Linq;

namespace IceAndFire
{
    public class DefenseStrategy : IStrategy
    {
        public void MoveUnits()
        {
            Strategies.Base.MoveUnits();
        }

        public void TrainUnits()
        {
            TrainKiller();
        }

        public bool TrainKiller()
        {
            var placesForTrain = BaseStrategy.PlacesForTrain();

            var opKiller = placesForTrain
                .Select(p => IceAndFire.game.Map[p.X, p.Y])
                .Where(t => t.Unit != null)
                .FirstOrDefault(t => t.Unit.IsOpponent && t.Unit.Level == 3);
            if (opKiller != null && IceAndFire.game.MyGold >= Unit.TrainCosts[3])
            {
                Command.Train(3, opKiller.Position);
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
            var placesForTrain = BaseStrategy.PlacesForTrain();
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

            var places = PlacesForTower();
            var placesUnderAttact =
                places.ToDictionary(p => p, p => IceAndFire.game.Area8(p).Count(c => c.Unit?.IsOpponent == true));
            if (placesUnderAttact.Values.Max() > 0)
            {
                var towerPlace = placesUnderAttact.OrderByDescending(p => p.Value).First().Key;
                Console.Error.WriteLine($"{placesUnderAttact.Values.Max()}, {towerPlace}");
                Command.Build(BuildingType.Tower, towerPlace);
                return true;
            }

            return false;
        }

        public Position[] PlacesForTower()
        {
            var myTerritory = IceAndFire.game.MyPositions
                                        .Select(p => IceAndFire.game.Map[p.X, p.Y])
                                        .Where(s => s.Active && !s.HasMineSpot)
                                        .Select(s => s.Position);
            var freeTerritory = myTerritory
                .Except(IceAndFire.game.Buildings.Select(b => b.Position))
                .Except(IceAndFire.game.MyUnits.Select(u => u.Position))
                .Except(IceAndFire.game.HoldPositions);

            return freeTerritory.ToArray();
        }

        public bool HasMenace()
        {
            var opponents = IceAndFire.game.OpponentUnits;
            var around = opponents.SelectMany(op => IceAndFire.game.Area8(op.Position));
            return around.Where(p => p.IsOwned).Any();
        }
    }
}
