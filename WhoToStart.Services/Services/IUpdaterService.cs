using System;
using System.Collections.Generic;
using System.Text;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Services
{
    public interface IUpdaterService
    {
        /// <summary>
        /// Updates the DraftSharks, Vegas, and Final rankings in the application.
        /// </summary>
        /// <returns></returns>
        Task UpdateProjections();
    }
}
