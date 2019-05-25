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
            map.Map[pos.X, pos.Y].Owner = Owner.ME;
            map.Map[pos.X, pos.Y].Active = true;
        }
    }
}