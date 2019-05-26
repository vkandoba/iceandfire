using System;
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
        public Unit[] MyUnits => Units.Values.Where(u => u.IsOwned).ToArray();
        public Unit[] OpponentUnits => Units.Values.Where(u => u.IsOpponent).ToArray();

        public Dictionary<Tile, Unit> Units = new Dictionary<Tile, Unit>();
        public Dictionary<Tile, Building> Buildings = new Dictionary<Tile, Building>();

        public int MyPlaces = 0;
        public int OpPlaces = 0;

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
            var places = new Tile[WIDTH * HEIGHT];
            int count = 0;
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    var tile = Map[x, y];
                    var area = Area4[Map[x, y]];
                    if (((tile.IsOwned && tile.Active) ||
                        area.Any(n => n.IsOwned && n.IsOwned)) &&
                        tile.AllowMove(level))
                    {
                        places[count] = tile;
                        count++;
                    }
                }
            }

            var forTrain = new Tile[count];
            Array.Copy(places, forTrain, count);
            return forTrain;
        }

        public Tile[] PlacesForTower()
        {
            var places = new Tile[WIDTH * HEIGHT];
            int count = 0;
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    var tile = Map[x, y];
                    if ((tile.IsOwned && tile.Active) && tile.AllowBuilldTower())
                    {
                        places[count] = tile;
                        count++;
                    }
                }
            }

            var forTower = new Tile[count];
            Array.Copy(places, forTower, count);
            return forTower;
        }

        public bool HasMenace()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    var area = Area4[Map[x, y]];
                    if (area.Any(a => a.IsOwned) && area.Any(a => a.IsOpponent))
                        return true;
                }
            }
            return false;
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
            
            MyPlaces = 0;
            OpPlaces = 0;
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
