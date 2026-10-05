using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;

namespace GSB.Test.Api.Services;

public class RawMsbCallbackService : IRawMsbCallbackService
{
    private readonly ApplicationDbContext _context;

    public RawMsbCallbackService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveRawAsync(string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return false;
        }

        _context.SanadCallbacks.Add(new SanadCallback
        {
            Code = 200,
            RawJson = rawJson,
            CreatedAt = DateTime.Now
        });

        _ = await _context.SaveChangesAsync();
        return true;
    }
}
