using System.Diagnostics;
using System.Text;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Services;

namespace GSB.Test.Api.Middleware;

public sealed class MsbInvocationLoggingMiddleware(
    RequestDelegate next, IConfiguration configuration, ILogger<MsbInvocationLoggingMiddleware> diagnostics)
{
    private static readonly Dictionary<string, string> Services = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/made-14/registration-response/v1/create"] = "RegistrationResponse",
        ["/document-ownership-verification/v1/create"] = "DocumentOwnershipVerification",
        ["/made14/send-unique-identifier-response/v1/create"] = "UniqueIdentifierResponse",
        ["/interagency-inquiry/poa-evaluation-response/v1/create"] = "PoaEvaluationResponse"
    };

    public async Task InvokeAsync(HttpContext context, IMsbInvocationLogger logger)
    {
        if (!HttpMethods.IsPost(context.Request.Method) ||
            !Services.TryGetValue(context.Request.Path.Value?.TrimEnd('/') ?? "", out var service))
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var sanitizer = new InvocationLogSanitizer(configuration);
        var id = Guid.Empty;
        try
        {
            var maskedBody = sanitizer.Body(await ReadBodyAsync(context.Request));
            var log = new MsbInvocationLog
            {
                Direction = "Inbound", ServiceName = service,
                Endpoint = context.Request.Path.Value!, HttpMethod = context.Request.Method,
                ReceivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow,
                RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                RemotePort = context.Connection.RemotePort,
                Host = sanitizer.Text(context.Request.Host.Value),
                UserAgent = sanitizer.Text(context.Request.Headers.UserAgent.ToString()),
                ContentType = sanitizer.Text(context.Request.ContentType),
                ContentLength = context.Request.ContentLength,
                ForwardedFor = sanitizer.Text(context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()),
                Referer = sanitizer.Url(context.Request.Headers.Referer.FirstOrDefault()),
                TraceIdentifier = sanitizer.Text(context.TraceIdentifier, 256),
                CorrelationId = sanitizer.Text(context.Request.Headers["X-Correlation-ID"].FirstOrDefault()),
                SenderSystem = Sender(context.Request, sanitizer)
            };
            InvocationLogData.Request(log, maskedBody);
            id = await SafeInvocationLogging.BeginAsync(logger, log, diagnostics);
        }
        catch (Exception ex)
        {
            diagnostics.LogWarning("Invocation request capture failed ({ErrorType}).", ex.GetType().Name);
        }

        var original = context.Response.Body;
        using var capture = new InvocationResponseCaptureStream(original);
        context.Response.Body = capture;
        Exception? failure = null;
        try { await next(context); }
        catch (Exception ex) { failure = ex; throw; }
        finally
        {
            context.Response.Body = original;
            try
            {
                var completion = InvocationLogData.Completion(stopwatch.ElapsedMilliseconds,
                    failure is null ? context.Response.StatusCode : context.Response.HasStarted ? context.Response.StatusCode : 500,
                    sanitizer.Body(capture.CapturedBody), failure);
                await SafeInvocationLogging.CompleteAsync(logger, id, completion, diagnostics);
            }
            catch (Exception ex)
            {
                diagnostics.LogWarning("Invocation response capture failed ({ErrorType}).", ex.GetType().Name);
            }
        }
    }

    private static async Task<string?> ReadBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        var position = request.Body.Position;
        try
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, false, 4096, leaveOpen: true);
            var buffer = new char[InvocationLogSanitizer.BodyLimit + 1];
            var total = 0;
            while (total < buffer.Length)
            {
                var read = await reader.ReadAsync(buffer.AsMemory(total));
                if (read == 0) break;
                total += read;
            }
            return total == 0 ? null : new string(buffer, 0, total);
        }
        finally { request.Body.Position = position; }
    }

    private static string? Sender(HttpRequest request, InvocationLogSanitizer sanitizer)
    {
        foreach (var name in new[] { "X-System-Name", "X-Client-Name", "X-Source-System" })
        {
            var value = request.Headers[name].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value) && value.Length <= 128 &&
                value.All(c => char.IsLetterOrDigit(c) || c is ' ' or '-' or '_' or '.'))
                return sanitizer.Text(value);
        }
        // An explicit product token is evidence; no business system is inferred from host/IP.
        var product = request.Headers.UserAgent.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (product is not null && System.Net.Http.Headers.ProductInfoHeaderValue.TryParse(product, out var parsed)
            && parsed.Product?.Version is not null)
            return sanitizer.Text("User-Agent:" + parsed.Product.Name);
        return null;
    }
}
