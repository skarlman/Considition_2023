using Microsoft.AspNetCore.Mvc;
using Shared.Game;

namespace SolutionSubmitter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly SolutionProcessor solutionProcessor;

        public GameController(SolutionProcessor solutionProcessor)
        {
            this.solutionProcessor = solutionProcessor;
        }
        
        [HttpPost("submitSolution")]
        public async Task<IActionResult> SubmitSolution([FromQuery] string mapName, [FromBody] SubmitSolution solution)
        {
            // Your logic to handle the solution submission
            await solutionProcessor.ProcessSubmissionAsync(mapName, solution);

            // Respond with Ok status
            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
