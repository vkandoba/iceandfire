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
    }
}