using Considition2023_Cs.Game;
using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics.NormalMap
{
    internal class FitnessRunner
    {
        static DateTime LastSubmit = DateTime.MinValue;
        static double LastSubmitScore = double.MinValue;
        static object SubmissionLock = new object();
        public static SubmitSolution RunEvolution(MapData mapdata, GeneralData generalData, int populationMinSize, int populationMaxSize, int runs, Api submissionApi, SolutionChromosome firstChromosome = null, bool isSandboxMap = false )
        {

            var selection = new EliteSelection((int)Math.Floor(0.6 * populationMinSize));
            //  var selection = new RouletteWheelSelection();
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation(true);

            var fitness = new SolutionFitnessFunction(mapdata, generalData);
            firstChromosome ??= new SolutionChromosome(isSandboxMap ? mapdata.Hotspots.Count() : mapdata.locations.Count());

            var population = new Population(populationMinSize, populationMaxSize, firstChromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            //ga.Termination = new Terminator(2415); //uppsala
            //ga.Termination = new Terminator(56826753); //göteborg

            ga.Termination = new FitnessStagnationTermination(runs);
            ga.GenerationRan += (s, e) =>
            {
                if ((DateTime.Now - LastSubmit).TotalSeconds > GlobalUtils.secondsBetweenApiSubmits)
                {
                    Console.WriteLine($"Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness.Value}");

                    lock (SubmissionLock)
                    {
                        if ((DateTime.Now - LastSubmit).TotalSeconds > GlobalUtils.secondsBetweenApiSubmits)
                        {
                            LastSubmit = DateTime.Now;
                            if (LastSubmitScore < ga.BestChromosome.Fitness.Value)
                            {
                                try
                                {
                                    Console.WriteLine($"Last submission score [{LastSubmitScore}] < New best  [{ga.BestChromosome.Fitness.Value}], Submitting to API");
                                    submissionApi.SumbitAsync(mapdata.MapName, ((SolutionChromosome)ga.BestChromosome).ToSolution(mapdata), GlobalUtils.apiKey).Wait();
                                    LastSubmitScore = ga.BestChromosome.Fitness.Value;
                                    Console.WriteLine($"New best submitted score: [{LastSubmitScore}]");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Exception when submitting solution: {ex.Message}");
                                }
                            }
                        }

                    }
                }
              };
            Console.WriteLine("GA running...");
            ga.TaskExecutor = new ParallelTaskExecutor();
            ga.Start();

            Console.WriteLine();
            Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
            Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");

            return ((SolutionChromosome)ga.BestChromosome).ToSolution(mapdata);
        }
    }
}
