using GSB.Test.Api.Models.Requests;
using System.Text.Json;

namespace GSB.Test.Api.Services;

public interface IMapApprovalService
{
    Task<JsonElement> SendAsync(MapApprovalRequest request);
}
