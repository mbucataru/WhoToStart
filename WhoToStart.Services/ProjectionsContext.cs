using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace WhoToStart.Services
{
    internal class ProjectionsContext : DbContext
    {
        public DbSet<Projection> Projections { get; set; }

        public ProjectionsContext(DbContextOptions<ProjectionsContext> options) : base(options)
        {
            
        }
    }
}
