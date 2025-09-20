namespace StarTradersUI.Api;

public class SystemWaypoint
{
    public string Symbol { get; set; }
    public WaypointType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public WaypointOrbital[] Orbitals { get; set; }
    public string? Orbits { get; set; }
}