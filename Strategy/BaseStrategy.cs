﻿using System;
using System.Linq;

namespace IceAndFire
{
    public class BaseStrategy
    {
        public void ConstructBuildings() => ConstructMines();

        public void ConstructMines()
        {
            var myTerritory = IceAndFire.game.MineSpots
                .Select(x => IceAndFire.game.Map[x.X, x.Y])
                .Where(t => t.IsOwned && t.Active)
                .ToArray();
            var mySpots = myTerritory.Where(x => x.HasMineSpot && x.Building == null).ToArray();
            if (mySpots.Any() && IceAndFire.game.MyGold > IceAndFire.MINE_BUILD_COST)
            {
                var posForMine = mySpots.Select(s => s.Position).OrderBy(s => IceAndFire.game.DistanceToMyHQ[s.X, s.Y]).First();
                Commands.Build(BuildingType.Mine, posForMine);
            }
        }

        public void MoveUnits()
        {
            Position target = IceAndFire.game.OpponentHq.Position;

            if (IceAndFire.game.Map[target.X, target.Y].IsOwned) return;

            var workers = IceAndFire.game.MyUnits.Where(u => u.Level == 1).ToArray();
            foreach (var unit in workers)
            {
                var om = GetOccupationMove(unit)?.Position ?? target;
                Commands.Move(unit, om);
            }
            var solders = IceAndFire.game.MyUnits.Where(u => u.Level > 1).ToArray();
            foreach (var solder in solders)
            {
                var path = Geometry.FindPathInternal(p => !IceAndFire.game.Map[p.X, p.Y].IsWall && 
                                                                     IceAndFire.game.Map[p.X, p.Y]?.Unit?.IsOwned != true,
                    solder.Position, target);
                Commands.Move(solder, path[0]);
            }
        }

        public Tile GetOccupationMove(Unit unit)
        {
            var ns = IceAndFire.game.Area4[IceAndFire.game.Map[unit.Position.X, unit.Position.Y]];
            var next = ns.Where(c => c.Unit == null)
                         .OrderByDescending(p => IceAndFire.game.Area8[p]
                                                    .Where(c => !c.IsWall && c.IsNeutral)
                                                    .Count())
                         .FirstOrDefault();
            return next;
        }

        public void TrainUnits()
        {
            var placesForTrain = IceAndFire.game.PlacesForTrain();
            if (TrainKiller(placesForTrain))
                return;
            if (TrainSolder(placesForTrain))
                return;
            if (TrainSlave(placesForTrain, 5))
                return;
        }

        public bool TrainBase(Tile[] places, Func<Tile[], Tile> getPlace, int unitLevel, int unitLimit)
        {
            var defaultPlace = IceAndFire.game.Area4[IceAndFire.game.MyHq].First();
            var placeForTrain = getPlace(places) ?? defaultPlace;
            if (IceAndFire.game.MyGold >= Unit.TrainCosts[unitLevel] && 
                (IceAndFire.game.MyIncome >= IceAndFire.game.MyUpkeep + Unit.UpkeepCosts[unitLevel]) &&
                IceAndFire.game.MyUnits.Count(u => u.Level == unitLevel) < unitLimit)
            {
                Commands.Train(unitLevel, placeForTrain.Position);
                return true;
            }

            return false;
        }
        
        public bool TrainSlave(Tile[] places, int limit)
        {
            return TrainBase(places, ps => ps
                    .OrderByDescending(p => IceAndFire.game.Area4[p]
                                             .Where(c => !c.IsOwned && !c.IsWall).Count())
                    .FirstOrDefault(),
                1,
                limit);
        }

        public bool TrainSolder(Tile[] places)
        {
            return TrainBase(places, ps => ps.OrderBy(c => IceAndFire.game.OpponentHq.Position.MDistanceTo(c.Position))
                    .FirstOrDefault(),
                2,
                4);
        }

        public bool TrainKiller(Tile[] places)
        {
            return TrainBase(places, ps => ps.OrderBy(c => IceAndFire.game.OpponentHq.Position.MDistanceTo(c.Position))
                    .FirstOrDefault(),
                3,
                2);
        }

    }
}