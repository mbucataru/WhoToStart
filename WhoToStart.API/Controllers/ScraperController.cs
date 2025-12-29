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
            var html = await _scraper.GetDraftSharksHtmlAsync();
            return Ok(html);
        }

        [HttpGet("test/vegas")]
        public async Task<IActionResult> TestVegasScraper()
        {
            string[] rankings = await _scraper.GetVegasHtmlAsync();

            return Ok(rankings[0]);
        }

        [HttpGet("test/draftsharks/parser")]
        public async Task<IActionResult> TestDraftSharksParser()
        {
            var html = await _scraper.GetDraftSharksHtmlAsync();

            List<Projection> projections = _scraper.ParseDraftSharksHtml(html);

            return Ok(projections);
        }
    }
}
