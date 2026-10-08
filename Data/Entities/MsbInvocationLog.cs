namespace GSB.Test.Api.Data.Entities;

public class MsbInvocationLog
{
    public Guid Id { get; set; }
    public string Direction { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    public int? HttpStatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string? OrganId { get; set; }
    public string? OwTrakingCode { get; set; }
    public string? MapConfirmationTrackingCode { get; set; }
    public string? RequestId { get; set; }
    public string? TaskId { get; set; }
    public string? ActivityId { get; set; }
    public string? ActionId { get; set; }
    public string? RuleId { get; set; }
    public string? RemoteIpAddress { get; set; }
    public int? RemotePort { get; set; }
    public string? Host { get; set; }
    public string? UserAgent { get; set; }
    public string? ContentType { get; set; }
    public long? ContentLength { get; set; }
    public string? ForwardedFor { get; set; }
    public string? Referer { get; set; }
    public string? TraceIdentifier { get; set; }
    public string? CorrelationId { get; set; }
    public string? SenderSystem { get; set; }
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RequestBodyMasked { get; set; }
    public string? ResponseBodyMasked { get; set; }
    public DateTime CreatedAt { get; set; }
}
