namespace StarTradersUI.Api.Ships;

public class ShipModule : BaseShipPart<ShipModuleSymbol>
{
    public int? Capacity { get; set; }
    public int? Range { get; set; }
}