//using GSB.Test.Api.Configurations;
using GSB.Test.Api.Configurations;
using GSB.Test.Api.Data;
using GSB.Test.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<GsbSettings>(
    builder.Configuration.GetSection("GSB"));

builder.Services.AddHttpClient("GSB", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);

    client.DefaultRequestHeaders.Accept.Clear();

    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IGsbService, GsbService>();

builder.Services.AddScoped<IRegistrationCallbackService, RegistrationCallbackService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
    {
        options.UseInMemoryDatabase("GSBRegistrationDb-Test");
        return;
    }

    _ = options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
