using System.Diagnostics;
using System.Text;
using GSB.Test.Api.Configurations;
using GSB.Test.Api.Data.Entities;

namespace GSB.Test.Api.Services;

// Shared outbound capture; only sanitized data is ever handed to a persistence logger.
public sealed class MsbOutboundInvocation(
    HttpClient client, MsbSettings settings, IMsbInvocationLogger logger,
    ILogger diagnostics, IConfiguration configuration, IHttpContextAccessor contextAccessor)
{
    public async Task<T> SendAsync<T>(string serviceName, string endpoint, string json, Func<string, T> deserialize)
    {
        var url = $"{settings.BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        message.Headers.Add(settings.ApiKeyHeaderName, settings.ApiKey);

        var stopwatch = Stopwatch.StartNew();
        var id = Guid.Empty;
        InvocationLogSanitizer? sanitizer = null;
        try
        {
            sanitizer = new InvocationLogSanitizer(configuration, settings.ApiKey);
            var maskedRequest = sanitizer.Body(json);
            var context = contextAccessor.HttpContext;
            var target = new UriBuilder(url) { UserName = "", Password = "", Query = "", Fragment = "" };
            var log = new MsbInvocationLog
            {
                Direction = "Outbound", ServiceName = serviceName,
                Endpoint = sanitizer.Text(target.Uri.GetLeftPart(UriPartial.Path))!,
                HttpMethod = "POST", ReceivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow,
                Host = sanitizer.Text(target.Host), ContentType = "application/json",
                ContentLength = Encoding.UTF8.GetByteCount(json),
                TraceIdentifier = sanitizer.Text(context?.TraceIdentifier, 256),
                CorrelationId = sanitizer.Text(context?.Request.Headers["X-Correlation-ID"].FirstOrDefault())
            };
            InvocationLogData.Request(log, maskedRequest);
            id = await SafeInvocationLogging.BeginAsync(logger, log, diagnostics);
        }
        catch (Exception ex)
        {
            diagnostics.LogWarning("Outbound invocation capture failed ({ErrorType}).", ex.GetType().Name);
        }

        int? httpStatus = null;
        string? body = null;
        Exception? failure = null;
        try
        {
            using var response = await client.SendAsync(message);
            httpStatus = (int)response.StatusCode;
            body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"MSB {serviceName} HTTP {httpStatus}; response details omitted to protect secrets.");
            return deserialize(body);
        }
        catch (Exception ex) { failure = ex; throw; }
        finally
        {
            try
            {
                await SafeInvocationLogging.CompleteAsync(logger, id,
                    InvocationLogData.Completion(stopwatch.ElapsedMilliseconds, httpStatus, sanitizer?.Body(body), failure), diagnostics);
            }
            catch (Exception ex)
            {
                diagnostics.LogWarning("Outbound invocation completion failed ({ErrorType}).", ex.GetType().Name);
            }
        }
    }
}
