using System;
using System.Collections.Generic;
using System.Text;

namespace WhoToStart.Services
{
    public interface IScraperService
    {
        public Task ScrapeDraftSharks();
        public Task ScrapeVegas();
        public Task<string> GetDraftSharksHtml();
    }
}
