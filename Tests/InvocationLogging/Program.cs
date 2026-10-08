using System.Net;
using System.Text;
using System.Text.Json;
using GSB.Test.Api.Configurations;
using GSB.Test.Api.Controllers;
using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Middleware;
using GSB.Test.Api.Models.Requests;
using GSB.Test.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

// No SQL connection, real MSB, external packages, or real credentials are used.
var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["MSB:ApiKeyHeaderName"] = "msb-identifier", ["MSB:ApiKey"] = "TEST-API-KEY"
}).Build();
var secretBody = """{"nested":[{"ToKeN":"TEST-TOKEN","password":"TEST-PASSWORD","cookie":"TEST-COOKIE"}],"echo":"TEST-TOKEN TEST-API-KEY","api-key":{"value":"TEST-SECRET"},"secret":123,"safe":7}""";
var sanitizer = new InvocationLogSanitizer(configuration);
var masked = sanitizer.Body(secretBody)!;
foreach (var secret in new[] { "TEST-TOKEN", "TEST-PASSWORD", "TEST-COOKIE", "TEST-SECRET", "TEST-API-KEY" })
    Check(!masked.Contains(secret), "Recursive/cross-field masking");
Check(masked.Contains("MASKED") && masked.Contains("\"safe\":7"), "Safe fields preserved");
Check(!sanitizer.Body("malformed TEST-PASSWORD")!.Contains("TEST-PASSWORD"), "Malformed body omitted");
Check(sanitizer.Body(new string('x', InvocationLogSanitizer.BodyLimit + 1))!.StartsWith("[OMITTED"), "Large body omitted");
Check(sanitizer.Url("https://user:TEST-PASSWORD@example.test/path?token=UNKNOWN-SECRET#UNKNOWN-SECRET") == "https://example.test/path",
    "Referer URL credentials/query/fragment omitted");

var diagnostics = new Diagnostics();
var persistence = new LoggingFailureInterceptor();
var replacement = new SwitchableLogger();
var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
builder.Configuration.Sources.Clear();
builder.Configuration.AddConfiguration(configuration);
builder.Logging.ClearProviders();
builder.Logging.AddProvider(diagnostics);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddControllers().AddApplicationPart(typeof(RegistrationResponseController).Assembly);
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase("TEST-" + TestState.Database)
    .AddInterceptors(persistence));
builder.Services.AddScoped<IRegistrationCallbackService, RegistrationCallbackService>();
builder.Services.AddScoped<IMsbCallbackService, MsbCallbackService>();
builder.Services.AddScoped<IUniqueIdentifierCallbackService, UniqueIdentifierCallbackService>();
builder.Services.AddScoped<IPoaEvaluationCallbackService, PoaEvaluationCallbackService>();
builder.Services.AddScoped<MsbInvocationLogger>();
// The wrapper exercises failures outside the normal logger's own exception boundary.
builder.Services.AddScoped<IMsbInvocationLogger>(sp =>
{
    replacement.Inner = sp.GetRequiredService<MsbInvocationLogger>();
    return replacement;
});
await using var app = builder.Build();
app.UseMiddleware<MsbInvocationLoggingMiddleware>();
app.MapControllers();
await app.StartAsync();
using var client = new HttpClient { BaseAddress = new Uri(app.Services.GetRequiredService<IServer>()
    .Features.Get<IServerAddressesFeature>()!.Addresses.Single()) };

const string regPath = "/made-14/registration-response/v1/create";
const string docPath = "/document-ownership-verification/v1/create";
const string uniquePath = "/made14/send-unique-identifier-response/v1/create";
const string poaPath = "/interagency-inquiry/poa-evaluation-response/v1/create";
const string regBody = """{"organId":"TEST-ORGAN","owTrakingCode":"TEST-REG","status":2,"result":{"code":200,"msg":"TEST"}}""";
const string docBody = """{"organId":"TEST-ORGAN","owTrakingCode":"TEST-DOC","code":200,"data":{"ConfirmDocumentByElectronicInfo":{"Successful":true}}}""";
const string uniqueBody = """{"organId":"TEST-ORGAN","data":{"actionId":"TEST-ACTION","ruleId":"TEST-RULE","activityId":"TEST-ACTIVITY","requestId":"TEST-REQUEST","code":"TEST-CODE","token":"TEST-TOKEN","taskId":"TEST-TASK"},"mapConfirmationTrackingCode":"TEST-MAP","landData":[{"uniqueIdentifier":"TEST-UNIQUE","unitBlockNumber":"1","unitNumber":"1","warehouseNumberList":[],"parkingNumberList":[],"otherAttachmentsList":[]}],"userInfo":[{"name":"TEST","family":"TEST","usertype":"1","nationalCode":"0000000000","totalShare":1,"shareOf":1,"landArseh":1,"landAyan":1}]}""";
const string poaBody = """{"organId":"TEST-ORGAN","owTrakingCode":"TEST-POA","code":200,"data":{"succseed":true,"NationalRegisterNo":"TEST-REGISTER"}}""";

foreach (var (path, body, service) in new[]
{
    (regPath, regBody, "RegistrationResponse"), (docPath, docBody, "DocumentOwnershipVerification"),
    (uniquePath, uniqueBody, "UniqueIdentifierResponse"), (poaPath, poaBody, "PoaEvaluationResponse")
})
{
    var unauthorized = await Send(path, body, false);
    Check(unauthorized.StatusCode == HttpStatusCode.Unauthorized, service + " authentication preserved");
    var denied = (await Logs()).Last();
    Check(denied.HttpStatusCode == 401 && !denied.IsSuccess && denied.CompletedAt.HasValue, "Unauthorized log completed");
    var response = await Send(path, body);
    Check(response.StatusCode == HttpStatusCode.OK, service + " success response");
    using var ack = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    Check(path == uniquePath ? ack.RootElement.GetProperty("status")[0].GetProperty("code").GetInt32() == 100
        : ack.RootElement.GetProperty("code").GetString() == "200" && ack.RootElement.GetProperty("message").GetString() == "OK",
        "Existing ack contract");
    if (path != uniquePath) Check(ack.RootElement.GetProperty("timestamp").GetString()!.Length > 0 &&
        ack.RootElement.GetProperty("msbTrackingCode").GetString()!.Length > 0, "Existing ack metadata");
    var log = (await Logs()).Last();
    Check(log.ServiceName == service && log.Direction == "Inbound" && log.Endpoint == path &&
        log.HttpStatusCode == 200 && log.IsSuccess && log.CompletedAt.HasValue && log.DurationMs >= 0,
        "Begin/complete updates same log");
    Check(log.OrganId == "TEST-ORGAN" && log.SenderSystem == "TEST-SENDER" && log.RemoteIpAddress is not null &&
        log.ForwardedFor == "203.0.113.10" && log.CorrelationId == "TEST-CORRELATION", "Request metadata");
    Check(log.RemoteIpAddress != log.ForwardedFor, "Forwarded headers do not change remote IP");
    Check(log.Referer == "https://example.test/source", "Referer credentials stripped");
    Check(!JsonSerializer.Serialize(log).Contains("TEST-API-KEY"), "No API key persisted");
}
Check((await Logs()).Single(x => x.ServiceName == "UniqueIdentifierResponse" && x.IsSuccess)
    .RequestBodyMasked!.Contains(InvocationLogSanitizer.Mask), "Unique token masked");
await WithDb(async db =>
{
    Check(await db.SanadCallbacks.CountAsync() == 2 && await db.UniqueIdentifierCallbacks.CountAsync() == 1 &&
        await db.PoaEvaluationCallbacks.CountAsync() == 1, "Business storage unchanged");
    Check((await db.UniqueIdentifierCallbacks.SingleAsync()).RawJson.Contains("TEST-TOKEN"), "Business payload unchanged");
    Check((await db.PoaEvaluationCallbacks.SingleAsync()).RawJson == poaBody, "POA raw body unchanged");
});

foreach (var path in new[] { docPath, poaPath })
{
    var errorAck = await Send(path, """{"organId":"TEST-ORGAN","owTrakingCode":"TEST-ERROR","code":201,"error":{"errorCode":900,"errorMessage":"TEST"}}""");
    Check(errorAck.StatusCode == HttpStatusCode.OK, "201/error business path unchanged");
    var invalid = await Send(path, """{"organId":"TEST-ORGAN","owTrakingCode":"TEST-INVALID","code":200}""");
    Check(invalid.StatusCode == HttpStatusCode.BadRequest && (await Logs()).Last().HttpStatusCode == 400,
        "Missing data validation/logging unchanged");
}
var badJson = await Send(poaPath, "{\"token\":\"TEST-MALFORMED-SECRET");
Check(badJson.StatusCode == HttpStatusCode.BadRequest && !(await Logs()).Last().RequestBodyMasked!.Contains("TEST-MALFORMED-SECRET"),
    "Malformed JSON does not leak secrets");

// Required fields listed in the registration PDF must fail before business persistence.
foreach (var field in new[] { "organId", "owTrakingCode", "status", "result", "result.code", "result.msg" })
{
    var body = System.Text.Json.Nodes.JsonNode.Parse(regBody)!.AsObject();
    var parts = field.Split('.');
    if (parts.Length == 1) body.Remove(field);
    else body[parts[0]]!.AsObject().Remove(parts[1]);
    var invalid = await Send(regPath, body.ToJsonString());
    Check(invalid.StatusCode == HttpStatusCode.BadRequest, "Registration required PDF field rejected");
}

// Unique-identifier behavior remains limited to specified required fields, not inferred business rules.
foreach (var field in new[] { "usertype", "nationalCode", "name", "family", "totalShare", "shareOf", "landArseh", "landAyan" })
{
    var body = System.Text.Json.Nodes.JsonNode.Parse(uniqueBody)!;
    body["userInfo"]![0]!.AsObject().Remove(field);
    var invalid = await Send(uniquePath, body.ToJsonString());
    using var ack = JsonDocument.Parse(await invalid.Content.ReadAsStringAsync());
    Check(invalid.StatusCode == HttpStatusCode.BadRequest && ack.RootElement.GetProperty("status")[0].GetProperty("code").GetInt32() == 104,
        "Unique required user field yields 104");
}
var missingAttachment = System.Text.Json.Nodes.JsonNode.Parse(uniqueBody)!;
missingAttachment["landData"]![0]!["otherAttachmentsList"] = System.Text.Json.Nodes.JsonNode.Parse("[{\"attachmentsType\":\"TEST\"}]");
Check((await Send(uniquePath, missingAttachment.ToJsonString())).StatusCode == HttpStatusCode.BadRequest,
    "Unique attachment number required");

// Both database failures and a throwing custom logger must leave business responses intact.
foreach (var mode in new[] { "persist", "begin", "complete" })
{
    persistence.Fail = mode == "persist";
    replacement.FailBegin = mode == "begin";
    replacement.FailComplete = mode == "complete";
    var response = await Send(regPath, regBody);
    Check(response.StatusCode == HttpStatusCode.OK, mode + " logging failure preserves inbound business");
}
persistence.Fail = replacement.FailBegin = replacement.FailComplete = false;
Check(diagnostics.Messages.Any(x => x.Contains("logging")), "Logging failures emit internal diagnostics");

// Exercise transparent forwarding, body rewind, sender fallback and exceptions directly.
foreach (var mode in new[] { "large", "exception", "sender", "unknown" })
{
    var ctx = new DefaultHttpContext();
    ctx.Request.Method = "POST";
    ctx.Request.Path = regPath;
    ctx.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(regBody));
    ctx.Response.Body = new MemoryStream();
    if (mode == "sender") ctx.Request.Headers.UserAgent = "TEST-Agent/1.0";
    using var logScope = app.Services.CreateScope();
    var logging = logScope.ServiceProvider.GetRequiredService<IMsbInvocationLogger>();
    var middleware = new MsbInvocationLoggingMiddleware(async context =>
    {
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        Check(await reader.ReadToEndAsync() == regBody, "Request body position restored");
        if (mode == "exception") throw new InvalidOperationException("TEST-BUSINESS-SECRET");
        await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(mode == "large"
            ? new string('x', InvocationLogSanitizer.BodyLimit + 1) : "{\"code\":\"200\",\"message\":\"OK\"}"));
    }, configuration, NullLogger<MsbInvocationLoggingMiddleware>.Instance);
    try { await middleware.InvokeAsync(ctx, logging); Check(mode != "exception", "Business exception retained"); }
    catch (InvalidOperationException ex) when (mode == "exception")
    { Check(ex.Message == "TEST-BUSINESS-SECRET", "Business exception is unchanged"); }
    var edgeLog = (await Logs()).Last();
    if (mode == "large")
    {
        Check(ctx.Response.Body.Length == InvocationLogSanitizer.BodyLimit + 1, "Large response bytes unchanged");
        Check(edgeLog.ResponseBodyMasked!.StartsWith("[OMITTED"), "Large response safely omitted");
    }
    if (mode == "exception") Check(edgeLog.HttpStatusCode == 500 && !edgeLog.IsSuccess &&
        edgeLog.ErrorType == "InvalidOperationException" && !edgeLog.ErrorMessage!.Contains("TEST-BUSINESS-SECRET"), "Inbound exception safely recorded");
    if (mode == "sender") Check(edgeLog.SenderSystem == "User-Agent:TEST-Agent", "Sender is actual user agent product");
    if (mode == "unknown") Check(edgeLog.SenderSystem is null, "Unknown sender never guessed");
}

var outbound = new MockMsb();
var options = Options.Create(new MsbSettings { BaseUrl = "http://127.0.0.1:1", ApiKeyHeaderName = "msb-identifier", ApiKey = "TEST-API-KEY" });
using var outboundScope = app.Services.CreateScope();
var outboundLogger = outboundScope.ServiceProvider.GetRequiredService<IMsbInvocationLogger>();
var msb = new Made14CancellationService(new Factory(new HttpClient(outbound)), options, outboundLogger,
    NullLogger<Made14CancellationService>.Instance, configuration, new HttpContextAccessor());
foreach (var rule in new[] { "mgrjz6mo", "mgw2bnhz" })
{
    var response = await msb.CancelMade14Async(Cancel(rule));
    Check(response.Code == 200 && response.Msg == "TEST OK", "Cancellation contract preserved");
    var log = (await Logs()).Last();
    Check(log.Direction == "Outbound" && log.ServiceName == "Made14Cancellation" && log.RuleId == rule &&
        log.OwTrakingCode == "TEST-CANCEL" && log.HttpStatusCode == 200 && log.ResponseCode == "200" &&
        log.ResponseMessage == "TEST OK" && log.IsSuccess, "Outbound log fields");
}
var calls = outbound.Calls;
try { await msb.CancelMade14Async(Cancel("INVALID-RULE-ID")); throw new Exception("Invalid rule accepted"); }
catch (ArgumentException) { }
Check(outbound.Calls == calls, "Invalid rule never sent");
foreach (var mode in new[] { "http", "network", "json" })
{
    outbound.Mode = mode;
    try { await msb.CancelMade14Async(Cancel("mgrjz6mo")); throw new Exception("Expected original exception"); }
    catch (Exception ex) when (ex is HttpRequestException or JsonException) { }
    var log = (await Logs()).Last();
    Check(!log.IsSuccess && log.ErrorType is not null && log.CompletedAt.HasValue, "Outbound exceptions recorded");
    Check(!JsonSerializer.Serialize(log).Contains("TEST-OUTBOUND-SECRET"), "Exception/response secrets redacted");
}
outbound.Mode = "ok";
foreach (var mode in new[] { "persist", "begin", "complete" })
{
    persistence.Fail = mode == "persist";
    replacement.FailBegin = mode == "begin";
    replacement.FailComplete = mode == "complete";
    Check((await msb.CancelMade14Async(Cancel("mgrjz6mo"))).Code == 200, "Outbound tolerates " + mode + " failure");
}
persistence.Fail = replacement.FailBegin = replacement.FailComplete = false;
Check(!string.Join("\n", diagnostics.Messages).Contains("TEST-OUTBOUND-SECRET"), "Internal diagnostic secrecy");
await ExchangeAuditTests.RunAsync(app.Services, configuration);
await app.StopAsync();
Console.WriteLine($"PASS: all eight Exchange services, PDF validation, sanitized logs, four callback contracts/auth/storage, outbound fake success/errors and logging failures. Assertions={TestState.Assertions}; external MSB calls=0; SQL writes=0.");

async Task<HttpResponseMessage> Send(string path, string body, bool authorized = true)
{
    using var req = new HttpRequestMessage(HttpMethod.Post, path) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
    if (authorized) req.Headers.Add("msb-identifier", "TEST-API-KEY");
    req.Headers.Add("Authorization", "Bearer TEST-AUTHORIZATION");
    req.Headers.Add("Cookie", "TEST-COOKIE=TEST-COOKIE-VALUE");
    req.Headers.Add("X-System-Name", "TEST-SENDER");
    req.Headers.Add("X-Forwarded-For", "203.0.113.10");
    req.Headers.Add("X-Correlation-ID", "TEST-CORRELATION");
    req.Headers.Add("Referer", "https://user:TEST-REFERER-PASSWORD@example.test/source?token=TEST-REFERER-TOKEN");
    return await client.SendAsync(req);
}
async Task WithDb(Func<ApplicationDbContext, Task> action)
{
    using var scope = app.Services.CreateScope();
    await action(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
}
async Task<List<MsbInvocationLog>> Logs()
{
    List<MsbInvocationLog> logs = [];
    await WithDb(async db => logs = await db.MsbInvocationLogs.AsNoTracking().OrderBy(x => x.CreatedAt).ToListAsync());
    foreach (var log in logs)
        foreach (var secret in new[] { "TEST-AUTHORIZATION", "TEST-COOKIE-VALUE", "TEST-API-KEY", "TEST-TOKEN" })
            Check(!JsonSerializer.Serialize(log).Contains(secret), "Sensitive values absent from complete log");
    return logs;
}
static Made14CancellationRequest Cancel(string rule) => new()
{
    OrganId = "TEST-ORGAN", OwTrackingCode = "TEST-CANCEL", RuleId = rule,
    Data = new() { ActionId = "TEST-ACTION", CancelReason = "TEST CANCEL" }
};
static void Check(bool condition, string message)
{
    TestState.Assertions++;
    if (!condition) throw new Exception("Assertion failed: " + message);
}
static class TestState { public static int Assertions; public static readonly Guid Database = Guid.NewGuid(); }
sealed class Factory(HttpClient client) : IHttpClientFactory { public HttpClient CreateClient(string name) => client; }
sealed class MockMsb : HttpMessageHandler
{
    public int Calls;
    public string Mode = "ok";
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        Calls++;
        if (request.RequestUri!.Host != "127.0.0.1" || request.RequestUri.AbsolutePath != "/made14/ebtal/v1/response" ||
            request.Headers.GetValues("msb-identifier").Single() != "TEST-API-KEY") throw new Exception("Outbound contract changed");
        using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(token));
        if (body.RootElement.GetProperty("owTrackingCode").GetString() != "TEST-CANCEL" ||
            !body.RootElement.GetProperty("noToken").GetBoolean()) throw new Exception("Cancellation serialization changed");
        if (Mode == "network") throw new HttpRequestException("TEST-OUTBOUND-SECRET");
        return new HttpResponseMessage(Mode == "http" ? HttpStatusCode.BadGateway : HttpStatusCode.OK)
        {
            Content = new StringContent(Mode == "json" ? "malformed TEST-OUTBOUND-SECRET" :
                Mode == "http" ? "{\"token\":\"TEST-OUTBOUND-SECRET\",\"msg\":\"TEST-OUTBOUND-SECRET\"}" : "{\"code\":200,\"msg\":\"TEST OK\"}")
        };
    }
}
sealed class LoggingFailureInterceptor : SaveChangesInterceptor
{
    public bool Fail;
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken token = default)
    {
        if (Fail && eventData.Context!.ChangeTracker.Entries<MsbInvocationLog>().Any())
            throw new InvalidOperationException("TEST-OUTBOUND-SECRET");
        return ValueTask.FromResult(result);
    }
}
sealed class SwitchableLogger : IMsbInvocationLogger
{
    public IMsbInvocationLogger Inner = null!;
    public bool FailBegin, FailComplete;
    public Task<Guid> BeginAsync(MsbInvocationLog log) => FailBegin ? throw new Exception("TEST-OUTBOUND-SECRET") : Inner.BeginAsync(log);
    public Task CompleteAsync(Guid id, MsbInvocationCompletion completion) => FailComplete ? throw new Exception("TEST-OUTBOUND-SECRET") : Inner.CompleteAsync(id, completion);
}
sealed class Diagnostics : ILoggerProvider
{
    public System.Collections.Concurrent.ConcurrentQueue<string> Messages { get; } = new();
    public ILogger CreateLogger(string category) => new DiagnosticLogger(Messages);
    public void Dispose() { }
    private sealed class DiagnosticLogger(System.Collections.Concurrent.ConcurrentQueue<string> messages) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel level) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => messages.Enqueue(formatter(state, exception));
    }
}
