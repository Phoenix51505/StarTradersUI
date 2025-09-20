using System.Runtime.Serialization;

namespace StarTradersUI.Api.WaypointInfo;

/// <summary>
/// The type of waypoint.
/// </summary>
public enum WaypointType
{
    [EnumMember(Value = "PLANET")]
    Planet,
    [EnumMember(Value = "GAS_GIANT")]
    GasGiant,
    [EnumMember(Value = "MOON")]
    Moon,
    [EnumMember(Value = "ORBITAL_STATION")]
    OrbitalStation,
    [EnumMember(Value = "JUMP_GATE")]
    JumpGate,
    [EnumMember(Value = "ASTEROID_FIELD")]
    AsteroidField,
    [EnumMember(Value = "ASTEROID")]
    Asteroid,
    [EnumMember(Value = "ENGINEERED_ASTEROID")]
    EngineeredAsteroid,
    [EnumMember(Value = "ASTEROID_BASE")]
    AsteroidBase,
    [EnumMember(Value = "NEBULA")]
    Nebula,
    [EnumMember(Value = "DEBRIS_FIELD")]
    DebrisField,
    [EnumMember(Value = "GRAVITY_WELL")]
    GravityWell,
    [EnumMember(Value = "ARTIFICIAL_GRAVITY_WELL")]
    ArtificialGravityWell,
    [EnumMember(Value = "FUEL_STATION")]
    FuelStation,
}