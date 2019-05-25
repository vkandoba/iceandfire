namespace IceAndFire
{
    public class WaitCommand : Command
    {
        public override Position Target => null;

        public override void Unapply(GameMap game)
        {
        }

        protected override string MakeCmd() => "WAIT";

        protected override void ChangeMap(GameMap map)
        {
        }
    }
}