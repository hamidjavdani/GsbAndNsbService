using GSB.Test.Api.Data;
using GSB.Test.Api.Data.Entities;
using GSB.Test.Api.Models.Callback;
using System.Text.Json;

namespace GSB.Test.Api.Services;

/// <summary>
/// سرویس دریافت Callback از پنجره واحد
/// </summary>
public class RegistrationCallbackService : IRegistrationCallbackService
{
    private readonly ApplicationDbContext _context;

    public RegistrationCallbackService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// ذخیره Callback دریافتی از پنجره واحد
    /// </summary>
    public async Task<bool> SaveCallbackAsync(RegistrationCallbackRequest request)
    {
        if (request == null)
        {
            return false;
        }

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