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
        private readonly string VegasBaseLink = "https://www.parlaysavant.com/fantasy/vegas-rankings/week-17/";
        private readonly string DraftSharksBaseLink = "https://www.draftsharks.com/weekly-rankings";

        public ScraperService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task ScrapeDraftSharks()
        {
            string html = await GetDraftSharksHtml();
        }

        private async Task<string> GetDraftSharksHtml()
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

        public async Task ScrapeVegas()
        {
            throw new NotImplementedException();
        }
    }
}
