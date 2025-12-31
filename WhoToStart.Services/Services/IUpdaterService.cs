using System;
using System.Collections.Generic;
using System.Text;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Services
{
    public interface IUpdaterService
    {
        Task UpdateProjections();
    }
}
