//using GSB.Test.Api.Configurations;
using GSB.Test.Api.Configurations;
using GSB.Test.Api.Data;
using GSB.Test.Api.Services;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);


// Add services to the container.


builder.Services.Configure<GsbSettings>(
    builder.Configuration.GetSection("GSB"));

//builder.Services.AddHttpClient();

builder.Services.AddHttpClient("GSB", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);

    client.DefaultRequestHeaders.Accept.Clear();

    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});



//builder.Services.AddScoped<GsbService>();

builder.Services.AddScoped<IGsbService, GsbService>();

builder.Services.AddScoped<IRegistrationCallbackService, RegistrationCallbackService>();

builder.Services.AddScoped<IRegistrationCallbackService, RegistrationCallbackService>();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    _ = options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
