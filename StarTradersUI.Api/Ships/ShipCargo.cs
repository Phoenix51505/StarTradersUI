namespace StarTradersUI.Api.Ships;

public class ShipCargo
{
    public int Capacity { get; set; }
    public int Units { get; set; }
    public ShipCargoItem[] Inventory {get; set;}
}