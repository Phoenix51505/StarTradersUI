using System;
using Avalonia.Media;
using StarTradersUI.Api.SystemInfo;
using StarTradersUI.Utilities;

namespace StarTradersUI;

public class SystemInformation(Api.SystemInfo.System system) : IPositionable
{
    public readonly Api.SystemInfo.System System = system;
    public double X => System.X;
    public double Y => System.Y;
    public readonly double Scale = GetSystemScale(system);
    public static implicit operator SystemInformation(Api.SystemInfo.System system) => new(system);

    #region Cached data for drawing

    public FormattedText? SystemNameText;
    public double SystemNameWidth;

    #endregion

    private static double GetSystemScale(Api.SystemInfo.System system) => system.Type switch
    {
        SystemType.NeutronStar => 1,
        SystemType.RedStar => 1,
        SystemType.OrangeStar => 1,
        SystemType.BlueStar => 1,
        SystemType.YoungStar => 0.75,
        SystemType.WhiteDwarf => 0.5,
        SystemType.BlackHole => 1.5,
        SystemType.Hypergiant => 2,
        SystemType.Nebula => 2.5,
        SystemType.Unstable => 1,
        _ => throw new ArgumentOutOfRangeException()
    };

    public override int GetHashCode()
    {
        return System.Symbol.GetHashCode();
    }
}