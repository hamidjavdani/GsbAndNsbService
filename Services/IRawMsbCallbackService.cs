namespace GSB.Test.Api.Services;

public interface IRawMsbCallbackService
{
    Task<bool> SaveRawAsync(string rawJson);
}
