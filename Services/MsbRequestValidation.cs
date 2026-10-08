namespace GSB.Test.Api.Services;

internal static class MsbRequestValidation
{
    public static void Required(string? value, string field, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{field} is required.", "request");
        if (maxLength.HasValue && value.Length > maxLength.Value)
            throw new ArgumentException($"{field} exceeds the documented maximum length.", "request");
    }

    public static void RequiredNumber<T>(T? value, string field) where T : struct
    {
        if (!value.HasValue) throw new ArgumentException($"{field} is required.", "request");
    }

    public static string Rule(string? supplied, string official)
    {
        if (string.IsNullOrWhiteSpace(supplied)) return official;
        if (supplied != official)
            throw new ArgumentException("ruleId is not valid for this service.", "request");
        return supplied;
    }
}
