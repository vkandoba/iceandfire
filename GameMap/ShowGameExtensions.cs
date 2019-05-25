using System.Collections.Generic;
using System.Text;

namespace IceAndFire
{
    public static class ShowGameExtensions
    {
        public static string ShowMap(this GameMap game, IEnumerable<Position> marks = null)
        {
            var m = new HashSet<Position>(marks ?? new Position[0]);

            var str = new StringBuilder();
            str.AppendLine(game.Me.ToString());
            str.AppendLine(game.Opponent.ToString());
            str.AppendLine();
            str.Append($"   ");
            for (int y = 0; y < GameMap.HEIGHT; y++)
            {
                str.Append($"{y} ");
            }
            str.AppendLine();
            str.AppendLine();

            for (int y = 0; y < GameMap.HEIGHT; y++)
            {
                str.Append($"{y.ToString().PadRight(3)}");
                for (int x = 0; x < GameMap.WIDTH; x++)
                {
                    var c = !m.Contains(game.Map[x, y].Position) ? game.Map[x, y].ToChar() : "-";
                    str.Append($"{c} ");
                }

                str.AppendLine();
            }

            return str.ToString();
        }
    }
}