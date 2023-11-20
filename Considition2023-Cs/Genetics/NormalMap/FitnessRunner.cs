using GeneticSharp;
using Shared;
using Shared.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            //SubmitSolution result = new() { Locations = new()};

            var fitness = new SolutionFitnessFunction(mapdata, generalData);
            
            firstChromosome ??= new SolutionChromosome(mapdata.locations.Count());

            //SolutionChromosome bestChromosomeSoFar = firstChromosome;
            
            //for (int i = 0; i < 100; i++)
            //{
                //firstChromosome = bestChromosomeSoFar;
                IPopulation population = new Population(populationMinSize, populationMaxSize, firstChromosome);
                
                ISelection selection = new EliteSelection((int)Math.Floor(0.3 * populationMinSize));
            ICrossover crossover = new UniformCrossover();
            IMutation mutation = new UniformMutation(true);
            //Console.WriteLine($" -- StartingGeneration {population.GenerationsNumber} Best Score: {bestChromosomeSoFar?.Fitness ?? -1} --");

            //ISelection selection = i%2==0 
            //        ?  ChooseSelection(populationMinSize)
            //        : new EliteSelection((int)Math.Floor(0.3 * populationMinSize));
            //    Console.WriteLine($" New Selection: {selection.GetType().Name}");


            //ICrossover crossover = i % 2 == 0
            //        ? ChooseCrossover()
            //        : new UniformCrossover();
            //    Console.WriteLine($" New Crossover: {crossover.GetType().Name}");

            //IMutation mutation = i % 2 == 0
            //        ? ChooseMutation()
            //        : new UniformMutation(true);
            //    Console.WriteLine($" New Mutation: {mutation.GetType().Name}");


                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
                ga.Reinsertion = new ElitistReinsertion() ;
                ga.Termination = new FitnessStagnationTermination(runs);
                ga.GenerationRan += (s, e) =>
                {
                    UpdateStatusAndSubmit(mapdata, submissionApi, ga);
                };
                Console.WriteLine("GA running...");
                //ga.TaskExecutor = new ParallelTaskExecutor();
                ga.Start();

                Console.WriteLine();
                Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
                Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");
                //bestChromosomeSoFar = bestChromosomeSoFar.Fitness.HasValue && bestChromosomeSoFar.Fitness > ((SolutionChromosome)ga.BestChromosome).Fitness
                //    ? bestChromosomeSoFar
                //    : (SolutionChromosome)ga.BestChromosome;
                    //? (((SolutionChromosome)ga.BestChromosome.Clone()))
                    //: bestChromosomeSoFar;
                //result = ((SolutionChromosome)ga.BestChromosome).ToSolution(mapdata);
            //}


            return ((SolutionChromosome)ga.BestChromosome).ToSolution(mapdata);
            }

        private static void UpdateStatusAndSubmit(MapData mapdata, Api submissionApi, GeneticAlgorithm ga)
        {
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
        }

        private static IMutation ChooseMutation()
        {
            var chosen = new List<IMutation>()
            {
                new DisplacementMutation(),
                new InsertionMutation(),
                new PartialShuffleMutation(),
                new ReverseSequenceMutation(),
                new TworsMutation(),

            }.OrderBy(_ => Guid.NewGuid()).First();

            return chosen;
        }

        private static ICrossover ChooseCrossover()
        {
            //  var selection = new RouletteWheelSelection();
            ICrossover chosen = new List<ICrossover>() {
                //new AlternatingPositionCrossover(),
                //new CutAndSpliceCrossover(),
                //new CycleCrossover(),
                new OnePointCrossover(),
                //new OrderedCrossover(),
                //new PartiallyMappedCrossover(),
                //new PositionBasedCrossover(),
                new ThreeParentCrossover(),
                new TwoPointCrossover(),
                new VotingRecombinationCrossover()
            }.OrderBy(_ => Guid.NewGuid()).First();

            return chosen;
        }

        private static ISelection ChooseSelection(int populationMinSize)
        {

            ISelection chosen = new List<ISelection>() {

            new RankSelection(),
            new RouletteWheelSelection(),
            new StochasticUniversalSamplingSelection(),
            new TournamentSelection(),
            new TruncationSelection() }
            .OrderBy(_ => Guid.NewGuid())
            .First();

            return chosen;
            
        }
    }
}
