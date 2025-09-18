using System.Text.Json.Serialization;

namespace StarTradersUi.Api;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FactionSymbol
{
    [JsonStringEnumMemberName("COSMIC")]
    Cosmic,
    [JsonStringEnumMemberName("VOID")]
    Void,
    [JsonStringEnumMemberName("GALACTIC")]
    Galactic,
    [JsonStringEnumMemberName("QUANTUM")]
    Quantum,
    [JsonStringEnumMemberName("DOMINION")]
    Dominion,
    [JsonStringEnumMemberName("ASTRO")]
    Astro,
    [JsonStringEnumMemberName("CORSAIRS")]
    Corsairs,
    [JsonStringEnumMemberName("OBSIDIAN")]
    Obsidian,
    [JsonStringEnumMemberName("AEGIS")]
    Aegis,
    [JsonStringEnumMemberName("UNITED")]
    United,
    [JsonStringEnumMemberName("SOLITARY")]
    Solitary,
    [JsonStringEnumMemberName("COBALT")]
    Cobalt,
    [JsonStringEnumMemberName("OMEGA")]
    Omega,
    [JsonStringEnumMemberName("ECHO")]
    Echo,
    [JsonStringEnumMemberName("LORDS")]
    Lords,
    [JsonStringEnumMemberName("CULT")]
    Cult,
    [JsonStringEnumMemberName("ANCIENTS")]
    Ancients,
    [JsonStringEnumMemberName("SHADOW")]
    Shadow,
    [JsonStringEnumMemberName("ETHEREAL")]
    Ethereal,
}