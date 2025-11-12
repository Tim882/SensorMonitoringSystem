using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SensorProcessor.Data;
using SensorProcessor.Filters;
using SensorProcessor.Repositories;
using SensorProcessor.Services;
using SensorProcessor.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISensorRepository, SensorRepository>();

builder.Services.AddScoped<ISensorService, SensorService>();

builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddValidatorsFromAssemblyContaining<SensorDataDtoValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SensorDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", async (SensorDbContext context) =>
{
    var canConnect = await context.Database.CanConnectAsync();
    var dataCount = await context.SensorData.CountAsync();

    return new
    {
        status = canConnect ? "Healthy" : "Unhealthy",
        timestamp = DateTime.UtcNow,
        service = "Sensor Processor API",
        database = canConnect ? "Connected" : "Disconnected",
        dataCount = dataCount,
        version = "1.0.0"
    };
});

app.Run();