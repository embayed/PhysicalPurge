using System;
using Intalio.Storage.FileSystem.Core;
using Intalio.Storage.FileSystem.Core.API;
using Intalio.Storage.Interface;
using Microsoft.EntityFrameworkCore;
using PhysicalStoragePurge.Data;
using PhysicalStoragePurge.Repositories;
using PhysicalStoragePurge.Services;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// CORS
const string CorsPolicyName = "AllowFrontend4141";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:4141")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add DbContext
builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DmsDb")));

builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IPhysicalPurgeService, PhysicalPurgeService>();
builder.Services.AddScoped<ManageStorage>();


Configuration.DbConnectionString = builder.Configuration.GetConnectionString("StorageDb");
Configuration.DatabaseType = DatabaseType.PostgreSQL;
Configuration.IsConnectionStringEncrypted = false;





// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();


app.UseAuthorization();

// Apply CORS BEFORE controllers
app.UseCors(CorsPolicyName);

app.MapGet("/", () => Results.Redirect("/swagger"));


app.MapControllers();

app.Run();
