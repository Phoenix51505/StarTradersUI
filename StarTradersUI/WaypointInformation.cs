using System;
using Avalonia.Media;
using StarTradersUI.Api.WaypointInfo;
using StarTradersUI.Utilities;

namespace StarTradersUI;

public class WaypointInformation(Waypoint waypoint) : IPositionable
{
    public readonly Waypoint Waypoint = waypoint;
    public double X => Waypoint.X + OffsetX;
    public double Y => Waypoint.Y + OffsetY;
    public FormattedText? WaypointNameText;
    public double WaypointNameWidth;
    public double OffsetX = 0;
    public double OffsetY = 0;
    public double Scale = GetWaypointScale(waypoint);
    private static double GetWaypointScale(Waypoint waypoint)
    {
        return waypoint.Type switch
        {
            WaypointType.Planet => 1,
            WaypointType.GasGiant => 2,
            WaypointType.Moon => 0.75,
            WaypointType.OrbitalStation => 0.125,
            WaypointType.JumpGate => 0.25,
            WaypointType.AsteroidField => 1,
            WaypointType.Asteroid => 0.25,
            WaypointType.EngineeredAsteroid => 0.25,
            WaypointType.AsteroidBase => 0.25,
            WaypointType.Nebula => 3,
            WaypointType.DebrisField => 0.25,
            WaypointType.GravityWell => 2,
            WaypointType.ArtificialGravityWell => 2,
            WaypointType.FuelStation => 0.25,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override int GetHashCode()
    {
        return Waypoint.Symbol.GetHashCode() * 27 + Waypoint.SystemSymbol.GetHashCode() * 17;
    }
}