using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public class GameMap
    {
        public const int WIDTH = 12;
        public const int HEIGHT = 12;

        public Team MyTeam => Me.Team;
        public int MyGold => ActualGold - HoldGold;
        public int MyUpkeep => ActuaUpkeep - HoldUpkeep;
        public int MyIncome => Me.Income;
        public int ActualGold => Me.Gold;
        public int ActuaUpkeep => Me.Upkeep;
        public int OpponentGold => Opponent.Gold;
        public int OpponentIncome => Opponent.Income;

        public int HoldGold;
        public int HoldUpkeep;

        public PlayerState Me = new PlayerState();
        public PlayerState Opponent = new PlayerState();

        public readonly Tile[,] Map = new Tile[WIDTH, WIDTH];
        
        public Position MyHq => MyTeam == Team.Fire ? (0, 0) : (11, 11);
        public Position OpponentHq => MyTeam == Team.Fire ? (11, 11) : (0, 0);
        public List<Unit> MyUnits => Units.Where(u => u.IsOwned).ToList();
        public List<Unit> OpponentUnits => Units.Where(u => u.IsOpponent).ToList();

        public List<Building> Buildings = new List<Building>();
        public List<Unit> Units = new List<Unit>();

        public List<Tile> MyPositions = new List<Tile>();
        public List<Tile> OpPositions = new List<Tile>();
        public List<Tile> NeutralPositions = new List<Tile>();
        public HashSet<Position> HoldPositions = new HashSet<Position>();


        public List<Position> MineSpots = new List<Position>();

        public Tile[] PlacesForTrain(int level = 1)
        {
            var territory = MyPositions.Concat(MyPositions.SelectMany(this.Area4)).Distinct();
            return territory.Where(t => AllowMove(t, level)).ToArray();
        }

        public Tile[] PlacesForTower()
        {
            return MyPositions.Where(AllowBuilldTower).ToArray();
        }

        public bool AllowMove(Tile tile, int level = 1)
        {
            return !(HoldPositions.Contains(tile.Position) ||
                     (tile.Building != null && tile.IsOwned) ||
                     tile.Unit?.IsOwned == true || (tile.Unit?.Level ?? 0) >= level);
        }

        private bool AllowBuilldTower(Tile tile)
        {
            return tile.Unit == null && tile.Building == null && !tile.HasMineSpot && !HoldPositions.Contains(tile.Position);
        }

        public bool HasMenace()
        {
            var around = OpponentUnits.SelectMany(op => this.Area8(op.Position));
            return around.Where(p => p.IsOwned).Any();
        }

        public string ShowMap()
        {
            var str = new StringBuilder();
            str.AppendLine(Me.ToString());
            str.AppendLine(Opponent.ToString());
            str.AppendLine();
            str.Append($"   ");
            for (int y = 0; y < HEIGHT; y++)
            {
                str.Append($"{y} ");
            }
            str.AppendLine();
            str.AppendLine();

            for (int y = 0; y < HEIGHT; y++)
            {
                str.Append($"{y.ToString().PadRight(3)}");
                for (int x = 0; x < WIDTH; x++)
                {
                    str.Append($"{Map[x, y].ToChar()} ");
                }

                str.AppendLine();
            }

            return str.ToString();
        }

        public void Clear()
        {
            Units.Clear();
            Buildings.Clear();
            
            MyPositions.Clear();
            OpPositions.Clear();
            NeutralPositions.Clear();

            HoldPositions.Clear();
            HoldGold = 0;
            HoldUpkeep = 0;
        }

        public static Position[] FindPathInternal(Func<Position, bool> isFree, 
            Position source,
            Position target)
        {
            var queue = new Queue<Position>();
            var path = new Dictionary<Position, Position>();
            path[source] = source;
            queue.Enqueue(source);
            do
            {
                var pos = queue.Dequeue();
                if (pos == target)
                    return BuidlPath(path, target, source);
                var area = pos.Area4().Where(isFree).Where(c => !path.ContainsKey(c));
                foreach (var c in area)
                {
                    path[c] = pos;
                    queue.Enqueue(c);
                }
            } while (queue.Any());

            return new Position[0];
        }

        private static Position[] BuidlPath(Dictionary<Position, Position> links, Position target, Position source)
        {
            var path = new List<Position>();
            var current = target;
            do
            {
                path.Add(current);
                current = links[current];
            } while (current != source);

            path.Reverse();
            return path.ToArray();
        }
    }
}
