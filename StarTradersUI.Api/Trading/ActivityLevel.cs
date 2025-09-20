namespace StarTradersUI.Api.Trading;

using System.Runtime.Serialization;

/// <summary>
/// The activity level of a trade good. If the good is an import, this represents how strong consumption is. If the good is an export, this represents how strong the production is for the good. When activity is strong, consumption or production is near maximum capacity. When activity is weak, consumption or production is near minimum capacity.
/// </summary>
public enum ActivityLevel
{
    /// <summary>
    /// Indicates very low production or consumption activity. This may suggest a surplus in supply or a lack of demand.
    /// </summary>
    [EnumMember(Value = "WEAK")] Weak,

    /// <summary>
    /// Represents increasing activity in production or consumption, suggesting a developing market.
    /// </summary>
    [EnumMember(Value = "GROWING")] Growing,

    /// <summary>
    /// Signifies high levels of production or consumption. Indicates a healthy and active market with high demand or supply.
    /// </summary>
    [EnumMember(Value = "STRONG")] Strong,

    /// <summary>
    /// Reflects a bottleneck in production or consumption, possibly due to a lack of supply or demand in related goods.
    /// </summary>
    [EnumMember(Value = "RESTRICTED")] Restricted,
}