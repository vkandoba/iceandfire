using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class CommandGenerator
    {
        public static IEnumerable<ICommand> Moves(GameMap map)
        {
            var units = map.MyUnits;
            foreach (var unit in units)
            {
                foreach (var tile in map.Area4(unit.Position))
                {
                    if (map.AllowMove(tile))
                        yield return new MoveCommand(unit, tile.Position);
                }
            }
        }
        public static IEnumerable<ICommand> Trains(GameMap map)
        {
            if (map.MyGold >= Unit.TrainCosts[1])
                return map.PlacesForTrain().Select(p => new TrainCommand(1, p.Position));
            return new ICommand[0];
        }
    }
}