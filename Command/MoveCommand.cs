﻿namespace IceAndFire
{
    public class MoveCommand : BaseCommand
    {
        private readonly Unit unit;

        public MoveCommand(Unit unit, Position target) : base(target)
        {
            this.unit = unit;
        }

        protected override string MakeCmd() => $"MOVE {unit.Id} {target.X} {target.Y}";

        protected override void ChangeMap(GameMap map)
        {
            map.DestroyOp(target);
            map.MarkPositionIsMe(target);

            unit.IsTouch = true;
            var old = unit.Position;
            map.Map[old.X, old.Y].Unit = null;
            unit.Position = target;
            map.Map[target.X, target.Y].Unit = unit;
        }

        public override string ToString() => $"{unit.Position} -> {target}";
    }
}