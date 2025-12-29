using System;
using System.Collections.Generic;
using System.Text;

namespace WhoToStart.Services
{
    public interface IProjectionsService
    {
        public Task<Projection> GetProjectionByID(int id);
    }
}
