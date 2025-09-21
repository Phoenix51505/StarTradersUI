namespace StarTradersUI.Api.Ships.Route;

public class ShipNav
{
    public string SystemSymbol { get; set; }
    public string WaypointSymbol { get; set; }
    public ShipNavRoute Route { get; set; }
    public ShipNavStatus Status { get; set; }
    public ShipNavFlightMode FlightMode { get; set; } = ShipNavFlightMode.Cruise;
}