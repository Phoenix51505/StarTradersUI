using System.Text.Json;
using System.Text.Json.Serialization;

namespace StarTradersUI.Api;

public static class JsonUtils
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        Converters =
        {
            new JsonEnumMemberStringEnumConverter()
        },
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        
    };

    public static async Task<T?> FromJson<T>(Stream jsonStream, CancellationToken ct=default) =>
        await JsonSerializer.DeserializeAsync<T>(jsonStream, Options, ct);
    
    public static async Task ToJson<T>(T value, Stream jsonStream, CancellationToken ct=default) => await JsonSerializer.SerializeAsync(jsonStream, value, Options, ct);
}