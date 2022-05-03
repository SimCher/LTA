using LTA.API.Infrastructure.Ioc;
using LTA.API.Infrastructure.Data.Context;
using LTA.API.Infrastructure.Data.Migrations;
using LTA.API.Infrastructure.Hubs.Hubs;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

var builder = WebApplication.CreateBuilder(args);
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), @"\NLog.config"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 2000000;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "LTA.API", Version = "v1" });
});



builder.Services.AddDbContext<LtaApiContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("LtaApiContext"));
});


DependencyContainer.RegisterServices(builder.Services);

var app = await CreateDatabaseIfNotExistsAsync(builder);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LTA.API v1"));
}

//app.UseHttpsRedirection();

app.UseRouting();



app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/lta");
});


app.Run();

static async Task<WebApplication> CreateDatabaseIfNotExistsAsync(WebApplicationBuilder builder)
{
    var app = builder.Build();
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        await DatabaseInitializer.InitializeAsync(services.GetRequiredService<LtaApiContext>());
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured creating the database.");
    }

    return app;
}