using System.Text.Json.Serialization;

namespace StarTradersUi.Api;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WaypointType
{
    [JsonStringEnumMemberName("PLANET")]
    Planet,
    [JsonStringEnumMemberName("GAS_GIANT")]
    GasGiant,
    [JsonStringEnumMemberName("MOON")]
    Moon,
    [JsonStringEnumMemberName("ORBITAL_STATION")]
    OrbitalStation,
    [JsonStringEnumMemberName("JUMP_GATE")]
    JumpGate,
    [JsonStringEnumMemberName("ASTEROID_FIELD")]
    AsteroidField,
    [JsonStringEnumMemberName("ASTEROID")]
    Asteroid,
    [JsonStringEnumMemberName("ENGINEERED_ASTEROID")]
    EngineeredAsteroid,
    [JsonStringEnumMemberName("ASTEROID_BASE")]
    AsteroidBase,
    [JsonStringEnumMemberName("NEBULA")]
    Nebula,
    [JsonStringEnumMemberName("DEBRIS_FIELD")]
    DebrisField,
    [JsonStringEnumMemberName("GRAVITY_WELL")]
    GravityWell,
    [JsonStringEnumMemberName("ARTIFICIAL_GRAVITY_WELL")]
    ArtificialGravityWell,
    [JsonStringEnumMemberName("FUEL_STATION")]
    FuelStation,
}