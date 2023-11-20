using Hangfire;
using Shared;
using Shared.Game;

namespace SolutionSubmitter
{
    public class SolutionProcessor(Api submissionApi)
    {
        
        public async Task ProcessSubmissionAsync(string mapName, SubmitSolution solution)
        {
            double? score = null;

            try
            {
                var mapEntity = await submissionApi.GetMapDataAsync(mapName, GlobalUtils.apiKey);
                var generalData = await submissionApi.GetGeneralDataAsync();
                score = Scoring.CalculateScore(mapName, solution, mapEntity, generalData).GameScore.Total;

            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Exception when calculating score: {ex.Message}");
            }

            BackgroundJob.Enqueue(() => submissionApi.SumbitAsync(mapName, solution, GlobalUtils.apiKey, score));


        }
    }
}
