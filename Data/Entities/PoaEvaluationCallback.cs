namespace GSB.Test.Api.Data.Entities;

public class PoaEvaluationCallback
{
    public long Id { get; set; }

    public string OrganId { get; set; } = string.Empty;

    public string OwTrakingCode { get; set; } = string.Empty;

    public int Code { get; set; }

    public bool? Succseed { get; set; }

    public string? NationalRegisterNo { get; set; }

    public string? DocType { get; set; }

    public bool? HasPermission { get; set; }

    public bool? ExistDoc { get; set; }

    public string? AdvocacyEndDate { get; set; }

    public int? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }

    public string RawJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
