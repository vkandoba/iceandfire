namespace IceAndFire
{
    public abstract class BaseCommand : Command
    {
        public override Position Target => target;

        protected readonly Position target;

        protected BaseCommand(Position target)
        {
            this.target = target;
        }

        protected override void ChangeMap(GameMap map)
        {
            MarkPosition(map, target);
        }

        private static void MarkPosition(GameMap map, Position pos)
        {
            var tile = map.Map[pos.X, pos.Y];
            tile.Owner = Owner.ME;
            tile.Active = true;
            if (!map.MyPositions.Contains(tile))
                map.MyPositions.Add(tile);
            map.NeutralPositions.Remove(tile);
            map.OpPositions.Remove(tile);
        }

        protected static void DestroyOp(GameMap map, Position pos)
        {
            var tile = map.Map[pos.X, pos.Y];
            if (tile.Unit != null)
            {
                map.Units.Remove(tile.Unit);
                tile.Unit = null;
            }
            if (tile.Building != null)
            {
                map.Buildings.Remove(tile.Building);
                tile.Building = null;
            }
        }
    }
}