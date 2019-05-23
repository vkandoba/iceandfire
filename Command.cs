﻿using System.Text;

namespace IceAndFire
{
    public static class Command
    {
        private static void Apply(StringBuilder output, string cmd) => output.Append($"{cmd};");
        public static string Train(int level, Position position)
        {
            // TODO: Handle upkeep
            int cost = 0;
            switch (level)
            {
                case 1: cost = IceAndFire.TRAIN_COST_LEVEL_1; break;
                case 2: cost = IceAndFire.TRAIN_COST_LEVEL_2; break;
                case 3: cost = IceAndFire.TRAIN_COST_LEVEL_3; break;
            }

            IceAndFire.game.HoldGold += cost;
            Command.Apply(IceAndFire.game.Output, $"TRAIN {level} {position.X} {position.Y}");
            return $"TRAIN {level} {position.X} {position.Y}";
        }

        public static string Move(int id, Position position)
        {
            IceAndFire.game.HoldPositions.Add(position);
            Command.Apply(IceAndFire.game.Output, $"MOVE {id} {position.X} {position.Y}");
            return $"MOVE {id} {position.X} {position.Y}";
        }

        public static string Build(BuildingType type, Position position)
        {
            if (type == BuildingType.Tower)
                IceAndFire.game.HoldPositions.Add(position);
            var cost = type == BuildingType.Mine ? IceAndFire.MINE_BUILD_COST : IceAndFire.TOWER_BUILD_COST;
            IceAndFire.game.HoldGold += cost;
            Command.Apply(IceAndFire.game.Output, $"BUILD {type.ToString().ToUpper()} {position.X} {position.Y}");
            return $"BUILD {type.ToString().ToUpper()} {position.X} {position.Y};";
        }
    }
}