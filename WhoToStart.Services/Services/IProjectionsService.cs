using System;
using System.Collections.Generic;
using System.Text;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Services
{
    public interface IProjectionsService
    {
        public Task<Projection> GetProjectionByID(int id);
    }
}
