using StarTradersUI.Api.FactionInfo;

namespace StarTradersUI.Api.Agents;

public class AgentRegisterRequest
{
    public string Symbol { get; set; }
    public FactionSymbol Faction { get; set; }
}