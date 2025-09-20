using StarTradersUI.Api.Trading;

namespace StarTradersUI.Api.WaypointInfo.WaypointConstruction;

public class ConstructionMaterial
{
    public TradeSymbol TradeSymbol { get; set; }
    public int Required { get; set; }
    public int Fulfilled { get; set; }
}