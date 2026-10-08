using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public class UniqueIdentifierCallbackService : IUniqueIdentifierCallbackService
{
    private readonly ApplicationDbContext _context;

    public UniqueIdentifierCallbackService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveAsync(
        UniqueIdentifierResponseRequest request,
        string rawJson)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(rawJson) ||
            string.IsNullOrWhiteSpace(request.OrganId) ||
            string.IsNullOrWhiteSpace(request.MapConfirmationTrackingCode))
        {
            return false;
        }

        var entity = new UniqueIdentifierCallback
        {
            OrganId = request.OrganId,
            MapConfirmationTrackingCode = request.MapConfirmationTrackingCode,
            RequestId = request.Data?.RequestId,
            TaskId = request.Data?.TaskId,
            ActivityId = request.Data?.ActivityId,
            RuleId = request.Data?.RuleId,
            ActionId = request.Data?.ActionId,
            ExternalCode = request.Data?.Code,
            RawJson = rawJson,
            CreatedAt = DateTime.Now
        };

        _context.UniqueIdentifierCallbacks.Add(entity);
        _ = await _context.SaveChangesAsync();

        return true;
    }
}
