using Considition2023_Cs.Game;
using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics
{
    internal class SandboxSolutionFitnessFunction : IFitness
    {
        private readonly MapData _mapData;
        private readonly GeneralData _generalData;

        public SandboxSolutionFitnessFunction(MapData mapData, GeneralData generalData)
        {
            this._mapData = mapData;
            this._generalData = generalData;
        }
        public double Evaluate(IChromosome chromosome)
        {
            var candidateChromosome = chromosome as SandboxSolutionChromosome;

            var gameData = Scoring.CalculateScore(_mapData.MapName, candidateChromosome.ToSolution(_mapData), _mapData, _generalData);

            return gameData.GameScore.Total;

        }
    }
}
