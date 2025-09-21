using StarTradersUI.Api.Trading;

namespace StarTradersUI.Api.Ships;

public class ShipCargoItem
{
    public TradeSymbol Symbol {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public int Units { get; set; }
}