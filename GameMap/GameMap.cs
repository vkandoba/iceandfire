using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IceAndFire
{
    public class GameMap
    {
        public const int WIDTH = 12;
        public const int HEIGHT = 12;

        public Team MyTeam => Me.Team;
        public int MyGold => Me.Gold;
        public int MyUpkeep => Me.Upkeep;
        public int MyIncome => Me.Income;
        public int OpponentGold => Opponent.Gold;
        public int OpponentIncome => Opponent.Income;

        public PlayerState Me = new PlayerState();
        public PlayerState Opponent = new PlayerState();

        public readonly Tile[,] Map = new Tile[WIDTH, WIDTH];
        
        public Tile MyHq => MyTeam == Team.Fire ? Map[0, 0] : Map[11, 11];
        public Tile OpponentHq => MyTeam == Team.Fire ? Map[11, 11] : Map[0, 0];
        public List<Unit> MyUnits => Units.Values.Where(u => u.IsOwned).ToList();
        public List<Unit> OpponentUnits => Units.Values.Where(u => u.IsOpponent).ToList();

        public Dictionary<Tile, Unit> Units = new Dictionary<Tile, Unit>();
        public Dictionary<Tile, Building> Buildings = new Dictionary<Tile, Building>();

        public HashSet<Tile> MyPositions = new HashSet<Tile>();

        public List<Position> MineSpots = new List<Position>();

        public Dictionary<Tile, Tile[]> Area4 = new Dictionary<Tile, Tile[]>();

        public Dictionary<Tile, Tile[]> Area8 = new Dictionary<Tile, Tile[]>();

        private int myGoldChange;
        private int oppenentGoldChange;

        private IDictionary<Unit, bool> touchUnitState = new Dictionary<Unit, bool>();

        public void UpTurn()
        {
            myGoldChange = Me.Income - Me.Upkeep;
            oppenentGoldChange = Opponent.Income - Opponent.Upkeep;
            Me.Gold += myGoldChange;
            Opponent.Gold += oppenentGoldChange;

            foreach (var unit in Units.Values.Where(u => u.IsTouch))
            {
                touchUnitState[unit] = unit.IsTouch;
                unit.IsTouch = false;
            }
        }

        public void DownTurn()
        {
            Me.Gold -= myGoldChange;
            Opponent.Gold -= oppenentGoldChange;

            foreach (var unit in Units.Values.Where(u => u.IsTouch))
            {
                unit.IsTouch = touchUnitState[unit];
            }
        }

        public Tile[] PlacesForTrain(int level = 1)
        {
            var territory = MyPositions.Concat(MyPositions.SelectMany(t => Area4[t])).Distinct();
            return territory.Where(t => t.AllowMove(level)).ToArray();
        }

        public Tile[] PlacesForTower()
        {
            return MyPositions.Where(t => t.AllowBuilldTower()).ToArray();
        }

        public bool HasMenace()
        {
            var around = OpponentUnits.SelectMany(op => Area8[Map[op.X, op.Y]]);
            return around.Where(p => p.IsOwned).Any();
        }

        public Tile UpdateTile(Tile tile)
        {
            if (tile.IsOwned && !MyPositions.Contains(tile))
                MyPositions.Add(tile);

            if (!tile.IsOwned)
                MyPositions.Remove(tile);

            return tile;
        }

        public Entity DestroyOp(Position pos)
        {
            var tile = Map[pos.X, pos.Y];
            if (tile.Unit != null)
            {
                var opUnit = tile.Unit;
                Units.Remove(tile);
                tile.Unit = null;
                return opUnit;
            }
            if (tile.Building != null)
            {
                var opBuilding = tile.Building;
                Buildings.Remove(tile);
                tile.Building = null;
                return opBuilding;
            }

            return null;
        }

        public bool UnDestroyOp(Position pos, Entity destroed)
        {
            if (destroed == null)
                return false;

            var tile = Map[pos.X, pos.Y];

            if (destroed is Unit unit)
            {
                Units.Add(tile, unit);
                tile.Unit = unit;
                return true;
            }

            if (destroed is Building building)
            {
                Buildings.Add(tile, building);
                tile.Building = building;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            Units.Clear();
            Buildings.Clear();
            
            MyPositions.Clear();
            touchUnitState.Clear();
        }

        public void UpdateAreas()
        {
            for (var y = 0; y < HEIGHT; y++)
            {
                for (var x = 0; x < WIDTH; x++)
                {
                    var tile = Map[x, y];
                    if (!Area4.ContainsKey(tile))
                        Area4[tile] = Area4Internal((x, y));
                    if (!Area8.ContainsKey(tile))
                        Area8[tile] = Area8Internal((x, y));
                }
            }
        }

        private Tile[] Area4Internal(Position pos)
        {
            return pos.Area4().Select(p => Map[p.X, p.Y]).Where(t => !t.IsWall).ToArray();
        }

        private Tile[] Area8Internal(Position pos)
        {
            return pos.Area8().Select(p => Map[p.X, p.Y]).Where(t => !t.IsWall).ToArray();
        }
    }
}
