namespace StarTradersUi.Api;

public class System
{
    public string Symbol { get; set; }
    public string SectorSymbol { get; set; }
    public string Constellation { get; set; }
    public string Name { get; set; }
    public SystemType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public SystemWaypoint[] Waypoints { get; set; }
    public SystemFaction[] Factions { get; set; }
}