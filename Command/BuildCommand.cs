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
            if (type == BuildingType.Tower)
                map.HoldPositions.Add(target);
            var cost = type == BuildingType.Mine ? IceAndFire.MINE_BUILD_COST : IceAndFire.TOWER_BUILD_COST;
            IceAndFire.game.HoldGold += cost;
        }
    }
}