namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

/// <summary>
/// The symbol of the engine.
/// </summary>
public enum ShipEngineSymbol {
    [EnumMember(Value = "ENGINE_IMPULSE_DRIVE_I")]
    EngineImpulseDriveI,
    [EnumMember(Value = "ENGINE_ION_DRIVE_I")]
    EngineIonDriveI,
    [EnumMember(Value = "ENGINE_ION_DRIVE_II")]
    EngineIonDriveIi,
    [EnumMember(Value = "ENGINE_HYPER_DRIVE_I")]
    EngineHyperDriveI,
}