namespace StarTradersUI.Api.Ships.Route;

public class ShipNavRoute
{
    public ShipNavRouteWaypoint Origin { get; set; }
    public ShipNavRouteWaypoint Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
}