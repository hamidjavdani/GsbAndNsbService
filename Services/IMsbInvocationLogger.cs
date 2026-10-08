using GSB.Test.Api.Data.Entities;

namespace GSB.Test.Api.Services;

public interface IMsbInvocationLogger
{
    Task<Guid> BeginAsync(MsbInvocationLog log);
    Task CompleteAsync(Guid id, MsbInvocationCompletion completion);
}

public sealed record MsbInvocationCompletion(
    DateTime CompletedAt, long DurationMs, int? HttpStatusCode, bool IsSuccess,
    string? ResponseBodyMasked, string? ResponseCode, string? ResponseMessage,
    string? ErrorType, string? ErrorMessage);
