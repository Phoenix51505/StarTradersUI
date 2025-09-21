using StarTradersUI.Api.Ships.Route;

namespace StarTradersUI.Api.Ships;

public class Ship
{
    public string Symbol { get; set; }
    public ShipRegistration Registration { get; set; }
    public ShipNav Nav { get; set; }
    public ShipCrew Crew { get; set; }
    public DegradableShipFrame Frame { get; set; }
    public DegradableShipReactor Reactor { get; set; }
    public DegradableShipEngine Engine { get; set; }
    public Cooldown Cooldown { get; set; }
    public ShipModule[] Modules { get; set; }
    public ShipMount[] Mounts { get; set; }
    public ShipCargo Cargo { get; set; }
    public ShipFuel Fuel { get; set; }
}