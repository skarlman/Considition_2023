using Considition2023_Cs.Genetics.NormalMap;
using Considition2023_Cs.Genetics.SandboxMap;
using GeneticSharp;
using Shared;
using Shared.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics
{
    internal class SandboxFitnessRunner
    {
        static DateTime LastSubmit = DateTime.MinValue;
        static double LastSubmitScore = double.MinValue;
        static object SubmissionLock = new object();

        public static SubmitSolution RunEvolution(MapData mapdata, GeneralData generalData,int populationMinSize, int populationMaxSize, int runs, Api submissionApi, Api backupApi, SandboxSolutionChromosome firstChromosome = null)
        {
            
                var selection = new EliteSelection((int)Math.Floor(0.6*populationMinSize) );
              //  var selection = new RouletteWheelSelection();
                var crossover = new SandboxUniformCrossover();

                var mutation = new SandboxUniformMutation(true);

                var fitness = new SandboxSolutionFitnessFunction(mapdata, generalData);
                firstChromosome ??= new SandboxSolutionChromosome(mapdata, generalData);

                var population = new Population(populationMinSize, populationMaxSize, firstChromosome);

                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Reinsertion = new ElitistReinsertion();
            //ga.Termination = new Terminator(2415); //uppsala
            //ga.Termination = new Terminator(56826753); //göteborg
            //ga.Termination = new GenerationNumberTermination(10);
            ga.Termination = new FitnessStagnationTermination(runs);
            ga.GenerationRan += (s, e) => {
                if ((DateTime.Now - LastSubmit).TotalSeconds > GlobalUtils.secondsBetweenApiSubmits)
                {
                    Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness.Value}");
                    Console.Title = $"{mapdata.MapName} [{ga.BestChromosome.Fitness.Value}]";

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
                                    submissionApi.SumbitAsync(mapdata.MapName, ((SandboxSolutionChromosome)ga.BestChromosome).ToSolution(mapdata), GlobalUtils.apiKey).Wait();
                                    LastSubmitScore = ga.BestChromosome.Fitness.Value;
                                    Console.WriteLine($"New best submitted score: [{LastSubmitScore}]");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Exception when submitting solution, retrying with backup API: {ex.Message}");

                                    try
                                    {
                                        submissionApi.SumbitAsync(mapdata.MapName, ((SandboxSolutionChromosome)ga.BestChromosome).ToSolution(mapdata), GlobalUtils.apiKey).Wait();
                                        LastSubmitScore = ga.BestChromosome.Fitness.Value;
                                        Console.WriteLine($"New best submitted score: [{LastSubmitScore}]");
                                    }
                                    catch (Exception ex2)
                                    {
                                        Console.WriteLine($" Exception when submitting solution with backup API, giving up: {ex.Message}");
                                        
                                    }
                                }
                            }
                        }

                    }
                }
            };
            Console.WriteLine("GA running...");
            //ga.TaskExecutor = new ParallelTaskExecutor();
            ga.Start();

                Console.WriteLine();
                Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
                Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");

            return ((SandboxSolutionChromosome)ga.BestChromosome).ToSolution(mapdata);
        }
    }
}
