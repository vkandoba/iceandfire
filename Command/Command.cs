namespace IceAndFire
{
    public abstract class Command : ICommand
    {
        public abstract Position Target { get; }

        public void Apply(Game game)
        {
            var cmd = MakeCmd();
            ChangeMap(game.Map);
            game.Output.Append($"{cmd};");
        }

        protected abstract string MakeCmd();
        protected abstract void ChangeMap(GameMap map);

        public override string ToString()
        {
            return MakeCmd();
        }
    }
}