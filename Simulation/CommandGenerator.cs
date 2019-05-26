using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class CommandGenerator
    {
        public static IEnumerable<ICommand[]> MovesByUnit(GameMap map, Func<Position, bool> filter)
        {
            var units = map.MyUnits;
            for (int i = 0; i < units.Length; i++)
            {
                if (!units[i].IsTouch)
                {
                    var unitArea = map.Area4[map.Map[units[i].Position.X, units[i].Position.Y]];
                    var unitMoves = unitArea
                        .Where(tile => tile.AllowMove() && filter(tile.Position))
                        .Select(tile => new MoveCommand(units[i], tile.Position) as ICommand)
                        .ToArray();
                    if (unitMoves.Any())
                        yield return unitMoves;
                }
            }
        }
        public static IEnumerable<ICommand> Moves(GameMap map)
        {
            return MovesByUnit(map, (p) => true).SelectMany(x => x);
        }

        public static ICommand[] Trains(GameMap map)
        {
            if (map.MyGold >= Unit.TrainCosts[1])
            {
                var places = map.PlacesForTrain();
                var cmd = new ICommand[places.Length];
                for (int i = 0; i < places.Length; i++)
                {
                    cmd[i] = new TrainCommand(1, places[i].Position);
                }
                return cmd;
            }
            return new ICommand[0];
        }
    }
}