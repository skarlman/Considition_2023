using GeneticSharp;
using Shared.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics.NormalMap
{
    internal class SolutionFitnessFunction : IFitness
    {
        private readonly MapData _mapData;
        private readonly GeneralData _generalData;

        public SolutionFitnessFunction(MapData mapData, GeneralData generalData)
        {
            _mapData = mapData;
            _generalData = generalData;
        }
        public double Evaluate(IChromosome chromosome)
        {
            var candidateChromosome = chromosome as SolutionChromosome;

            var gameData = Scoring.CalculateScore("IRRELEVANTMAPNAME", candidateChromosome.ToSolution(_mapData), _mapData, _generalData);

            return gameData.GameScore.Total;

        }
    }
}
