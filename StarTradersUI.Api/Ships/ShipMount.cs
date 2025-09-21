namespace StarTradersUI.Api.Ships;

public class ShipMount : BaseShipPart<ShipMountSymbol>
{
    public int? Strength { get; set; }
    public ShipMountDeposit[]? Deposits { get; set; }
}