using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using StarTradersUI.Api.WaypointInfo;
using StarTradersUI.Controls;
using StarTradersUI.Utilities;
using StarTradersUI.Utilities.Drawing;
using StarTradersUI.Utilities.Interfaces;

namespace StarTradersUI;

public class WaypointInformation(Waypoint waypoint) : ISystemRenderable
{
    public readonly Waypoint Waypoint = waypoint;
    public double X => Waypoint.X + OffsetX;
    public double Y => Waypoint.Y + OffsetY;
    public FormattedText? WaypointNameText;
    public double WaypointNameWidth;
    public double OffsetX = 0;
    public double OffsetY = 0;
    public double Scale { get; set; } = GetWaypointScale(waypoint);
    public const double WaypointDefaultScale = 1 / 5d;
    private static double GetWaypointScale(Waypoint waypoint)
    {
        return waypoint.Type switch
        {
            WaypointType.Planet => 1,
            WaypointType.GasGiant => 2,
            WaypointType.Moon => 1,
            WaypointType.OrbitalStation => 0.5,
            WaypointType.JumpGate => 0.33,
            WaypointType.AsteroidField => 1,
            WaypointType.Asteroid => 0.33,
            WaypointType.EngineeredAsteroid => 0.33,
            WaypointType.AsteroidBase => 0.33,
            WaypointType.Nebula => 3,
            WaypointType.DebrisField => 0.33,
            WaypointType.GravityWell => 2,
            WaypointType.ArtificialGravityWell => 2,
            WaypointType.FuelStation => 0.33,
            _ => throw new ArgumentOutOfRangeException()
        } * WaypointDefaultScale;
    }

    public override int GetHashCode()
    {
        return Waypoint.Symbol.GetHashCode() * 27 + Waypoint.SystemSymbol.GetHashCode() * 17;
    }

    #region Rendering
    private static readonly Brush PlanetBrush = new SolidColorBrush(Colors.Green);
    private static readonly Brush GasGiantBrush = new SolidColorBrush(Colors.Blue);
    private static readonly Brush MoonBrush = new SolidColorBrush(Colors.Gray);
    private static readonly Brush OrbitalStationBrush = new SolidColorBrush(Colors.Orange);
    private static readonly Brush JumpGateBrush = new SolidColorBrush(Colors.Purple);
    private static readonly Brush AsteroidFieldBrush = new SolidColorBrush(Colors.SaddleBrown);
    private static readonly Brush AsteroidBrush = new SolidColorBrush(Colors.DarkGray);
    private static readonly Brush EngineeredAsteroidBrush = new SolidColorBrush(Colors.LightGray);
    private static readonly Brush AsteroidBaseBrush = new SolidColorBrush(Colors.SlateGray);
    private static readonly Brush DebrisFieldBrush = new SolidColorBrush(Colors.OrangeRed);
    private static readonly Brush GravityWellBrush = new SolidColorBrush(Colors.Pink);
    private static readonly Brush ArtificialGravityWellBrush = new SolidColorBrush(Colors.DeepPink);
    private static readonly Brush FuelStationBrush = new SolidColorBrush(Colors.Red);
    private static readonly Brush NebulaDefaultBrush = new SolidColorBrush(Colors.DeepPink);
    private static readonly Brush NebulaBackground = new SolidColorBrush(new Color(255, 74, 95, 159));
    private static readonly Brush NebulaBorder = new SolidColorBrush(new Color(255, 199, 116, 87));
    private static readonly Brush NebulaStar = new SolidColorBrush(new Color(255, 174, 223, 235));
    
    public void Render(SystemDrawer drawer, DrawingContext context, Point center, double scaledSize)
    {
        switch (Waypoint.Type)
        {
            case WaypointType.Planet:
                DrawWaypoint(drawer,context, PlanetBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<WaypointInformation>(PlanetBrush, GasGiantBrush));
                break;
            case WaypointType.GasGiant:
                DrawWaypoint(drawer,context, GasGiantBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<WaypointInformation>(GasGiantBrush));
                break;
            case WaypointType.Moon:
                DrawWaypoint(drawer,context, MoonBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<WaypointInformation>(MoonBrush));
                break;
            case WaypointType.OrbitalStation:
                DrawWaypoint(drawer,context, OrbitalStationBrush, center, scaledSize, DrawOrbitalStation);
                break;
            case WaypointType.JumpGate:
                DrawWaypoint(drawer,context, JumpGateBrush, center, scaledSize,
                    DrawWell(JumpGateBrush, 2, 1 / 20d));
                break;
            case WaypointType.AsteroidField:
                DrawWaypoint(drawer,context, AsteroidFieldBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<WaypointInformation>(null!, AsteroidFieldBrush,
                        new SolidColorBrush(Colors.Transparent)));
                break;
            case WaypointType.Asteroid:
                DrawWaypoint(drawer,context, AsteroidBrush, center, scaledSize, DrawAsteroid);
                break;
            case WaypointType.EngineeredAsteroid:
                DrawWaypoint(drawer,context, EngineeredAsteroidBrush, center, scaledSize,
                    DrawEngineeredAsteroid);
                break;
            case WaypointType.AsteroidBase:
                DrawWaypoint(drawer,context, AsteroidBaseBrush, center, scaledSize, DrawAsteroidBase);
                break;
            case WaypointType.Nebula:
                DrawWaypoint(drawer,context, NebulaDefaultBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<WaypointInformation>(NebulaBackground, NebulaBorder, NebulaBorder));
                break;
            case WaypointType.DebrisField:
                DrawWaypoint(drawer,context, DebrisFieldBrush, center, scaledSize,
                    BodyDrawingUtils.DrawSpotted<WaypointInformation>(null!, DebrisFieldBrush, new SolidColorBrush(Colors.Transparent)));
                break;
            case WaypointType.GravityWell:
                DrawWaypoint(drawer,context, GravityWellBrush, center, scaledSize,
                    DrawWell(GravityWellBrush, 10, 1 / 60d));
                break;
            case WaypointType.ArtificialGravityWell:
                DrawWaypoint(drawer,context, ArtificialGravityWellBrush, center, scaledSize,
                    DrawWell(ArtificialGravityWellBrush, 10, 1 / 60d));
                break;
            case WaypointType.FuelStation:
                DrawWaypoint(drawer,context, FuelStationBrush, center, scaledSize, DrawFuelStation);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    

    public void DrawWaypoint(SystemDrawer drawer, DrawingContext context, Brush brush, Point location, double scaledSize,
        Action<DrawingContext, Point, double, WaypointInformation>? drawLarger = null)
    {
        if (scaledSize <= 1)
        {
            context.DrawEllipse(brush, null, location, 0.5, 0.5);
        }
        else
        {
            if (drawLarger != null)
            {
                drawLarger(context, location, scaledSize, this);
            }
            else
            {
                context.DrawEllipse(brush, null, location, scaledSize / 2, scaledSize / 2);
            }
        }

        DrawWaypointDecorations(drawer, context, location, scaledSize);
    }
    
    

    private void DrawWaypointDecorations(SystemDrawer drawer, DrawingContext context, Point location, double scaledSize)
    {
        var bottom = location.Y + scaledSize / 2;
        var topOfText = bottom + 3;
        double width;
        if (WaypointNameText is { } waypointNameText)
        {
            width = WaypointNameWidth;
        }
        else
        {
            waypointNameText = WaypointNameText = new FormattedText($"{Waypoint.Symbol}",
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, Typeface.Default, 10, Brushes.White);
            width = WaypointNameWidth = waypointNameText.Width;
        }

        context.DrawText(waypointNameText, new Point(location.X - width / 2, topOfText));

        if (Waypoint.Faction is { Symbol: var sym })
        {
            drawer.FactionDrawer.DrawSymbolsLarge(context, location, scaledSize, [(Waypoint.Faction.Symbol, FactionUtilities.GetFactionIcon(sym))]);
        }

        if (scaledSize >= 9)
        {
            drawer.WaypointDrawer.DrawSymbolsLarge(context, location, scaledSize,
                Waypoint.Traits.Select(x => (x, WaypointUtilities.GetWaypointTraitImage(x.Symbol))).ToArray(),
                true);
        }
    }
    
    private static readonly (double x, double y)[][] AsteroidPoints =
    [
        GenerateAsteroidPoints(10), GenerateAsteroidPoints(10), GenerateAsteroidPoints(10), GenerateAsteroidPoints(10),
        GenerateAsteroidPoints(15), GenerateAsteroidPoints(15), GenerateAsteroidPoints(15), GenerateAsteroidPoints(15),
        GenerateAsteroidPoints(20), GenerateAsteroidPoints(20), GenerateAsteroidPoints(20), GenerateAsteroidPoints(20),
        GenerateAsteroidPoints(25), GenerateAsteroidPoints(25), GenerateAsteroidPoints(25), GenerateAsteroidPoints(25),
        GenerateAsteroidPoints(30), GenerateAsteroidPoints(30), GenerateAsteroidPoints(30), GenerateAsteroidPoints(30),
    ];

    private static (double x, double y)[] GenerateAsteroidPoints(int size)
    {
        var result = new (double x, double y)[size];
        var thetaStep = 2 * Math.PI / size;
        var random = new Random();
        for (var i = 0; i < size; i++)
        {
            var theta = i * thetaStep;
            var r = Math.Sqrt(Uniform(0.25, 1)); // 0.5 squared, 1 squared, 
            var x = r * Math.Cos(theta);
            var y = r * Math.Sin(theta);
            result[i] = (x, y);
        }

        return result;

        double Uniform(double min, double max)
        {
            var distance = max - min;
            var value = random.NextDouble();
            return (value * distance) + min;
        }
    }

    private static void DrawAsteroid(DrawingContext context, Point location, double scaledSize, WaypointInformation waypoint)
    {
        var streamGeometry = new StreamGeometry();
        using (var ctx = streamGeometry.Open())
        {
            var points = AsteroidPoints[Math.Abs(waypoint.GetHashCode()) % AsteroidPoints.Length];
            var start = points[0];
            ctx.BeginFigure(new Point(location.X + start.x * scaledSize / 2, location.Y + start.y * scaledSize / 2),
                true);
            for (var i = 1; i < points.Length; i++)
            {
                var point = points[i];
                ctx.LineTo(new Point(location.X + point.x * scaledSize / 2, location.Y + point.y * scaledSize / 2));
            }

            ctx.EndFigure(true);
        }

        context.DrawGeometry(AsteroidBrush, null, streamGeometry);
    }

    private static void DrawEngineeredAsteroid(DrawingContext context, Point location, double scaledSize,
        WaypointInformation waypoint)
    {
        var streamGeometry = new StreamGeometry();
        using (var ctx = streamGeometry.Open())
        {
            var points = AsteroidPoints[Math.Abs(waypoint.GetHashCode()) % AsteroidPoints.Length];
            var start = points[0];
            ctx.BeginFigure(new Point(location.X + start.x * scaledSize / 2.1, location.Y + start.y * scaledSize / 2.1),
                true);
            for (var i = 1; i < points.Length; i++)
            {
                var point = points[i];
                ctx.LineTo(new Point(location.X + point.x * scaledSize / 2.1, location.Y + point.y * scaledSize / 2.1));
            }

            ctx.EndFigure(true);
        }

        var pen = new Pen(Brushes.OrangeRed, scaledSize / 40d);

        context.DrawGeometry(AsteroidBrush, pen, streamGeometry);
    }

    private static readonly Brush AsteroidBaseAtmosphere = new SolidColorBrush(new Color(127, 173, 216, 230));

    private static void DrawAsteroidBase(DrawingContext context, Point location, double scaledSize,
        WaypointInformation waypoint)
    {
        DrawEngineeredAsteroid(context, location, scaledSize, waypoint);
        context.DrawEllipse(AsteroidBaseAtmosphere, null, location, scaledSize / 2, scaledSize / 2);
    }
    
    


    private Action<DrawingContext, Point, double, WaypointInformation> DrawWell(Brush ringColor, int nRings,
        double ringThicknessScale)
    {
        return Draw;

        void Draw(DrawingContext context, Point location, double scaledSize, WaypointInformation waypoint)
        {
            var ringStep = scaledSize / nRings;
            var startingSize = scaledSize;
            var ringPen = new Pen(ringColor, Math.Max(1, ringThicknessScale * scaledSize));
            for (var i = 0; i < nRings; i++)
            {
                context.DrawEllipse(null, ringPen, location, startingSize / 2, startingSize / 2);
                startingSize -= ringStep;
            }
        }
    }

    private static void DrawOrbitalStation(DrawingContext context, Point location, double scaledSize,
        WaypointInformation waypoint)
    {
        // What do we want, hmmm, maybe like a rounded line down the center, and 2 spokes out to the side, the left being orange, and the right being blue
        var coreBrush = Brushes.LightSlateGray;
        var coreWidth = Math.Max(1, scaledSize / 15);
        var corePen = new Pen(coreBrush, coreWidth, lineCap: PenLineCap.Round);
        var panelBrush = OrbitalStationBrush;
        context.DrawRectangle(panelBrush, null,
            new Rect(location.X - scaledSize / 2 + coreWidth / 2, location.Y - scaledSize / 2, scaledSize / 3,
                scaledSize));
        context.DrawRectangle(panelBrush, null,
            new Rect(location.X + scaledSize / 6 - coreWidth / 2, location.Y - scaledSize / 2, scaledSize / 3,
                scaledSize));
        context.DrawLine(corePen, new Point(location.X, location.Y - scaledSize / 2 + coreWidth / 2),
            new Point(location.X, location.Y + scaledSize / 2 - coreWidth / 2));
        context.DrawLine(corePen, new Point(location.X - scaledSize / 2 + coreWidth / 2, location.Y - scaledSize / 4),
            new Point(location.X + scaledSize / 2 - coreWidth / 2, location.Y - scaledSize / 4));
        context.DrawLine(corePen, new Point(location.X - scaledSize / 2 + coreWidth / 2, location.Y),
            new Point(location.X + scaledSize / 2 - coreWidth / 2, location.Y));
        context.DrawLine(corePen, new Point(location.X - scaledSize / 2 + coreWidth / 2, location.Y + scaledSize / 4),
            new Point(location.X + scaledSize / 2 - coreWidth / 2, location.Y + scaledSize / 4));
    }

    private static void DrawFuelStation(DrawingContext context, Point location, double scaledSize,
        WaypointInformation waypoint)
    {
        var coreBrush = Brushes.LightSlateGray;
        var coreWidth = Math.Max(1, scaledSize / 15);
        var corePen = new Pen(coreBrush, coreWidth, lineCap: PenLineCap.Round);
        var tankBrush = FuelStationBrush;
        context.DrawRectangle(tankBrush, null,
            new Rect(location.X - scaledSize / 3, location.Y - scaledSize / 2, 2 * scaledSize / 3, scaledSize),
            radiusX: scaledSize / 20, radiusY: scaledSize / 20);
        context.DrawLine(corePen, new Point(location.X - scaledSize / 2 + coreWidth / 2, location.Y - scaledSize / 4),
            new Point(location.X + scaledSize / 2 - coreWidth / 2, location.Y - scaledSize / 4));
        context.DrawLine(corePen, new Point(location.X - scaledSize / 2 + coreWidth / 2, location.Y),
            new Point(location.X + scaledSize / 2 - coreWidth / 2, location.Y));
        context.DrawLine(corePen, new Point(location.X - scaledSize / 2 + coreWidth / 2, location.Y + scaledSize / 4),
            new Point(location.X + scaledSize / 2 - coreWidth / 2, location.Y + scaledSize / 4));
    }
    #endregion

}