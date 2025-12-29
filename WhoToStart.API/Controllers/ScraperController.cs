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
        [HttpGet("test")]
        public async Task<IActionResult> TestScraper()
        {
            var html = await _scraper.GetDraftSharksHtml();
            return Ok(html);
        }
    }
}
