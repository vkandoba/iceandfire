namespace IceAndFire
{
    public abstract class Command : ICommand
    {
        public abstract Position Target { get; }

        public void Execute(Game game)
        {
            var cmd = MakeCmd();
            Apply(game.Map);
            game.Output.Append($"{cmd};");
        }

        public void Apply(GameMap gameMap)
        {
            ChangeMap(gameMap);
        }

        public abstract void Unapply(GameMap game);

        protected abstract string MakeCmd();
        protected abstract void ChangeMap(GameMap map);

        public override string ToString()
        {
            return MakeCmd();
        }
    }
}