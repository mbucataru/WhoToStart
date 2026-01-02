using Microsoft.AspNetCore.Mvc;
using WhoToStart.Services.Models;
using WhoToStart.Services.Services;

namespace WhoToStart.API.Controllers
{

    // This scraper will most likely end up just having one route: Update, and will be a background job.
    [ApiController]
    [Route("[controller]")]
    public class UpdaterController : ControllerBase
    {
        private readonly IUpdaterService _updater;

        public UpdaterController(IUpdaterService updater)
        {
            _updater = updater;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProjections()
        {
            await _updater.UpdateProjections();
            return NoContent();
        }
    }
}
