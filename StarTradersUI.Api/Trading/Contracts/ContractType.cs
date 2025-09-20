namespace StarTradersUI.Api.Trading.Contracts;
using System.Runtime.Serialization;

/// <summary>
/// Type of contract.
/// </summary>
public enum ContractType {
    [EnumMember(Value = "PROCUREMENT")]
    Procurement,
    [EnumMember(Value = "TRANSPORT")]
    Transport,
    [EnumMember(Value = "SHUTTLE")]
    Shuttle,
}