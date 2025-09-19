using System.Text.Json.Serialization;

namespace StarTradersUi.Api;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SystemType
{
    [JsonStringEnumMemberName("NEUTRON_STAR")]
    NeutronStar,
    [JsonStringEnumMemberName("RED_STAR")]
    RedStar,
    [JsonStringEnumMemberName("ORANGE_STAR")]
    OrangeStar,
    [JsonStringEnumMemberName("BLUE_STAR")]
    BlueStar,
    [JsonStringEnumMemberName("YOUNG_STAR")]
    YoungStar,
    [JsonStringEnumMemberName("WHITE_DWARF")]
    WhiteDwarf,
    [JsonStringEnumMemberName("BLACK_HOLE")]
    BlackHole,
    [JsonStringEnumMemberName("HYPERGIANT")]
    Hypergiant,
    [JsonStringEnumMemberName("NEBULA")]
    Nebula,
    [JsonStringEnumMemberName("UNSTABLE")]
    Unstable,
}