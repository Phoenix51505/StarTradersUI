using System.Runtime.Serialization;

namespace StarTradersUI.Api.WaypointInfo;

public enum WaypointModifierSymbol
{
    [EnumMember(Value = "STRIPPED")]
    Stripped,
    [EnumMember(Value = "UNSTABLE")]
    Unstable,
    [EnumMember(Value = "RADIATION_LEAK")]
    RadiationLeak,
    [EnumMember(Value = "CRITICAL_LIMIT")]
    CriticalLimit,
    [EnumMember(Value = "CIVIL_UNREST")]
    CivilUnrest,
}