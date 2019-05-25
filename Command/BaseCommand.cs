namespace IceAndFire
{
    public abstract class BaseCommand : Command
    {
        public override Position Target => target;

        protected readonly Position target;
        protected Owner savedOwner;
        protected bool savedActive;

        protected BaseCommand(Position target)
        {
            this.target = target;
        }

        protected override void ChangeMap(GameMap map)
        {
            var tile = map.Map[target.X, target.Y];

            savedOwner = tile.Owner;
            savedActive = tile.Active;

            map.UpdateTile(tile.MarkIsMe());
        }

        public override void Unapply(GameMap game)
        {
            var tile = game.Map[target.X, target.Y];

            tile.Owner = savedOwner;
            tile.Active = savedActive;

            game.UpdateTile(tile);
        }
    }
}