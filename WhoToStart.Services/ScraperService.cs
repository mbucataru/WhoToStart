using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.Playwright;

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

            while (await page.Locator("div.column-title.rank-index:has-text('250')").CountAsync() == 0)
            {
                await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                await Task.Delay(500);
            }

            var html = await page.ContentAsync();

            return html;
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
