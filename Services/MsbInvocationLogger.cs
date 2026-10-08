using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GSB.Test.Api.Services;

public sealed class MsbInvocationLogger(
    IServiceScopeFactory scopeFactory, ILogger<MsbInvocationLogger> logger) : IMsbInvocationLogger
{
    public async Task<Guid> BeginAsync(MsbInvocationLog log)
    {
        try
        {
            // A separate context must never flush business entities or poison their tracker.
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (db.Database.IsRelational()) db.Database.SetCommandTimeout(5);
            log.Id = Guid.NewGuid();
            db.MsbInvocationLogs.Add(log);
            await db.SaveChangesAsync();
            return log.Id;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Invocation logging begin failed ({ErrorType}).", ex.GetType().Name);
            return Guid.Empty;
        }
    }

    public async Task CompleteAsync(Guid id, MsbInvocationCompletion completion)
    {
        if (id == Guid.Empty) return;
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (db.Database.IsRelational()) db.Database.SetCommandTimeout(5);
            var log = await db.MsbInvocationLogs.FindAsync(id);
            if (log is null) return;
            log.CompletedAt = completion.CompletedAt;
            log.DurationMs = completion.DurationMs;
            log.HttpStatusCode = completion.HttpStatusCode;
            log.IsSuccess = completion.IsSuccess;
            log.ResponseBodyMasked = completion.ResponseBodyMasked;
            log.ResponseCode = completion.ResponseCode;
            log.ResponseMessage = completion.ResponseMessage;
            log.ErrorType = completion.ErrorType;
            log.ErrorMessage = completion.ErrorMessage;
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Do not pass exception objects: SQL/HTTP exceptions can include secrets.
            logger.LogWarning("Invocation logging completion failed ({ErrorType}).", ex.GetType().Name);
        }
    }
}
