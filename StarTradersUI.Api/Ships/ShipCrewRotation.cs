namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

/// <summary>
/// The rotation of crew shifts. A stricter shift improves the ship's performance. A more relaxed shift improves the crew's morale.
/// </summary>
public enum ShipCrewRotation {
    [EnumMember(Value = "STRICT")]
    Strict,
    [EnumMember(Value = "RELAXED")]
    Relaxed,
}