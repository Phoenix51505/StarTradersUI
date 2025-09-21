namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

public enum ShipMountDeposit {
    [EnumMember(Value = "QUARTZ_SAND")]
    QuartzSand,
    [EnumMember(Value = "SILICON_CRYSTALS")]
    SiliconCrystals,
    [EnumMember(Value = "PRECIOUS_STONES")]
    PreciousStones,
    [EnumMember(Value = "ICE_WATER")]
    IceWater,
    [EnumMember(Value = "AMMONIA_ICE")]
    AmmoniaIce,
    [EnumMember(Value = "IRON_ORE")]
    IronOre,
    [EnumMember(Value = "COPPER_ORE")]
    CopperOre,
    [EnumMember(Value = "SILVER_ORE")]
    SilverOre,
    [EnumMember(Value = "ALUMINUM_ORE")]
    AluminumOre,
    [EnumMember(Value = "GOLD_ORE")]
    GoldOre,
    [EnumMember(Value = "PLATINUM_ORE")]
    PlatinumOre,
    [EnumMember(Value = "DIAMONDS")]
    Diamonds,
    [EnumMember(Value = "URANITE_ORE")]
    UraniteOre,
    [EnumMember(Value = "MERITIUM_ORE")]
    MeritiumOre,
}