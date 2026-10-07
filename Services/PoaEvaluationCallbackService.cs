using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Models.Callback;

namespace GSB.Test.Api.Services;

public class PoaEvaluationCallbackService : IPoaEvaluationCallbackService
{
    private readonly ApplicationDbContext _context;

    public PoaEvaluationCallbackService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveAsync(
        PoaEvaluationResponseRequest request,
        string rawJson)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(rawJson) ||
            string.IsNullOrWhiteSpace(request.OrganId) ||
            string.IsNullOrWhiteSpace(request.OwTrakingCode) ||
            request.Code is null)
        {
            return false;
        }

        var entity = new PoaEvaluationCallback
        {
            OrganId = request.OrganId,
            OwTrakingCode = request.OwTrakingCode,
            Code = request.Code.Value,
            Succseed = request.Data?.Succseed,
            NationalRegisterNo = request.Data?.NationalRegisterNo,
            DocType = request.Data?.DocType,
            HasPermission = request.Data?.HasPermission,
            ExistDoc = request.Data?.ExistDoc,
            AdvocacyEndDate = request.Data?.AdvocacyEndDate,
            ErrorCode = request.Error?.ErrorCode,
            ErrorMessage = request.Error?.ErrorMessage,
            RawJson = rawJson,
            CreatedAt = DateTime.Now
        };

        _context.PoaEvaluationCallbacks.Add(entity);
        _ = await _context.SaveChangesAsync();
        return true;
    }
}
