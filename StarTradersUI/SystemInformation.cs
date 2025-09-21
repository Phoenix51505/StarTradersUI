using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using StarTradersUI.Api.SystemInfo;
using StarTradersUI.Controls;
using StarTradersUI.Utilities;
using StarTradersUI.Utilities.Drawing;
using StarTradersUI.Utilities.Interfaces;

namespace StarTradersUI;

public class SystemInformation(Api.SystemInfo.System system) : ISystemRenderable
{
    public readonly Api.SystemInfo.System System = system;
    public double X => System.X;
    public double Y => System.Y;
    public double Scale { get; set; } = GetSystemScale(system);
    public static implicit operator SystemInformation(Api.SystemInfo.System system) => new(system);

    #region Cached data for drawing

    public FormattedText? SystemNameText;
    public double SystemNameWidth;

    #endregion

    private const double SystemDefaultScale = 0.5d;

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
    } * SystemDefaultScale;

    public override int GetHashCode()
    {
        return System.Symbol.GetHashCode();
    }

    #region Drawing

    private static readonly Brush NeutronStarDefaultBrush = new SolidColorBrush(new Color(255, 210, 237, 23));
    private static readonly Brush RedStarDefaultBrush = new SolidColorBrush(Colors.Red);
    private static readonly Brush OrangeStarDefaultBrush = new SolidColorBrush(Colors.Orange);
    private static readonly Brush BlueStarDefaultBrush = new SolidColorBrush(Colors.Blue);
    private static readonly Brush YoungStarDefaultBrush = new SolidColorBrush(Colors.Yellow);
    private static readonly Brush WhiteDwarfDefaultBrush = new SolidColorBrush(Colors.White);
    private static readonly Brush BlackHoleDefaultBrush = new SolidColorBrush(Colors.DimGray);
    private static readonly Brush HyperGiantDefaultBrush = new SolidColorBrush(Colors.OrangeRed);
    private static readonly Brush NebulaDefaultBrush = new SolidColorBrush(Colors.DeepPink);
    private static readonly Brush UnstableDefaultBrush = new SolidColorBrush(Colors.Purple);
    private static readonly Brush NebulaBackground = new SolidColorBrush(new Color(255, 74, 95, 159));
    private static readonly Brush NebulaBorder = new SolidColorBrush(new Color(255, 199, 116, 87));
    private static readonly Brush NebulaStar = new SolidColorBrush(new Color(255, 174, 223, 235));

    // We should set up the render functions in here, as more things will be rendered than just this
    public void Render(SystemDrawer drawer, DrawingContext context, Point center, double scaledSize)
    {
        switch (System.Type)
        {
            case SystemType.NeutronStar:
                DrawSystem(drawer, context, NeutronStarDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(NeutronStarDefaultBrush));
                break;
            case SystemType.RedStar:
                DrawSystem(drawer, context, RedStarDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(RedStarDefaultBrush));
                break;
            case SystemType.OrangeStar:
                DrawSystem(drawer, context, OrangeStarDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(OrangeStarDefaultBrush));
                break;
            case SystemType.BlueStar:
                DrawSystem(drawer, context, BlueStarDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(BlueStarDefaultBrush));
                break;
            case SystemType.YoungStar:
                DrawSystem(drawer, context, YoungStarDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(YoungStarDefaultBrush));
                break;
            case SystemType.WhiteDwarf:
                DrawSystem(drawer, context, WhiteDwarfDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(WhiteDwarfDefaultBrush));
                break;
            case SystemType.BlackHole:
                DrawSystem(drawer, context, BlackHoleDefaultBrush, center, scaledSize, DrawBlackHole);
                break;
            case SystemType.Hypergiant:
                DrawSystem(drawer, context, HyperGiantDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(HyperGiantDefaultBrush));
                break;
            case SystemType.Nebula:
                DrawSystem(drawer, context, NebulaDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(NebulaBackground, NebulaStar, NebulaBorder));
                break;
            case SystemType.Unstable:
                DrawSystem(drawer, context, UnstableDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<SystemInformation>(UnstableDefaultBrush,
                        border: new SolidColorBrush(Colors.Purple)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DrawSystem(SystemDrawer drawer, DrawingContext context, Brush brush, Point location, double scaledSize,
        Action<DrawingContext, Point, double, SystemInformation>? drawLarger = null)
    {
        if (scaledSize <= 1)
        {
            context.DrawEllipse(brush, null, location, 0.5, 0.5);
        }
        else
        {
            if (drawLarger != null)
            {
                drawLarger(context, location, scaledSize, System);
            }
            else
            {
                context.DrawEllipse(brush, null, location, scaledSize / 2, scaledSize / 2);
            }
        }

        // At this point, we need to start drawing text and other decorations
        if (drawer.UniverseUnitsPerPixel <= 3)
        {
            DrawSystemDecorations(drawer, context, location, scaledSize);
        }
    }


    private void DrawSystemDecorations(SystemDrawer drawer, DrawingContext context, Point location, double scaledSize)
    {
        var bottom = location.Y + scaledSize * 1.1 / 2;
        var topOfText = bottom + 3;
        double width;
        if (SystemNameText is { } systemNameText)
        {
            width = SystemNameWidth;
        }
        else
        {
            systemNameText = SystemNameText = new FormattedText($"{System.Symbol} - {System.Name}",
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, Typeface.Default, 14, Brushes.White);
            width = SystemNameWidth = systemNameText.Width;
        }

        context.DrawText(systemNameText, new Point(location.X - width / 2, topOfText));

        if (scaledSize >= 9)
        {
            drawer.FactionDrawer.DrawSymbolsLarge(context, location, scaledSize,
                System.Factions.Select(x => (x.Symbol, FactionUtilities.GetFactionIcon(x.Symbol))).ToArray());
        }
    }

    private static readonly Brush BlackHoleLargerInside = new SolidColorBrush(new Color(255, 16, 16, 16));

    private static void DrawBlackHole(DrawingContext context, Point location, double scaledSize, SystemInformation _)
    {
        Pen blackHoleBorder = new(Brushes.OrangeRed, Math.Max(1, scaledSize / 20));
        context.DrawEllipse(BlackHoleLargerInside, blackHoleBorder, location, scaledSize / 2, scaledSize / 2);
    }

    #endregion
}