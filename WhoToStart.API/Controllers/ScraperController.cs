using Microsoft.AspNetCore.Mvc;
using WhoToStart.Services.Models;
using WhoToStart.Services.Services;

namespace WhoToStart.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperService _scraper;

        public ScraperController(IScraperService scraper)
        {
            _scraper = scraper;
        }
        [HttpGet("test/draftsharks/scraper")]
        public async Task<IActionResult> TestDraftSharksScraper()
        {
            var html = await _scraper.ScrapeDraftSharksHtmlAsync();
            return Ok(html);
        }

        [HttpGet("test/vegas")]
        public async Task<IActionResult> TestVegasScraper()
        {
            string[] rankings = await _scraper.ScrapeVegasHtmlAsync();

            return Ok(rankings[0]);
        }

        [HttpGet("test/draftsharks/parser")]
        public async Task<IActionResult> TestDraftSharksParser()
        {
            var html = await _scraper.ScrapeDraftSharksHtmlAsync();

            List<Projection> projections = _scraper.ProcessDraftSharksHtml(html);

            return Ok(projections);
        }
    }
}
