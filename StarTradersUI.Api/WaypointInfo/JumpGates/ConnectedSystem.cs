using StarTradersUI.Api.SystemInfo;

namespace StarTradersUI.Api.WaypointInfo.JumpGates;

public class ConnectedSystem
{
    /// <summary>
    /// The symbol of the system.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// The sector of this system
    /// </summary>
    public string SectorSymbol { get; set; }

    public SystemType Type { get; set; }

    public string? FactionSymbol { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Distance { get; set; }
}