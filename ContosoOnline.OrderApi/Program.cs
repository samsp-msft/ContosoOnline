using ContosoOnline.Common;
using ContosoOnline.OrderApi;
using ContosoOnline.OrderApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDbContext") ?? throw new InvalidOperationException("Connection string 'OrderDbContext' not found.")));
builder.Services.AddHostedService<DatabaseInitializer<OrderDbContext>>();
builder.EnrichSqlServerDbContext<OrderDbContext>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomMiddleware();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map our custom HTTP API endpoints
app.MapCartEndpoints();
app.MapOrderEndpoints();

app.Run();
