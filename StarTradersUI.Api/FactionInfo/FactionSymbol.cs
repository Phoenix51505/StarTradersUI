using System.Runtime.Serialization;

namespace StarTradersUI.Api.FactionInfo;

/// <summary>
/// The symbol of the faction.
/// </summary>
public enum FactionSymbol
{
    [EnumMember(Value = "COSMIC")]
    Cosmic,
    [EnumMember(Value = "VOID")]
    Void,
    [EnumMember(Value = "GALACTIC")]
    Galactic,
    [EnumMember(Value = "QUANTUM")]
    Quantum,
    [EnumMember(Value = "DOMINION")]
    Dominion,
    [EnumMember(Value = "ASTRO")]
    Astro,
    [EnumMember(Value = "CORSAIRS")]
    Corsairs,
    [EnumMember(Value = "OBSIDIAN")]
    Obsidian,
    [EnumMember(Value = "AEGIS")]
    Aegis,
    [EnumMember(Value = "UNITED")]
    United,
    [EnumMember(Value = "SOLITARY")]
    Solitary,
    [EnumMember(Value = "COBALT")]
    Cobalt,
    [EnumMember(Value = "OMEGA")]
    Omega,
    [EnumMember(Value = "ECHO")]
    Echo,
    [EnumMember(Value = "LORDS")]
    Lords,
    [EnumMember(Value = "CULT")]
    Cult,
    [EnumMember(Value = "ANCIENTS")]
    Ancients,
    [EnumMember(Value = "SHADOW")]
    Shadow,
    [EnumMember(Value = "ETHEREAL")]
    Ethereal,
}