namespace IceAndFire
{
    public static class TileExtensions
    {
        public static bool AllowMove(this Tile tile, int level = 1)
        {
            return !((tile.Building?.IsTower == true && tile.IsOwned) || tile.IsWall ||
                     tile.Unit?.IsOwned == true || (tile.Unit?.Level ?? 0) >= level);
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