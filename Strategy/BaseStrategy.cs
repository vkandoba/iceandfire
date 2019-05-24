using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class BaseStrategy : IStrategy
    {
        public ICommand[] ConstructBuildings() => ConstructMines();

        public ICommand[] ConstructMines()
        {
            var myTerritory = IceAndFire.game.MyPositions
                .Select(x => IceAndFire.game.Map[x.X, x.Y])
                .Where(x => x.Active)
                .ToArray();
            var mySpots = myTerritory.Where(x => x.HasMineSpot && x.Building == null).ToArray();
            if (mySpots.Any() && IceAndFire.game.MyGold > IceAndFire.MINE_BUILD_COST)
            {
                var posForMine = mySpots.Select(s => s.Position).OrderBy(IceAndFire.game.MyHq.MDistanceTo).First();
                return new [] {Commands.Build(BuildingType.Mine, posForMine)};
            }
            return new ICommand[0];
        }

        public ICommand[] MoveUnits()
        {
            Position target = IceAndFire.game.OpponentHq;

            if (IceAndFire.game.Map[target.X, target.Y].IsOwned)
                return new ICommand[0];

            var cmd = new List<ICommand>();
            var workers = IceAndFire.game.MyUnits.Where(u => u.Level == 1).ToArray();
            foreach (var unit in workers)
            {
                var om = GetOccupationMove(unit)?.Position ?? target;
                cmd.Add(Commands.Move(unit.Id, om));
                IceAndFire.game.Map[om.X, om.Y].Owner = IceAndFire.ME;
            }
            var solders = IceAndFire.game.MyUnits.Where(u => u.Level > 1).ToArray();
            foreach (var solder in solders)
            {
                var path = GameMap.FindPathInternal(p => !IceAndFire.game.Map[p.X, p.Y].IsWall && 
                                                                     IceAndFire.game.Map[p.X, p.Y]?.Unit?.IsOwned != true,
                    solder.Position, target);
                cmd.Add(Commands.Move(solder.Id, path[0]));
            }

            return cmd.ToArray();
        }

        public Tile GetOccupationMove(Unit unit)
        {
            var ns = IceAndFire.game.Area4(unit.Position);
            var next = ns.Where(c => c.Unit == null && !IceAndFire.game.HoldPositions.Contains(c.Position))
                         .OrderByDescending(p => IceAndFire.game.Area8(p)
                                                    .Where(c => !c.IsWall && c.IsNeutral)
                                                    .Count())
                         .FirstOrDefault();
            return next;
        }

        public ICommand[] TrainUnits()
        {
            var placesForTrain = IceAndFire.game.PlacesForTrain();
            var trainKiller = TrainKiller(placesForTrain);
            if (trainKiller != null)
                return new []{ trainKiller };
            var trainSolder = TrainSolder(placesForTrain);
            if (trainSolder != null)
                return new[] { trainSolder };
            var trainSlave = TrainSlave(placesForTrain, 5);
            if (trainSlave != null)
                return new[]{ trainSlave };

            return new ICommand[0];
        }

        public ICommand TrainBase(Tile[] places, Func<Tile[], Tile> getPlace, int unitLevel, int unitLimit)
        {
            var defaultPlace = IceAndFire.game.Area4(IceAndFire.game.MyHq).First();
            var placeForTrain = getPlace(places) ?? defaultPlace;
            //Console.Error.WriteLine($"{IceAndFire.game.MyGold}, " +
            //                        $"{IceAndFire.game.MyUpkeep + IceAndFire.Unit.UpkeepCosts[unitLevel]}, " +
            //                        $"{IceAndFire.game.MyUnits.Count(u => u.Level == unitLevel)}");
            if (IceAndFire.game.MyGold >= Unit.TrainCosts[unitLevel] && 
                (IceAndFire.game.MyIncome >= IceAndFire.game.MyUpkeep + Unit.UpkeepCosts[unitLevel]) &&
                IceAndFire.game.MyUnits.Count(u => u.Level == unitLevel) < unitLimit)
            {
                return Commands.Train(unitLevel, placeForTrain.Position);
            }

            return null;
        }
        
        public ICommand TrainSlave(Tile[] places, int limit)
        {
            return TrainBase(places, ps => ps
                    .OrderByDescending(p => IceAndFire.game.Area4(p)
                                             .Where(c => !c.IsOwned && !c.IsWall).Count())
                    .FirstOrDefault(),
                1,
                limit);
        }

        public ICommand TrainSolder(Tile[] places)
        {
            return TrainBase(places, ps => ps.OrderBy(c => IceAndFire.game.OpponentHq.MDistanceTo(c.Position))
                    .FirstOrDefault(),
                2,
                4);
        }

        public ICommand TrainKiller(Tile[] places)
        {
            return TrainBase(places, ps => ps.OrderBy(c => IceAndFire.game.OpponentHq.MDistanceTo(c.Position))
                    .FirstOrDefault(),
                3,
                2);
        }

    }
}