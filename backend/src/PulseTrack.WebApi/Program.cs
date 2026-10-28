using PulseTrack.Application.Interfaces;
using PulseTrack.Infrastructure;
using PulseTrack.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PulseTrack Fleet & Collision Operations API",
        Version = "v1",
        Description = "High-Throughput Work Order, Real-Time Shop Lifecycle, and Low-Latency Parts Caching API."
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPulseFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var woRepo = scope.ServiceProvider.GetRequiredService<IWorkOrderRepository>();
    var partsRepo = scope.ServiceProvider.GetRequiredService<IPartsCatalogRepository>();
    await DataSeeder.SeedAsync(woRepo, partsRepo);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PulseTrack API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowPulseFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
