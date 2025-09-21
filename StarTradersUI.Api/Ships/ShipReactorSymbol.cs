namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

/// <summary>
/// Symbol of the reactor.
/// </summary>
public enum ShipReactorSymbol {
    [EnumMember(Value = "REACTOR_SOLAR_I")]
    ReactorSolarI,
    [EnumMember(Value = "REACTOR_FUSION_I")]
    ReactorFusionI,
    [EnumMember(Value = "REACTOR_FISSION_I")]
    ReactorFissionI,
    [EnumMember(Value = "REACTOR_CHEMICAL_I")]
    ReactorChemicalI,
    [EnumMember(Value = "REACTOR_ANTIMATTER_I")]
    ReactorAntimatterI,
}