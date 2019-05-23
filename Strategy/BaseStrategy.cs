﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class BaseStrategy : IStrategy
    {
        public void ConstructBuildings()
        {
            foreach (var unit in IceAndFire.game.MyUnits)
            {
                if (IceAndFire.game.MineSpots.Contains(unit.Position) && IceAndFire.game.MyGold > IceAndFire.MINE_BUILD_COST)
                    Command.Build(BuildingType.Mine, unit.Position);
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

        public static Position GetOccupationMove(IceAndFire.Unit unit)
        {
            var ns = unit.Position.GetAdjacents();
            var next = ns.FirstOrDefault(p => !IceAndFire.game.Map[p.X, p.Y].IsOwned && 
                                              !IceAndFire.game.Map[p.X, p.Y].IsWall);
            return next;
        }

        public void TrainUnits()
        {
            var upkeep = IceAndFire.game.MyUnits.Sum(x => x.Upkeep);
            var placesForTrain = PlacesForTrain();
            if (TrainKiller(placesForTrain, upkeep))
                return;
            if (TrainSolder(placesForTrain, upkeep))
                return;
            if (TrainSlave(placesForTrain, upkeep))
                return;
        }

        private bool TrainBase(Position[] places, Func<Position[], Position> getPlace, int unitLevel, 
            int goldLimit, int unitLimit)
        {
            Position defaultPlace = IceAndFire.game.MyTeam == Team.Fire ? (1, 0) : (10, 11);
            var placeForTrain = getPlace(places) ?? defaultPlace;
            if (IceAndFire.game.MyGold > goldLimit && 
                (IceAndFire.game.MyIncome >= IceAndFire.game.MyUpkeep + IceAndFire.Unit.UpkeepCosts[unitLevel]) &&
                IceAndFire.game.MyUnits.Count(u => u.Level == unitLevel) < unitLimit)
            {
                Command.Train(unitLevel, placeForTrain);
                return true;
            }

            return false;
        }

        public bool TrainSlave(Position[] places, int upkeep)
        {
            return TrainBase(places, ps => ps
                    .OrderByDescending(p => p.GetAdjacents().Where(c => !IceAndFire.game.Map[c.X, c.Y].IsOwned).Count())
                    .FirstOrDefault(),
                1,
                IceAndFire.TRAIN_COST_LEVEL_1,
                5);
        }

        public bool TrainSolder(Position[] places, int upkeep)
        {
            return TrainBase(places, ps => ps.OrderBy(IceAndFire.game.OpponentHq.MDistanceTo)
                    .FirstOrDefault(),
                2,
                IceAndFire.TRAIN_COST_LEVEL_2,
                4);
        }

        public bool TrainKiller(Position[] places, int upkeep)
        {
            return TrainBase(places, ps => ps.OrderBy(IceAndFire.game.OpponentHq.MDistanceTo)
                    .FirstOrDefault(),
                3,
                IceAndFire.TRAIN_COST_LEVEL_3,
                1);
        }

        public Position[] PlacesForTrain()
        {
            var freeTerritory = GetOccupied()
                .Except(IceAndFire.game.Buildings.Select(b => b.Position));
            foreach (var unit in IceAndFire.game.Units)
                freeTerritory = freeTerritory.Except(new[] { unit.Position }.Concat(unit.Position.GetAdjacents()));

            return freeTerritory.ToArray();
        }

        public Position[] GetOccupied()
        {
            var territory = new List<Position>();
            for (var x = 0; x < 12; ++x)
            for (var y = 0; y < 12; ++y)
                if (IceAndFire.game.Map[x, y].Owner == IceAndFire.ME)
                {
                    //                           Console.Error.WriteLine($"Position: {Map[x, y].Position} Owner: {Map[x, y].Owner}");
                    territory.Add(IceAndFire.game.Map[x, y].Position);
                }
            return territory.ToArray();
        }
    }
}