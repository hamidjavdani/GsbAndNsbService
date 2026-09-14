namespace GSB.Test.Api.Data.Entities;
public class SanadCallback
{
    public int Id { get; set; }

    public int Code { get; set; }

    public string? RawJson { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}