using Considition2023_Cs.Game;
using Considition2023_Cs.Genetics.NormalMap;
using Considition2023_Cs.Genetics.SandboxMap;
using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics
{
    internal class SandboxFitnessRunner
    {
        public static SubmitSolution RunEvolution(MapData mapdata, GeneralData generalData,int populationMinSize, int populationMaxSize, int runs, SandboxSolutionChromosome firstChromosome = null)
        {
            
                var selection = new EliteSelection((int)Math.Floor(0.6*populationMinSize) );
              //  var selection = new RouletteWheelSelection();
                var crossover = new SandboxUniformCrossover();

                var mutation = new SandboxUniformMutation(true);

                var fitness = new SandboxSolutionFitnessFunction(mapdata, generalData);
                firstChromosome ??= new SandboxSolutionChromosome(mapdata, generalData);

                var population = new Population(populationMinSize, populationMaxSize, firstChromosome);

                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            //ga.Termination = new Terminator(2415); //uppsala
            //ga.Termination = new Terminator(56826753); //göteborg
            //ga.Termination = new GenerationNumberTermination(10);
            ga.Termination = new FitnessStagnationTermination(runs);
            ga.GenerationRan += (s, e) =>
                    Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness.Value}");

            Console.WriteLine("GA running...");
            ga.TaskExecutor = new ParallelTaskExecutor();
            ga.Start();

                Console.WriteLine();
                Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
                Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");

            return ((SandboxSolutionChromosome)ga.BestChromosome).ToSolution(mapdata);
        }
    }
}
