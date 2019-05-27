namespace IceAndFire
{
    public static class TileExtensions
    {
        public static bool AllowMove(this Tile tile, int level = 1)
        {
            if (tile.IsWall)
                return false;
            if (tile.IsUnderAttack)
                return level == 3;
            if (tile.Building != null)
            {
                if (tile.IsOwned && !tile.Building.IsMine)
                    return false;
            }
            if (tile.Unit != null)
            {
                if (tile.Unit.IsOwned)
                    return false;

                return level != 1 && level > tile.Unit.Level;
            }

            return true;
        }

        public static bool AllowBuilldTower(this Tile tile)
        {
            return tile.IsOwned && tile.Active && tile.Unit == null && tile.Building == null && !tile.HasMineSpot && !tile.IsWall;
        }

        public static Tile MarkIsMe(this Tile tile)
        {
            tile.Owner = Owner.ME;
            tile.Active = true;
            return tile;
        }
    }
}