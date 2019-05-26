using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class CommandGenerator
    {
        public static IEnumerable<List<ICommand>> MovesByUnit(GameMap map, Func<Position, bool> filter)
        {
            var units = map.MyUnits;
            foreach (var unit in units.Where(u => !u.IsTouch))
            {
                var unitMoves = map.Area4[map.Map[unit.Position.X, unit.Position.Y]]
                    .Where(tile => tile.AllowMove() && filter(tile.Position))
                    .Select(tile => new MoveCommand(unit, tile.Position) as ICommand)
                    .ToList();
                if (unitMoves.Any())
                    yield return unitMoves;
            }
        }
        public static IEnumerable<ICommand> Moves(GameMap map)
        {
            return MovesByUnit(map, (p) => true).SelectMany(x => x);
        }

        public static IEnumerable<ICommand> Trains(GameMap map)
        {
            if (map.MyGold >= Unit.TrainCosts[1])
                return map.PlacesForTrain().Select(p => new TrainCommand(1, p.Position));
            return new ICommand[0];
        }
    }
}