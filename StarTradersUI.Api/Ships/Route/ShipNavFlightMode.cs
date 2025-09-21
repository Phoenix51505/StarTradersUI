namespace StarTradersUI.Api.Ships.Route;
using System.Runtime.Serialization;

/// <summary>
/// The ship's set speed when traveling between waypoints or systems.
/// </summary>
public enum ShipNavFlightMode {
    [EnumMember(Value = "DRIFT")]
    Drift,
    [EnumMember(Value = "STEALTH")]
    Stealth,
    [EnumMember(Value = "CRUISE")]
    Cruise,
    [EnumMember(Value = "BURN")]
    Burn,
}