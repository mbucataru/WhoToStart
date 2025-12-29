using HtmlAgilityPack;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WhoToStart.Services
{
    public class ScraperService : IScraperService
    {
        private readonly IHttpClientFactory _factory;
        private readonly string VegasBaseLink = "https://www.firstdown.studio/rankings/";
        private readonly string DraftSharksBaseLink = "https://www.draftsharks.com/weekly-rankings";
        private static readonly string[] Positions = { "QB", "FLEX", "K" };

        public ScraperService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task ScrapeDraftSharksAsync()
        {
            string html = await GetDraftSharksHtmlAsync();
        }

        public async Task<string> GetDraftSharksHtmlAsync()
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

        public List<Projection> ParseDraftSharksHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var projections = new List<Projection>();
            var playerRows = doc.DocumentNode.SelectNodes("//tbody[@data-player-row]");

            // Used to trim Ranking away from the Position field.
            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            foreach (var row in playerRows)
            {
                string position = row.SelectSingleNode(".//div[contains(@class, 'position-rank')]").InnerText.Trim().TrimEnd(digits);
                // I don't have the slighest idea of why this is needed. I don't see these TQB's when I open the site normally, but in my requests I do.
                if (position == "TQB")
                    continue;

                var projection = new Projection
                {
                    Name = row.SelectSingleNode(".//a[@class='hide-on-mobile']").InnerText.Trim(),
                    Team = row.SelectSingleNode(".//div[@class='team-position-logo-container']/span").InnerText.Trim(),
                    Position = position,
                    DraftSharksProjection = double.Parse(row.SelectSingleNode(".//td[contains(@class, 'three-d-proj')]//span[@class='column-title']").InnerText.Trim()),
                    Week = 0, // BROKEN
                    VegasProjection = 0,
                    FinalProjection = 0
                };

                projections.Add(projection);
            }

            return projections;
        }

        public async Task ScrapeVegasAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> GetVegasHtmlAsync()
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
    }
}
