using System.Text.Json.Nodes;

namespace GSB.Test.Api.Services;

/// <summary>Per-invocation redaction. Unparseable or oversized bodies are never logged verbatim.</summary>
public sealed class InvocationLogSanitizer
{
    public const int BodyLimit = 64 * 1024;
    public const string Mask = "***MASKED***";
    private readonly HashSet<string> _secrets = new(StringComparer.Ordinal);
    private readonly HashSet<string> _privateValues = new(StringComparer.Ordinal);

    public InvocationLogSanitizer(IConfiguration configuration, params string[] secrets)
    {
        foreach (var pair in configuration.AsEnumerable())
            if (IsSensitive(pair.Key.Split(':').Last()) && !string.IsNullOrEmpty(pair.Value))
                _secrets.Add(pair.Value);
        foreach (var secret in secrets)
            if (!string.IsNullOrEmpty(secret)) _secrets.Add(secret);
    }

    private static bool IsSensitive(string key)
    {
        var normalized = new string(key.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        return normalized is "token" or "apikey" or "password" or "secret" or "secretno" or "authorization"
            or "accesstoken" or "refreshtoken" or "cookie" or "setcookie"
            or "file" or "docimage" or "map"
            || IsPrivate(normalized)
            || normalized.Contains("base64", StringComparison.Ordinal)
            || normalized.Contains("64base", StringComparison.Ordinal);
    }

    private static bool IsPrivate(string key)
    {
        var normalized = new string(key.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        return normalized is "estatemap" or "unitmap" or "coordinates" or "ownersinfo" or "userinfo" or "lstfindpersoninquery"
            or "nationalno" or "nationalnomovakel" or "nationalcode" or "nationalid" or "nationalitycode"
            or "ownernationalitycode" or "owneridentityno" or "contactno" or "ownersaddress" or "address"
            or "name" or "family" or "ownername" or "ownerfamily" or "namemovakel" or "familymovakel"
            or "birthdate" or "fathername" or "requestername" or "requesterfamily" or "requesternationalcode"
            or "requesterofficecode" or "requesterofficnumber" or "electronicestatenoteno" or "nationalregisterno" or "filename";
    }

    public string? Text(string? value, int limit = 1024)
    {
        if (string.IsNullOrEmpty(value)) return null;
        foreach (var secret in _secrets.OrderByDescending(x => x.Length))
            value = value.Replace(secret, Mask, StringComparison.Ordinal);
        // Personal names can be short/common. Mask complete values, not fragments of operational IDs.
        foreach (var personal in _privateValues.OrderByDescending(x => x.Length))
            value = System.Text.RegularExpressions.Regex.Replace(value,
                @"(?<![\p{L}\p{N}_-])" + System.Text.RegularExpressions.Regex.Escape(personal) + @"(?![\p{L}\p{N}_-])", Mask);
        // Do not persist arbitrary header control characters.
        value = new string(value.Where(c => !char.IsControl(c)).ToArray());
        return value.Length > limit ? value[..limit] : value;
    }

    public string? Body(string? body)
    {
        if (string.IsNullOrEmpty(body)) return null;
        if (body.Length > BodyLimit) return "[OMITTED: body exceeds logging limit]";
        try
        {
            var node = JsonNode.Parse(body);
            Collect(node);
            Redact(node);
            return node?.ToJsonString();
        }
        catch (System.Text.Json.JsonException)
        {
            return "[OMITTED: non-JSON or malformed body]";
        }
    }

    public string? Url(string? value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            uri.Scheme is not ("http" or "https")) return null;
        // Query strings, fragments and URI credentials can contain arbitrary unknown secrets.
        var safe = new UriBuilder(uri) { UserName = "", Password = "", Query = "", Fragment = "" };
        return Text(safe.Uri.GetLeftPart(UriPartial.Path));
    }

    private void Collect(JsonNode? node)
    {
        if (node is JsonObject obj)
            foreach (var pair in obj)
            {
                if (IsSensitive(pair.Key)) CollectValues(pair.Value, IsPrivate(pair.Key));
                Collect(pair.Value);
            }
        else if (node is JsonArray array)
            foreach (var item in array) Collect(item);
    }

    private void CollectValues(JsonNode? node, bool personal)
    {
        if (node is JsonValue value && value.TryGetValue<string>(out var text) && !string.IsNullOrEmpty(text))
            (personal ? _privateValues : _secrets).Add(text);
        else if (node is JsonObject obj)
            foreach (var item in obj) CollectValues(item.Value, personal);
        else if (node is JsonArray array)
            foreach (var item in array) CollectValues(item, personal);
    }

    private void Redact(JsonNode? node)
    {
        if (node is JsonObject obj)
            foreach (var pair in obj.ToArray())
            {
                if (IsSensitive(pair.Key)) obj[pair.Key] = Mask;
                else if (pair.Value is JsonValue val && val.TryGetValue<string>(out var text))
                    obj[pair.Key] = Text(text, BodyLimit);
                else Redact(pair.Value);
            }
        else if (node is JsonArray array)
            for (var i = 0; i < array.Count; i++)
            {
                if (array[i] is JsonValue val && val.TryGetValue<string>(out var text))
                    array[i] = Text(text, BodyLimit);
                else Redact(array[i]);
            }
    }
}
