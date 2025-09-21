using StarTradersUI.Api.WaypointInfo;

namespace StarTradersUI.Api.Ships.Route;

public class ShipNavRouteWaypoint
{
    public string Symbol { get; set; }
    public WaypointType Type { get; set; }
    public string SystemSymbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}