using Microsoft.EntityFrameworkCore;
using WhoToStart.Services.Data;
using WhoToStart.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUpdaterService, UpdaterService>();
builder.Services.AddDbContext<WhoToStartDbContext>(options => options.UseSqlite("Data Source=whotostart.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
