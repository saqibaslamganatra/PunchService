using BBGPunchService.Infrastructure.Service.Handler;
using BBGPunchService.Source.Data;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

var logger = LogManager.GetCurrentClassLogger();

builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddDIService(builder.Configuration);

// Configure logging with NLog
builder.Services.AddSingleton<NLog.ILogger>(NLog.LogManager.GetCurrentClassLogger());



builder.Services.AddDbContext<TargetDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), 
        sqlServerOptions => sqlServerOptions.CommandTimeout(1800)); // 30 minutes in seconds
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
