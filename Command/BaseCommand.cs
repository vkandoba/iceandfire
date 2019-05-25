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
    }
}