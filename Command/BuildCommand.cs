namespace IceAndFire
{
    public class BuildCommand : BaseCommand
    {
        private readonly BuildingType type;

        public BuildCommand(BuildingType type, Position pos) : base(pos)
        {
            this.type = type;
        }

        protected override string MakeCmd() => $"BUILD {type.ToString().ToUpper()} {target.X} {target.Y};";

        protected override void ChangeMap(GameMap map)
        {
            base.ChangeMap(map);

            var building = new Building {Owner = Owner.ME, Position = target, Type = type};
            if (type == BuildingType.Tower)
                map.Buildings.Add(building);

            var cost = type == BuildingType.Mine ? IceAndFire.MINE_BUILD_COST : IceAndFire.TOWER_BUILD_COST;
            IceAndFire.game.Me.Gold += cost;
        }
    }
}