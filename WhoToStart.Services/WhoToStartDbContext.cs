using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace WhoToStart.Services
{
    internal class WhoToStartDbContext : DbContext
    {
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Player> Players { get; set; }

        public WhoToStartDbContext(DbContextOptions<WhoToStartDbContext> options) : base(options)
        {
            
        }
    }
}
