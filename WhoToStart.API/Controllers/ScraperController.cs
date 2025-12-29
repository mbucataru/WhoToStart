using Microsoft.AspNetCore.Mvc;
using WhoToStart.Services;

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
        [HttpGet("test/draftsharks")]
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
    }
}
