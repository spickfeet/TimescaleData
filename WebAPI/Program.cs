using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebAPI.Controllers;
using WebAPI.Database;
using WebAPI.Database.Repositories.ResultRepository;
using WebAPI.Database.Repositories.ValueRepositories;
using WebAPI.Database.UnitOfWork;
using WebAPI.Features.CalcStatistic;
using WebAPI.Features.UploadCsv.CsvWorker;
using WebAPI.Mapping.Profiles;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<TimescaleDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddScoped<ICsvHelperService, CsvHelperService>();
builder.Services.AddScoped<IStatisticsCalculatorService, StatisticsCalculatorService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IValueRepository, ValueRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ValueProfile>();
    cfg.AddProfile<ResultProfile>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TimescaleDbContext>();
    db.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
