

using StarTradersUI.Api.FactionInfo;
using StarTradersUI.Api.Ships;
using StarTradersUI.Api.Trading.Contracts;

namespace StarTradersUI.Api.Agents;

public class AgentRegisterResponse
{
    public Agent Agent { get; set; }
    public Contract Contract { get; set; }
    public Faction Faction { get; set; }
    public Ship[] Ships { get; set; }
    public string Token { get; set; }
}