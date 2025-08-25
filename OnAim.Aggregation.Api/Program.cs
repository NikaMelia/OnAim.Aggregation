using Microsoft.EntityFrameworkCore;
using OnAim.Aggregation.Api;
using OnAim.Aggregation.Application;
using OnAim.Aggregation.Application.Repositories;
using OnAim.Aggregation.Application.Services.ClientServerEventProducer;
using OnAim.Aggregation.Application.Transactions;
using OnAim.Aggregation.Infrastructure;
using OnAim.Aggregation.Persistence;
using OnAim.Aggregation.Persistence.DbContexts;
using OnAim.Aggregation.Persistence.Repositories;
using OnAim.Aggregation.Persistence.Transactions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddScoped<IAggregationConfigRepository, AggregationConfigRepository>();
builder.Services.AddScoped<IAggregationResultsRepository, AggregationResultsRepository>();
builder.Services.AddScoped<ITransactionalSession, TransactionalSession>();
builder.Services.AddApplication();
builder.Services.AddApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IEventBus, CapEventBus>();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHostedService<StartupHostedService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aggregator API v1");
    c.DocumentTitle = "Aggregator API";
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapControllers();
app.Run();
