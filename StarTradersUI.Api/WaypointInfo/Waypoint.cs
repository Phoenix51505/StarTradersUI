namespace StarTradersUI.Api.WaypointInfo;

public class Waypoint
{
    public string Symbol { get; set; }
    public WaypointType Type { get; set; }
    public string SystemSymbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public WaypointOrbital[] Orbitals { get; set; }
    public string? Orbits { get; set; }
    public WaypointFaction? Faction { get; set; }
    public WaypointTrait[] Traits { get; set; }
    public WaypointModifier[]? Modifiers { get; set; }
    public Chart? Chart { get; set; }
    public bool IsUnderConstruction { get; set; }
}