using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WhoToStart.Services.Data;
using WhoToStart.Services.Services;

var services = new ServiceCollection();
services.AddDbContext<WhoToStartDbContext>(options => options.UseSqlite("DataSource=whotostart.db"));
services.AddScoped<IUpdaterService, UpdaterService>();

var provider = services.BuildServiceProvider();
var scraper = provider.GetRequiredService<IUpdaterService>();

await scraper.UpdateProjections();