﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IceAndFire
{
    public class DefenseStrategy : IStrategy
    {
        public void MoveUnits()
        {
            Strategies.Base.MoveUnits();
        }

        public void TrainUnits()
        {
            var placesForTrain = BaseStrategy.PlacesForTrain();

            var opKiller = placesForTrain
                .Select(p => IceAndFire.game.Map[p.X, p.Y])
                .Where(t => t.Unit != null)
                .FirstOrDefault(t => t.Unit.IsOpponent && t.Unit.Level == 3);
            if (opKiller != null && IceAndFire.game.MyGold >= IceAndFire.TRAIN_COST_LEVEL_3)
                Command.Train(3, opKiller.Position);

            Strategies.Base.TrainKiller(placesForTrain);
        }

        public void ConstructBuildings()
        {
            if (IceAndFire.game.MyGold < IceAndFire.TOWER_BUILD_COST)
                return;

            var places = PlacesForTower();
            var placesUnderAttact =
                places.ToDictionary(p => p, p => p.Area8().Count(c => IceAndFire.game.Map[c.X, c.Y].Unit?.IsOpponent == true));
            var towerPlace = placesUnderAttact.OrderByDescending(p => p.Value).First().Key;
            Command.Build(BuildingType.Tower, towerPlace);
        }

        public Position[] PlacesForTower()
        {
            var myTerritory = IceAndFire.game.MyPositions
                                        .Select(p => IceAndFire.game.Map[p.X, p.Y])
                                        .Where(s => s.Active && !s.HasMineSpot)
                                        .Select(s => s.Position);
            var freeTerritory = myTerritory
                .Except(IceAndFire.game.Buildings.Select(b => b.Position))
                .Except(IceAndFire.game.MyUnits.Select(u => u.Position))
                .Except(IceAndFire.game.HoldPositions);

            return freeTerritory.ToArray();
        }

        public bool HasMenace()
        {
            var opponents = IceAndFire.game.OpponentUnits;
            var around = opponents.SelectMany(op => op.Position.Area8());
            return around.Where(p => IceAndFire.game.Map[p.X, p.Y].IsOwned).Any();
        }
    }
}