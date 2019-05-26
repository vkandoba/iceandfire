namespace IceAndFire
{
    public class BuildCommand : BaseCommand
    {
        private readonly BuildingType type;
        private Building building = null;
        private int cost;

        public BuildCommand(BuildingType type, Position pos) : base(pos)
        {
            this.type = type;
        }

        protected override string MakeCmd() => $"BUILD {type.ToString().ToUpper()} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            base.ChangeMap(map);

            building = new Building {Owner = Owner.ME, Position = target, Type = type};
            map.Map[target.X, target.Y].Building = building;
            map.Buildings.Add(map.Map[target.X, target.Y], building);

            cost = type == BuildingType.Mine ? IceAndFire.MINE_BUILD_COST : IceAndFire.TOWER_BUILD_COST;
            IceAndFire.game.Me.Gold -= cost;
        }

        public override void Unapply(GameMap game)
        {
            game.Map[target.X, target.Y].Building = null;
            game.Buildings.Remove(game.Map[target.X, target.Y]);
            building = null;

            base.Unapply(game);

            IceAndFire.game.Me.Gold += cost;
        }
    }
}