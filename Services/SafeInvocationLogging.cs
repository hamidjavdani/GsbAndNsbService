using GSB.Test.Api.Data.Entities;

namespace GSB.Test.Api.Services;

// Also protects business processing if a replacement logger throws unexpectedly.
public static class SafeInvocationLogging
{
    public static async Task<Guid> BeginAsync(IMsbInvocationLogger logger, MsbInvocationLog log, ILogger diagnostics)
    {
        try { return await logger.BeginAsync(log); }
        catch (Exception ex)
        {
            diagnostics.LogWarning("Invocation logging begin failed ({ErrorType}).", ex.GetType().Name);
            return Guid.Empty;
        }
    }

    public static async Task CompleteAsync(IMsbInvocationLogger logger, Guid id,
        MsbInvocationCompletion completion, ILogger diagnostics)
    {
        if (id == Guid.Empty) return;
        try { await logger.CompleteAsync(id, completion); }
        catch (Exception ex)
        {
            diagnostics.LogWarning("Invocation logging completion failed ({ErrorType}).", ex.GetType().Name);
        }
    }
}
