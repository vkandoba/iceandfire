using System;
using System.Collections.Generic;
using System.Linq;

namespace IceAndFire
{
    public class TurnGenerator
    {
       
        public static IEnumerable<List<ICommand>> Turns(GameMap game)
        {
            return GenerateNextTrains(game, new List<ICommand>());
        }

        private static IEnumerable<List<ICommand>> GenerateNextTrains(GameMap game, List<ICommand> prefix)
        {
            var nextTrains = CommandGenerator.Trains(game).ToList();
            if (!nextTrains.Any())
                yield break;

            var chains = new List<List<ICommand>>();
            foreach (var nextTrain in nextTrains)
            {
                var withNext = prefix.Concat(new[] { nextTrain }).ToList();
                yield return withNext;
                nextTrain.Apply(game);
                chains.AddRange(GenerateNextTrains(game, withNext));
                nextTrain.Unapply(game);
            }

            foreach (var chain in chains)
                yield return chain;
        }
        
    }
}