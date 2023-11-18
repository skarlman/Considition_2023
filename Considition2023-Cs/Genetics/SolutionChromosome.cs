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
            var result = new SolutionChromosome(this.Length);
            result.CreateGenes();

            return result;
        }
        Random rng = new Random();
        public override Gene GenerateGene(int geneIndex)
        {
            int smallMachines = rng.Next(3);
            int bigMachines = rng.Next(3);

            return new Gene((smallMachines, bigMachines));
        }


        public static SolutionChromosome CreateAdamChromosome(int length)
        {
            var result = new SolutionChromosome(length);
            for (int i = 0; i < length; i++)
            {
                result.ReplaceGene(i, new Gene((1, 0)));
            }
            return result;
        }

        internal SubmitSolution ToSolution(MapData mapData)
        {
            SubmitSolution result = new SubmitSolution()
            {
                Locations = new Dictionary<string, PlacedLocations>(mapData.locations.Count())
            };

            var ctr = 0;
            var allLocations = mapData.locations.AsEnumerable();
            foreach (var item in allLocations)
            {

                PlacedLocations value = new PlacedLocations
                {
                    Freestyle3100Count = (((int, int))GetGene(ctr).Value).Item1,
                    Freestyle9100Count = (((int, int))GetGene(ctr).Value).Item2
                };
                ctr++;

                if (value.Freestyle3100Count > 0 || value.Freestyle9100Count > 0) {
                    result.Locations.Add(item.Key,value);
                }
            }

            return result; 
        }   
    }
    public class Terminator : ITermination
    {
        private double _scoreToExceed;

        public Terminator(double scoreToExceed)
        {
            _scoreToExceed = scoreToExceed;
        }

        public bool HasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            return geneticAlgorithm.BestChromosome.Fitness > _scoreToExceed;
        }
    }
}
