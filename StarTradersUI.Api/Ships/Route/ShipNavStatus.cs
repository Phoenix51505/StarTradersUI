namespace StarTradersUI.Api.Ships.Route;

using System.Runtime.Serialization;

/// <summary>
/// The current status of the ship
/// </summary>
public enum ShipNavStatus {
    [EnumMember(Value = "IN_TRANSIT")]
    InTransit,
    [EnumMember(Value = "IN_ORBIT")]
    InOrbit,
    [EnumMember(Value = "DOCKED")]
    Docked,
}