using System.Collections.Generic;
using System.Linq;

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
        public HashSet<Tile> OpPositions = new HashSet<Tile>();
        public HashSet<Tile> NeutralPositions = new HashSet<Tile>();

        public List<Position> MineSpots = new List<Position>();

        public void UpTurn()
        {
            Me.Gold += Me.Income - Me.Upkeep;
            Opponent.Gold += Opponent.Income - Opponent.Upkeep;

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

        public void Clear()
        {
            Units.Clear();
            Buildings.Clear();
            
            MyPositions.Clear();
            OpPositions.Clear();
            NeutralPositions.Clear();
        }
    }
}
