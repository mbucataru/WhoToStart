using System;
using System.Collections.Generic;
using System.Text;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Services
{
    // ScraperService is a terrible name for what this is trying to do. Need to come up with a better one...
    public interface IUpdaterService
    {
        public Task UpdateDraftSharksProjections();
        public Task UpdateVegasProjections();

        /// <summary>
        ///  The method that returns DraftSharks Rankings.
        /// </summary>
        /// <returns>A string containing the HTML of the current week's DraftSharks rankings</returns>
        
        public Task<string> ScrapeDraftSharksHtmlAsync();

        /// <summary>
        /// The method that returns Vegas Rankings for each position
        /// </summary>
        /// <returns>An array (length 3) of strings, each string containing the Html for one position's rankings</returns>
        public Task<string[]> ScrapeVegasHtmlAsync();

        public Task<List<Projection>> ProcessDraftSharksHtml(string html);
    }
}
