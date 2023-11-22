using GeneticSharp;
using Shared;
using Shared.Game;

namespace Considition2023_Cs.Genetics.NormalMap;

internal class FitnessRunner
{
    private static DateTime LastSubmit = DateTime.MinValue;
    private static double LastSubmitScore = double.MinValue;
    private static readonly object SubmissionLock = new();

    public static SubmitSolution RunEvolution(MapData mapdata, GeneralData generalData, int populationMinSize,
        int populationMaxSize, int runs, Api submissionApi, Api backupApi, SolutionChromosome firstChromosome = null,
        bool isSandboxMap = false)
    {
        var fitness = new SolutionFitnessFunction(mapdata, generalData);

        firstChromosome ??= new SolutionChromosome(mapdata.locations.Count());


        IPopulation population = new Population(populationMinSize, populationMaxSize, firstChromosome);

        ISelection selection = new EliteSelection((int)Math.Floor(0.6 * populationMinSize));
        ICrossover crossover = new UniformCrossover();
        IMutation mutation = new UniformMutation(true);


        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        ga.Reinsertion = new ElitistReinsertion();


        ga.Termination = new FitnessStagnationTermination(runs);
        //ga.Termination = new Genetics.Terminator(5500);


        ga.GenerationRan += (s, e) => { UpdateStatusAndSubmit(mapdata, submissionApi, backupApi, ga); };
        Console.WriteLine("GA running...");
        ga.TaskExecutor = new ParallelTaskExecutor();
        ga.Start();

        Console.WriteLine();
        Console.WriteLine($"Best solution found has fitness: {ga.BestChromosome.Fitness}");
        Console.WriteLine($"Elapsed time: {ga.TimeEvolving}");


        return ((SolutionChromosome)ga.BestChromosome).ToSolution(mapdata);
    }

    private static void UpdateStatusAndSubmit(MapData mapdata, Api submissionApi, Api backupApi, GeneticAlgorithm ga)
    {
        if ((DateTime.Now - LastSubmit).TotalSeconds > GlobalUtils.secondsBetweenApiSubmits)
        {
            Console.WriteLine(
                $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] Generation {ga.GenerationsNumber}. Best fitness: {ga.BestChromosome.Fitness.Value}");

            Console.Title = $"{mapdata.MapName} [{ga.BestChromosome.Fitness.Value}]";

            lock (SubmissionLock)
            {
                if ((DateTime.Now - LastSubmit).TotalSeconds > GlobalUtils.secondsBetweenApiSubmits)
                {
                    LastSubmit = DateTime.Now;
                    if (LastSubmitScore < ga.BestChromosome.Fitness.Value)
                        try
                        {
                            Console.WriteLine(
                                $"Last submission score [{LastSubmitScore}] < New best  [{ga.BestChromosome.Fitness.Value}], Submitting to API");
                            submissionApi.SumbitAsync(mapdata.MapName,
                                ((SolutionChromosome)ga.BestChromosome).ToSolution(mapdata), GlobalUtils.apiKey).Wait();
                            LastSubmitScore = ga.BestChromosome.Fitness.Value;
                            Console.WriteLine($"New best submitted score: [{LastSubmitScore}]");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Exception when submitting solution, retrying with backup API: {ex.Message}");

                            try
                            {
                                backupApi.SumbitAsync(mapdata.MapName,
                                    ((SandboxSolutionChromosome)ga.BestChromosome).ToSolution(mapdata),
                                    GlobalUtils.apiKey).Wait();
                                LastSubmitScore = ga.BestChromosome.Fitness.Value;
                                Console.WriteLine($"New best submitted score: [{LastSubmitScore}]");
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine(
                                    $" Exception when submitting solution with backup API, giving up: {ex.Message}");
                            }
                        }
                }
            }
        }
    }

    private static IMutation ChooseMutation()
    {
        var chosen = new List<IMutation>
        {
            new DisplacementMutation(),
            new InsertionMutation(),
            new PartialShuffleMutation(),
            new ReverseSequenceMutation(),
            new TworsMutation()
        }.OrderBy(_ => Guid.NewGuid()).First();

        return chosen;
    }

    private static ICrossover ChooseCrossover()
    {
        //  var selection = new RouletteWheelSelection();
        var chosen = new List<ICrossover>
        {
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
        var chosen = new List<ISelection>
            {
                new RankSelection(),
                new RouletteWheelSelection(),
                new StochasticUniversalSamplingSelection(),
                new TournamentSelection(),
                new TruncationSelection()
            }
            .OrderBy(_ => Guid.NewGuid())
            .First();

        return chosen;
    }
}