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
        
        public Position MyHq => MyTeam == Team.Fire ? (0, 0) : (11, 11);
        public Position OpponentHq => MyTeam == Team.Fire ? (11, 11) : (0, 0);
        public List<Unit> MyUnits => Units.Where(u => u.IsOwned).ToList();
        public List<Unit> OpponentUnits => Units.Where(u => u.IsOpponent).ToList();

        public HashSet<Building> Buildings = new HashSet<Building>();
        public HashSet<Unit> Units = new HashSet<Unit>();

        public HashSet<Tile> MyPositions = new HashSet<Tile>();

        public List<Position> MineSpots = new List<Position>();

        private int myGoldChange;
        private int oppenentGoldChange;

        public void UpTurn()
        {
            myGoldChange = Me.Income - Me.Upkeep;
            oppenentGoldChange = Opponent.Income - Opponent.Upkeep;
            Me.Gold += myGoldChange;
            Opponent.Gold += oppenentGoldChange;

            foreach (var unit in Units.Where(u => u.IsTouch))
            {
                unit.IsTouch = false;
            }
        }

        public void DownTurn()
        {
            Me.Gold -= myGoldChange;
            Opponent.Gold -= oppenentGoldChange;

            foreach (var unit in Units.Where(u => u.IsTouch))
            {
                unit.IsTouch = false;
            }
        }

        public Tile[] PlacesForTrain(int level = 1)
        {
            var territory = MyPositions.Concat(MyPositions.SelectMany(this.Area4)).Distinct();
            return territory.Where(t => t.AllowMove(level)).ToArray();
        }

        public Tile[] PlacesForTower()
        {
            return MyPositions.Where(t => t.AllowBuilldTower()).ToArray();
        }

        public bool HasMenace()
        {
            var around = OpponentUnits.SelectMany(op => this.Area8(op.Position));
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
                Units.Remove(opUnit);
                tile.Unit = null;
                return opUnit;
            }
            if (tile.Building != null)
            {
                var opBuilding = tile.Building;
                Buildings.Remove(opBuilding);
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
                Units.Add(unit);
                tile.Unit = unit;
                return true;
            }

            if (destroed is Building building)
            {
                Buildings.Add(building);
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
        }
    }
}
