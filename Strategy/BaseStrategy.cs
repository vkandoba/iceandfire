using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class BaseStrategy : IStrategy
    {
        public void ConstructBuildings()
        {
            ConstructMines();
        }

        public void ConstructMines()
        {
            var myTerritory = IceAndFire.game.MyPositions
                .Select(x => IceAndFire.game.Map[x.X, x.Y])
                .Where(x => x.Active)
                .ToArray();
            var mySpots = myTerritory.Where(x => x.HasMineSpot && x.Building == null).ToArray();
            if (mySpots.Any() && IceAndFire.game.MyGold > IceAndFire.MINE_BUILD_COST)
            {
                var posForMine = mySpots.Select(s => s.Position).OrderBy(IceAndFire.game.MyHq.MDistanceTo).First();
                Command.Build(BuildingType.Mine, posForMine);
            }
        }

        public void MoveUnits()
        {
            Position target = IceAndFire.game.OpponentHq;

            if (IceAndFire.game.Map[target.X, target.Y].IsOwned) return;

            var workers = IceAndFire.game.MyUnits.Where(u => u.Level == 1).ToArray();
            foreach (var unit in workers)
            {
                var om = GetOccupationMove(unit) ?? target;
                Command.Move(unit.Id, om);
                IceAndFire.game.Map[om.X, om.Y].Owner = IceAndFire.ME;
            }

            var solders = IceAndFire.game.MyUnits.Where(u => u.Level > 1).ToArray();
            foreach (var solder in solders)
            {
                Command.Move(solder.Id, target);
            }
        }

        public Position GetOccupationMove(Unit unit)
        {
            var ns = unit.Position.Area4();
            var next = ns.Where(p => !IceAndFire.game.Map[p.X, p.Y].IsWall &&
                                     IceAndFire.game.Map[p.X, p.Y].Unit == null &&
                                     !IceAndFire.game.HoldPositions.Contains(p))
                         .OrderByDescending(p => p.Area8()
                                                    .Where(a => !IceAndFire.game.Map[p.X, p.Y].IsWall &&
                                                                 IceAndFire.game.Map[p.X, p.Y].IsNeutral)
                                                    .Count())
                         .FirstOrDefault();
            return next;
        }

        public void TrainUnits()
        {
            var placesForTrain = PlacesForTrain();
            if (TrainKiller(placesForTrain))
                return;
            if (TrainSolder(placesForTrain))
                return;
            if (TrainSlave(placesForTrain, 5))
                return;
        }

        private bool TrainBase(Position[] places, Func<Position[], Position> getPlace, int unitLevel, 
            int cost, int unitLimit)
        {
            Position defaultPlace = IceAndFire.game.MyTeam == Team.Fire ? (1, 0) : (10, 11);
            var placeForTrain = getPlace(places) ?? defaultPlace;
            //Console.Error.WriteLine($"{IceAndFire.game.MyGold}, " +
            //                        $"{IceAndFire.game.MyUpkeep + IceAndFire.Unit.UpkeepCosts[unitLevel]}, " +
            //                        $"{IceAndFire.game.MyUnits.Count(u => u.Level == unitLevel)}");
            if (IceAndFire.game.MyGold > cost && 
                (IceAndFire.game.MyIncome >= IceAndFire.game.MyUpkeep + Unit.UpkeepCosts[unitLevel]) &&
                IceAndFire.game.MyUnits.Count(u => u.Level == unitLevel) < unitLimit)
            {
                Command.Train(unitLevel, placeForTrain);
                return true;
            }

            return false;
        }
        
        public bool TrainSlave(Position[] places, int limit)
        {
            return TrainBase(places, ps => ps
                    .OrderByDescending(p => p.Area4()
                                             .Where(c => !IceAndFire.game.Map[c.X, c.Y].IsOwned &&
                                                         !IceAndFire.game.Map[c.X, c.Y].IsWall).Count())
                    .FirstOrDefault(),
                1,
                IceAndFire.TRAIN_COST_LEVEL_1,
                limit);
        }

        public bool TrainSolder(Position[] places)
        {
            return TrainBase(places, ps => ps.OrderBy(IceAndFire.game.OpponentHq.MDistanceTo)
                    .FirstOrDefault(),
                2,
                IceAndFire.TRAIN_COST_LEVEL_2,
                4);
        }

        public bool TrainKiller(Position[] places)
        {
            return TrainBase(places, ps => ps.OrderBy(IceAndFire.game.OpponentHq.MDistanceTo)
                    .FirstOrDefault(),
                3,
                IceAndFire.TRAIN_COST_LEVEL_3,
                2);
        }

        public static Position[] PlacesForTrain()
        {
            var myTerritory = IceAndFire.game.MyPositions.Where(p => IceAndFire.game.Map[p.X, p.Y].Active);
            var canBeTraining = new HashSet<Position>(myTerritory
                .Concat(myTerritory.SelectMany(p => p.Area4().Where(c => !IceAndFire.game.Map[c.X, c.Y].IsWall))));
            var freeTerritory = canBeTraining
                .Except(IceAndFire.game.Buildings.Select(b => b.Position))
                .Except(IceAndFire.game.MyUnits.Select(u => u.Position))
                .Except(IceAndFire.game.HoldPositions);

            return freeTerritory.ToArray();
        }
    }
}