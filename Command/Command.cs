namespace IceAndFire
{
    public abstract class Command : ICommand
    {
        public void Apply(Game game)
        {
            var cmd = MakeCmd();
            ChangeMap(game.Map);
            game.Output.Append($"{cmd};");
        }

        protected abstract string MakeCmd();
        protected abstract void ChangeMap(GameMap map);

        protected void MarkPosition(GameMap map, Position pos)
        {
            map.Map[pos.X, pos.Y].Owner = Owner.ME;
            map.Map[pos.X, pos.Y].Active = true;
        }
    }
}