namespace IceAndFire
{
    public class BuildCommand : Command
    {
        private readonly BuildingType type;
        private readonly Position pos;

        public BuildCommand(BuildingType type, Position pos)
        {
            this.type = type;
            this.pos = pos;
        }

        protected override string MakeCmd() => $"BUILD {type.ToString().ToUpper()} {pos.X} {pos.Y};";

        protected override void ChangeMap(GameMap map)
        {
            if (type == BuildingType.Tower)
                map.HoldPositions.Add(pos);
            var cost = type == BuildingType.Mine ? IceAndFire.MINE_BUILD_COST : IceAndFire.TOWER_BUILD_COST;
            IceAndFire.game.HoldGold += cost;
            MarkPosition(map, pos);
        }
    }
}