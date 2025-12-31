using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using WhoToStart.Services.Data;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Services
{
    public class UpdaterService : IUpdaterService
    {
        private readonly WhoToStartDbContext _context;
        private readonly string VegasBaseLink = "https://www.firstdown.studio/rankings/";
        private readonly string DraftSharksBaseLink = "https://www.draftsharks.com/weekly-rankings";
        private static readonly string[] Positions = { "QB", "FLEX", "K" };

        public UpdaterService(WhoToStartDbContext context)
        {
            _context = context;
        }

        public async Task UpdateProjections()
        {
            await UpdateDraftSharksProjections();
            await UpdateVegasProjections();
        }
        
        internal async Task UpdateDraftSharksProjections()
        {
            string html = await ScrapeDraftSharksHtml();
            await ProcessDraftSharksHtml(html);
        }

        internal async Task UpdateVegasProjections()
        {
            string[] html = await ScrapeVegasHtml();
            ProcessVegasHtml(html);
            throw new NotImplementedException();
        }

        internal async Task<string> ScrapeDraftSharksHtml()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync();

            var page = await browser.NewPageAsync();

            await page.GotoAsync(DraftSharksBaseLink);


            // DS doesn't load the page contents right away and the max number of rankings is 250. This says "scroll down until you can see all of the rankings."
            while (await page.Locator("div.column-title.rank-index:has-text('250')").CountAsync() == 0)
            {
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Task.Delay(500);
            }

            var html = await page.ContentAsync();

            return html;
        }

        // This also probably shouldn't return a list. Maybe this should return success / fail?
        internal async Task ProcessDraftSharksHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var playerRows = doc.DocumentNode.SelectNodes("//tbody[@data-player-row]");

            foreach (var row in playerRows)
            {
                Projection? newProjection = ParseDraftSharksProjection(row);

                if (newProjection is null)
                    continue;

                Projection? existingRecord = await _context.Projections.FirstOrDefaultAsync(existingProj => 
                    existingProj.Name == newProjection.Name && 
                    existingProj.Position == newProjection.Position &&
                    existingProj.Team == newProjection.Team);

                if (existingRecord is null)
                {
                    await _context.Projections.AddAsync(newProjection);
                }
                else
                {
                    existingRecord.DraftSharksProjection = newProjection.DraftSharksProjection;
                }
            }
        }
        
        internal Projection? ParseDraftSharksProjection(HtmlNode row)
        {
            // Used to trim Ranking away from the Position field.
            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            string position = row.SelectSingleNode(".//div[contains(@class, 'position-rank')]").InnerText.Trim().TrimEnd(digits);

            // I don't have the slighest idea of why this is needed. I don't see these TQB's when I open the site normally, but in my requests I do.
            if (position == "TQB")
                return null;

            var newProjection = new Projection
            {
                Name = row.SelectSingleNode(".//a[@class='hide-on-mobile']").InnerText.Trim(),
                Team = row.SelectSingleNode(".//div[@class='team-position-logo-container']/span").InnerText.Trim(),
                Position = position,
                DraftSharksProjection = double.Parse(row.SelectSingleNode(".//td[contains(@class, 'three-d-proj')]//span[@class='column-title']").InnerText.Trim()),
                Week = 0, // BROKEN
                VegasProjection = 0,
                FinalProjection = 0
            };

            return newProjection;
        }

        internal async Task<string[]> ScrapeVegasHtml()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync();

            string[] returnArray = new string[Positions.Length];

            var page = await browser.NewPageAsync();

            for (int i = 0; i < Positions.Length; i++)
            {
                string position = Positions[i];

                await page.GotoAsync(VegasBaseLink + position);

                var html = await page.ContentAsync();

                returnArray[i] = html;
            }

            return returnArray;
        }

        internal void ProcessVegasHtml(string[] html)
        {
            throw new NotImplementedException();
        }
    }
}
