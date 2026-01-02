using System;
using System.Collections.Generic;
using System.Text;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Services
{
    public interface IUpdaterService
    {
        /// <summary>
        /// Updates the DraftSharks and Vegas rankings in the application to be up-to-date.
        /// </summary>
        /// <returns></returns>
        Task UpdateProjections();
    }
}
