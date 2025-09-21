namespace StarTradersUI.Api.Ships;

using System.Runtime.Serialization;

/// <summary>
/// The symbol of the module.
/// </summary>
public enum ShipModuleSymbol {
    [EnumMember(Value = "MODULE_MINERAL_PROCESSOR_I")]
    ModuleMineralProcessorI,
    [EnumMember(Value = "MODULE_GAS_PROCESSOR_I")]
    ModuleGasProcessorI,
    [EnumMember(Value = "MODULE_CARGO_HOLD_I")]
    ModuleCargoHoldI,
    [EnumMember(Value = "MODULE_CARGO_HOLD_II")]
    ModuleCargoHoldIi,
    [EnumMember(Value = "MODULE_CARGO_HOLD_III")]
    ModuleCargoHoldIii,
    [EnumMember(Value = "MODULE_CREW_QUARTERS_I")]
    ModuleCrewQuartersI,
    [EnumMember(Value = "MODULE_ENVOY_QUARTERS_I")]
    ModuleEnvoyQuartersI,
    [EnumMember(Value = "MODULE_PASSENGER_CABIN_I")]
    ModulePassengerCabinI,
    [EnumMember(Value = "MODULE_MICRO_REFINERY_I")]
    ModuleMicroRefineryI,
    [EnumMember(Value = "MODULE_ORE_REFINERY_I")]
    ModuleOreRefineryI,
    [EnumMember(Value = "MODULE_FUEL_REFINERY_I")]
    ModuleFuelRefineryI,
    [EnumMember(Value = "MODULE_SCIENCE_LAB_I")]
    ModuleScienceLabI,
    [EnumMember(Value = "MODULE_JUMP_DRIVE_I")]
    ModuleJumpDriveI,
    [EnumMember(Value = "MODULE_JUMP_DRIVE_II")]
    ModuleJumpDriveIi,
    [EnumMember(Value = "MODULE_JUMP_DRIVE_III")]
    ModuleJumpDriveIii,
    [EnumMember(Value = "MODULE_WARP_DRIVE_I")]
    ModuleWarpDriveI,
    [EnumMember(Value = "MODULE_WARP_DRIVE_II")]
    ModuleWarpDriveIi,
    [EnumMember(Value = "MODULE_WARP_DRIVE_III")]
    ModuleWarpDriveIii,
    [EnumMember(Value = "MODULE_SHIELD_GENERATOR_I")]
    ModuleShieldGeneratorI,
    [EnumMember(Value = "MODULE_SHIELD_GENERATOR_II")]
    ModuleShieldGeneratorIi,
}