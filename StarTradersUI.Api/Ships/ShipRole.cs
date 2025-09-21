namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

/// <summary>
/// The registered role of the ship
/// </summary>
public enum ShipRole {
    [EnumMember(Value = "FABRICATOR")]
    Fabricator,
    [EnumMember(Value = "HARVESTER")]
    Harvester,
    [EnumMember(Value = "HAULER")]
    Hauler,
    [EnumMember(Value = "INTERCEPTOR")]
    Interceptor,
    [EnumMember(Value = "EXCAVATOR")]
    Excavator,
    [EnumMember(Value = "TRANSPORT")]
    Transport,
    [EnumMember(Value = "REPAIR")]
    Repair,
    [EnumMember(Value = "SURVEYOR")]
    Surveyor,
    [EnumMember(Value = "COMMAND")]
    Command,
    [EnumMember(Value = "CARRIER")]
    Carrier,
    [EnumMember(Value = "PATROL")]
    Patrol,
    [EnumMember(Value = "SATELLITE")]
    Satellite,
    [EnumMember(Value = "EXPLORER")]
    Explorer,
    [EnumMember(Value = "REFINERY")]
    Refinery,
}