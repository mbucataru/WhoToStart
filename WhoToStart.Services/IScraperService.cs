using System;
using System.Collections.Generic;
using System.Text;

namespace WhoToStart.Services
{
    public interface IScraperService
    {
        public Task ScrapeDraftSharksAsync();
        public Task ScrapeVegasAsync();

        /// <summary>
        ///  The method that returns DraftSharks Rankings.
        /// </summary>
        /// <returns>A string containing the HTML of the current week's DraftSharks rankings</returns>
        
        public Task<string> GetDraftSharksHtmlAsync();

        /// <summary>
        /// The method that returns Vegas Rankings for each position
        /// </summary>
        /// <returns>An array (length 3) of strings, each string containing the Html for one position's rankings</returns>
        public Task<string[]> GetVegasHtmlAsync();    
    }
}
