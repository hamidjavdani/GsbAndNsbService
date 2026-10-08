using GSB.Test.Api.Data.Entities;
using System.Text.Json.Nodes;

namespace GSB.Test.Api.Services;

public static class InvocationLogData
{
    public static void Request(MsbInvocationLog log, string? maskedBody)
    {
        log.RequestBodyMasked = maskedBody;
        var obj = Parse(maskedBody);
        log.OrganId = Value(obj, "organId", 256);
        log.OwTrakingCode = Value(obj, "owTrakingCode", 256) ?? Value(obj, "owTrackingCode", 256);
        log.MapConfirmationTrackingCode = Value(obj, "mapConfirmationTrackingCode", 256);
        var data = Property(obj, "data") as JsonObject;
        log.RequestId = Value(obj, "requestUniqueId", 256) ?? Value(data, "requestId", 256);
        log.TaskId = Value(data, "taskId");
        log.ActivityId = Value(data, "activityId");
        log.ActionId = Value(data, "actionId");
        log.RuleId = Value(obj, "ruleId") ?? Value(data, "ruleId");
    }

    public static MsbInvocationCompletion Completion(
        long durationMs, int? status, string? maskedBody, Exception? error)
    {
        var obj = Parse(maskedBody);
        var firstStatus = (Property(obj, "status") as JsonArray)?.FirstOrDefault() as JsonObject;
        return new(DateTime.UtcNow, durationMs, status,
            error is null && status is >= 200 and < 300, maskedBody,
            Value(obj, "code") ?? Value(firstStatus, "code"),
            Value(obj, "message") ?? Value(obj, "msg") ?? Value(firstStatus, "message"),
            error?.GetType().Name,
            error is null ? null : "Processing failed; exception details omitted to protect secrets.");
    }

    private static JsonObject? Parse(string? body)
    {
        try { return body is null ? null : JsonNode.Parse(body) as JsonObject; }
        catch (System.Text.Json.JsonException) { return null; }
    }

    private static JsonNode? Property(JsonObject? obj, string name) =>
        obj?.FirstOrDefault(x => string.Equals(x.Key, name, StringComparison.OrdinalIgnoreCase)).Value;

    private static string? Value(JsonObject? obj, string name, int limit = 1024)
    {
        var value = Property(obj, name) as JsonValue;
        var text = value?.ToString();
        return text?.Length > limit ? text[..limit] : text;
    }
}
