using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Models.Callback;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public class MsbCallbackService : IMsbCallbackService
{
    private readonly ApplicationDbContext _context;

    public MsbCallbackService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveCallbackAsync(MsbCallbackRequest request)
    {
        if (request is null)
            return false;

        var entity = new SanadCallback
        {
            Code = request.Code,
            RawJson = JsonSerializer.Serialize(request),
            CreatedAt = DateTime.Now
        };

        _context.SanadCallbacks.Add(entity);
        _ = await _context.SaveChangesAsync();

        return true;
    }
}
