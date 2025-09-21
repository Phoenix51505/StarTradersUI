namespace StarTradersUI.Api.Ships;

public class BaseShipPart<TSymbol>
{
    public TSymbol Symbol { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ShipRequirements Requirements { get; set; }
}