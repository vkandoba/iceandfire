namespace IceAndFire
{
    public abstract class Command : ICommand
    {
        public void Apply(Game game)
        {
            var cmd = MakeCmd();
            MakeHolds(game.Map);
            game.Output.Append($"{cmd};");
        }

        protected abstract string MakeCmd();
        protected abstract void MakeHolds(GameMap map);
    }
}