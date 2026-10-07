namespace GSB.Test.Api.Data.Entities;

public class UniqueIdentifierCallback
{
    public long Id { get; set; }

    public string OrganId { get; set; } = string.Empty;

    public string MapConfirmationTrackingCode { get; set; } = string.Empty;

    public string? RequestId { get; set; }

    public string? TaskId { get; set; }

    public string? ActivityId { get; set; }

    public string? RuleId { get; set; }

    public string? ActionId { get; set; }

    public string? ExternalCode { get; set; }

    public string RawJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
