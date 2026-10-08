using GSB.Test.Api.Configurations;
using GSB.Test.Api.Data;
using GSB.Test.Api.Services;
using Microsoft.EntityFrameworkCore;
using GSB.Test.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GsbSettings>(
    builder.Configuration.GetSection("GSB"));

builder.Services.Configure<MsbSettings>(
    builder.Configuration.GetSection("MSB"));

builder.Services.AddHttpClient("GSB", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("MSB", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IGsbService, GsbService>();
builder.Services.AddScoped<IMsbService, MsbService>();
builder.Services.AddScoped<IDocumentVerificationService, DocumentVerificationService>();
builder.Services.AddScoped<IPoaInquiryService, PoaInquiryService>();
builder.Services.AddScoped<IRegistrationCallbackService, RegistrationCallbackService>();
builder.Services.AddScoped<IMsbCallbackService, MsbCallbackService>();
builder.Services.AddScoped<IRawMsbCallbackService, RawMsbCallbackService>();
builder.Services.AddScoped<IUniqueIdentifierCallbackService, UniqueIdentifierCallbackService>();
builder.Services.AddScoped<IPoaEvaluationCallbackService, PoaEvaluationCallbackService>();
builder.Services.AddScoped<IMsbInvocationLogger, MsbInvocationLogger>();
builder.Services.AddHttpContextAccessor();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<MsbInvocationLoggingMiddleware>();
app.MapControllers();
app.Run();
