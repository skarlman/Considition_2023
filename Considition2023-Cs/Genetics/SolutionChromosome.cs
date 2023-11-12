using Considition2023_Cs.Game;
using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics
{
    internal class SolutionChromosome : ChromosomeBase
    {
        public SolutionChromosome(int length) : base(length)
        {
        }

        public override IChromosome CreateNew()
        {
            throw new NotImplementedException();
        }
        Random rng = new Random();
        public override Gene GenerateGene(int geneIndex)
        {
            int smallMachines = 100;
            int bigMachines = 100;

            while (smallMachines + bigMachines > 5)
            {
                smallMachines = rng.Next(6);
                bigMachines = rng.Next(6);
            }

            return new Gene((smallMachines, bigMachines));
        }

        internal SubmitSolution ToSolution(MapData mapData)
        {
            SubmitSolution result = new SubmitSolution()
            {
                Locations = new Dictionary<string, PlacedLocations>(mapData.locations.Count())
            };

            for (int i = 0; i < mapData.locations.Count(); i++)
            {
                result.Locations.Add(mapData.locations[i])
            }

        }
    }
}
