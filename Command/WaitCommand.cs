namespace IceAndFire
{
    public class WaitCommand : Command
    {
        protected override string MakeCmd() => "WAIT";

        protected override void ChangeMap(GameMap map)
        {
        }
    }
}