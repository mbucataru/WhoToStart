using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using System.Runtime.CompilerServices;
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
            await UpdateFinalProjections();
            
        }
        
        internal async Task UpdateDraftSharksProjections()
        {
            string html = await ScrapeDraftSharksHtml();
            await ProcessDraftSharksHtml(html);
            await _context.SaveChangesAsync();
        }

        internal async Task UpdateVegasProjections()
        {
            string[] html = await ScrapeVegasHtml();
            await ProcessVegasHtml(html);
            await _context.SaveChangesAsync();
        }

        internal async Task UpdateFinalProjections()
        {
            List<Projection> projections = await _context.Projections.ToListAsync();

            foreach (Projection projection in projections)
            {
                if (projection.DraftSharksProjection == 0 && projection.VegasProjection == 0) continue;

                if (projection.DraftSharksProjection == 0 || projection.VegasProjection == 0)
                {
                    projection.FinalProjection = Math.Max(projection.DraftSharksProjection, projection.VegasProjection);
                    continue;
                }

                if (projection.VegasProjection >= projection.DraftSharksProjection)
                {
                    projection.FinalProjection = projection.VegasProjection;
                    continue;
                }
                else
                {
                    double projectionDelta = projection.DraftSharksProjection - projection.VegasProjection;
                    projection.FinalProjection = projection.VegasProjection + (projectionDelta * 0.15);
                }
            }

            await _context.SaveChangesAsync();
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

        internal async Task ProcessVegasHtml(string[] html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html[0]);

            var rows = doc.DocumentNode.SelectNodes("//tbody[@data-slot='table-body']/tr");

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");

                string name = cells[1].SelectSingleNode(".//span[contains(@class, 'font-bold')]").InnerText.Trim();
                string team = cells[1].SelectSingleNode(".//span[contains(@class, 'muted-foreground')]//span[contains(@class, 'font-bold')]").InnerText.Trim();
                double vegasProjection = double.Parse(cells[2].InnerText.Trim());


                /* Our Vegas Rankings source will still post rankings even if they are not sourced from Vegas (they use some random fantasy model)
                 * This ruins the point of scraping the vegas projections because this kind of projection is quite literally not a vegas projection
                 * These non-Vegas stats are displayed as gray text. So, we check if any of the projections for one player is gray, if it is, the Vegas
                 * projection is incomplete, so we should skip it.
                 */
                bool vegasProjectionIsIncomplete = false;
                for (int i = 3; i < cells.Count; i++)
                {
                    var span = cells[i].SelectSingleNode(".//span[contains(@class, 'text-gray')]");
                    if (span != null)
                    {
                        vegasProjectionIsIncomplete = true;
                        break;
                    }
                }

                Projection? existingProjection = await _context.Projections.FirstOrDefaultAsync(projection =>
                projection.Name == name &&
                projection.Team == team &&
                projection.Position == Positions[0]);

                if (existingProjection is null) throw new InvalidOperationException($"DraftSharks projection not found for {name}");

                if (vegasProjectionIsIncomplete) continue;

                existingProjection.VegasProjection = vegasProjection;
            }
        }
    }
}
