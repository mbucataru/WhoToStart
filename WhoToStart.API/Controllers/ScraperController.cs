using Microsoft.AspNetCore.Mvc;
using WhoToStart.Services.Models;
using WhoToStart.Services.Services;

namespace WhoToStart.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IUpdaterService _updater;

        public ScraperController(IUpdaterService scraper)
        {
            _updater = scraper;
        }
        [HttpGet("test/draftsharks/scraper")]
        public async Task<IActionResult> TestDraftSharksScraper()
        {
            var html = await _updater.ScrapeDraftSharksHtmlAsync();
            return Ok(html);
        }

        [HttpGet("test/vegas")]
        public async Task<IActionResult> TestVegasScraper()
        {
            string[] rankings = await _updater.ScrapeVegasHtmlAsync();

            return Ok(rankings[0]);
        }

        [HttpGet("test/draftsharks/parser")]
        public async Task<IActionResult> TestDraftSharksParser()
        {
            var html = await _updater.ScrapeDraftSharksHtmlAsync();

            List<Projection> projections = await _updater.ProcessDraftSharksHtml(html);

            return Ok(projections);
        }
    }
}
