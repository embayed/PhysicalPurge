using Microsoft.EntityFrameworkCore;
using PhysicalStoragePurge.Data;

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


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Apply CORS BEFORE controllers
app.UseCors(CorsPolicyName);

app.MapControllers();

app.Run();
