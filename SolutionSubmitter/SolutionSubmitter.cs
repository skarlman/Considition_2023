using Hangfire;
using Shared;
using Shared.Game;

namespace SolutionSubmitter
{
    public class SolutionSubmitter
    {
        public void SubmitSolutionToServer(string mapName, SubmitSolution solution)
        {
            BackgroundJob.Enqueue(()  => { new Api(new HttpClient()).SumbitAsync(mapName, solution, GlobalUtils.apiKey)})
        }
    }
}
