using System.Runtime.Serialization;

namespace StarTradersUI.Api.SystemInfo;

/// <summary>
/// The type of system.
/// </summary>
public enum SystemType
{
    [EnumMember(Value = "NEUTRON_STAR")]
    NeutronStar,
    [EnumMember(Value = "RED_STAR")]
    RedStar,
    [EnumMember(Value = "ORANGE_STAR")]
    OrangeStar,
    [EnumMember(Value = "BLUE_STAR")]
    BlueStar,
    [EnumMember(Value = "YOUNG_STAR")]
    YoungStar,
    [EnumMember(Value = "WHITE_DWARF")]
    WhiteDwarf,
    [EnumMember(Value = "BLACK_HOLE")]
    BlackHole,
    [EnumMember(Value = "HYPERGIANT")]
    Hypergiant,
    [EnumMember(Value = "NEBULA")]
    Nebula,
    [EnumMember(Value = "UNSTABLE")]
    Unstable,
}