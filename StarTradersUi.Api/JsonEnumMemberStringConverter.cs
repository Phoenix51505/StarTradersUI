using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StarTradersUI.Api;

public class JsonEnumMemberStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
    : JsonConverterFactory
{
    private readonly JsonStringEnumConverter _baseConverter = new(namingPolicy, allowIntegerValues);

    public JsonEnumMemberStringEnumConverter() : this(null, true)
    {
    }

    public override bool CanConvert(Type typeToConvert) => _baseConverter.CanConvert(typeToConvert);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var query = from field in typeToConvert.GetFields(BindingFlags.Public | BindingFlags.Static)
            let attr = field.GetCustomAttribute<EnumMemberAttribute>()
            where attr is { Value: not null }
            select (field.Name, attr.Value);
        var dictionary = query.ToDictionary(p => p.Name, p => p.Value);
        return dictionary.Count > 0
            ? new JsonStringEnumConverter(new DictionaryLookupNamingPolicy(dictionary, namingPolicy),
                allowIntegerValues).CreateConverter(typeToConvert, options)
            : _baseConverter.CreateConverter(typeToConvert, options);
    }
}

public class JsonNamingPolicyDecorator(JsonNamingPolicy? underlyingNamingPolicy) : JsonNamingPolicy
{
    public override string ConvertName(string name) => underlyingNamingPolicy?.ConvertName(name) ?? name;
}

internal class DictionaryLookupNamingPolicy(
    Dictionary<string, string> dictionary,
    JsonNamingPolicy? underlyingNamingPolicy)
    : JsonNamingPolicyDecorator(underlyingNamingPolicy)
{
    private readonly Dictionary<string, string> _dictionary = dictionary ?? throw new ArgumentNullException();

    public override string ConvertName(string name) =>
        _dictionary.TryGetValue(name, out var value) ? value : base.ConvertName(name);
}