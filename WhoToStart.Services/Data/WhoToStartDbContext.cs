using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WhoToStart.Services.Models;

namespace WhoToStart.Services.Data
{
    public class WhoToStartDbContext : DbContext
    {
        public DbSet<Projection> Projections { get; set; }

        public WhoToStartDbContext(DbContextOptions<WhoToStartDbContext> options) : base(options)
        {
            
        }
    }
}
