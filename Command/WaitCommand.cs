namespace IceAndFire
{
    public class WaitCommand : Command
    {
        public override Position Target => null;

        protected override string MakeCmd() => "WAIT";

        protected override void ChangeMap(GameMap map)
        {
        }
    }
}