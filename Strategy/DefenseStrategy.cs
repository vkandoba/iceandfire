using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class DefenseStrategy : IStrategy
    {
        public ICommand[] MoveUnits() => Strategies.Base.MoveUnits();

        public ICommand[] TrainUnits()
        {
            var trainKiller = TrainKiller();
            return trainKiller == null ? new ICommand[0] : new[] {trainKiller};
        }

        public ICommand TrainKiller()
        {
            var placesForTrain = IceAndFire.game.PlacesForTrain();

            var opKiller = placesForTrain
                .Where(t => t.Unit != null)
                .FirstOrDefault(t => t.Unit.IsOpponent && t.Unit.Level == 3);
            if (opKiller != null && IceAndFire.game.MyGold >= Unit.TrainCosts[3])
            {
                return Commands.Train(3, opKiller.Position);
            }

            return Strategies.Base.TrainKiller(placesForTrain);
        }

        public ICommand[] ConstructBuildings()
        {
            var cmd = new List<ICommand>();

            var constructTower = ConstructTower();
            while (constructTower != null)
            {
                cmd.Add(constructTower);
                constructTower = ConstructTower();
            }

            cmd.AddRange(Strategies.Base.ConstructMines());
            var placesForTrain = IceAndFire.game.PlacesForTrain();
            var neutralPlaces = placesForTrain.Where(c => c.IsNeutral).ToArray();
            if (neutralPlaces.Any())
                cmd.Add(Strategies.Base.TrainSlave(neutralPlaces, 20));

            var trainKiller = TrainKiller();
            while (trainKiller != null)
            {
                cmd.Add(trainKiller);
                trainKiller = TrainKiller();
            }

            return cmd.ToArray();
        }

        public ICommand ConstructTower()
        {
            if (IceAndFire.game.MyGold < IceAndFire.TOWER_BUILD_COST)
                return null;

            var places = PlacesForTower();
            var placesUnderAttact =
                places.ToDictionary(p => p, p => IceAndFire.game.Area8(p).Count(c => c.Unit?.IsOpponent == true));
            if (placesUnderAttact.Values.Max() > 0)
            {
                var towerPlace = placesUnderAttact.OrderByDescending(p => p.Value).First().Key;
                return Commands.Build(BuildingType.Tower, towerPlace);
            }

            return null;
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

        public bool HasMenace(GameMap gameMap)
        {
            var opponents = gameMap.OpponentUnits;
            var around = opponents.SelectMany(op => gameMap.Area8(op.Position));
            return around.Where(p => p.IsOwned).Any();
        }
    }
}
