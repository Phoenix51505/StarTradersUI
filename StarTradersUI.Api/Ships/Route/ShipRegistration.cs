using StarTradersUI.Api.FactionInfo;

namespace StarTradersUI.Api.Ships.Route;

public class ShipRegistration
{
    public string Name { get; set; }
    public FactionSymbol Faction { get; set; }
    public ShipRole Role { get; set; }
}