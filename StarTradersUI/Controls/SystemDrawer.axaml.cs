using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using StarTradersUI.Api;
using StarTradersUI.Api.SystemInfo;
using StarTradersUI.Api.WaypointInfo;
using StarTradersUI.Utilities;

namespace StarTradersUI.Controls;

public partial class SystemDrawer : UserControl
{
    private static Cursor _defaultCursor = Cursor.Default;
    private static Cursor _handCursor = new Cursor(StandardCursorType.Hand);

    // This is meant to draw the entire system, including waypoints when zoomed in far enough


    public double ViewportCenterX;
    public double ViewportCenterY;
    public double ViewportScale = MaxScale / 2; // This is the width of the Viewport in universe units
    public bool IsDragging;

    private const double SystemViewLoadUpp = 0.5; // 100 pixels wide is how big we want to load in a system
    private const double SystemViewUnloadUpp = 0.75;

    private const double MaxScale = 64000;
    private const double MinScale = 1;


    private int _lastInvalidationType = 0;
    private const int InvalidationTypeZoomIn = 1;
    private const int InvalidationTypeRecalculate = 2;

    private double CenterX => Width / 2;
    private double CenterY => Height / 2;

    private double UniverseUnitsPerPixel => ViewportScale / Width;

    private int _waypointCacheGeneration = 0;


    // Since we are going to draw waypoints at 1/10th the scale 

    public SystemDrawer()
    {
        InitializeComponent();
        GlobalStates.DoWhenInitialized(() =>
        {
            _initialized = true;
            _lastInvalidationType = InvalidationTypeRecalculate;
            if (UniverseUnitsPerPixel <= SystemViewLoadUpp)
            {
                var closestSystem = GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!;
                DispatchWaypointUpdate(closestSystem);
            }

            InvalidateVisual();
        });
    }

    private Point _lastPosition = new(0, 0);

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Cursor = _handCursor;
        IsDragging = true;
        _lastPosition = e.GetPosition(this);
        // Todo here we we have to check if a waypoint or system was hit eventually, but not yet
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        Cursor = null;
        IsDragging = false;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var currentPosition = e.GetPosition(this);
        // var positionRelativeToCenter = (x: currentPosition.X - CenterX, y: currentPosition.Y - CenterY);
        // var globalZoomPositionRelativeToCenter = (x: positionRelativeToCenter.x * UniverseUnitsPerPixel,
        //     y: positionRelativeToCenter.y * UniverseUnitsPerPixel);
        // Console.WriteLine($"Moving cursor over {globalZoomPositionRelativeToCenter.x}, {globalZoomPositionRelativeToCenter.y}");
        if (IsDragging)
        {
            var delta = (x: currentPosition.X - _lastPosition.X, y: currentPosition.Y - _lastPosition.Y);
            var deltaUniverse = (x: delta.x * UniverseUnitsPerPixel, y: delta.y * UniverseUnitsPerPixel);
            ViewportCenterX -= deltaUniverse.x;
            ViewportCenterY -= deltaUniverse.y;
            _lastInvalidationType = InvalidationTypeRecalculate;

            if (_initialized && _waypointParentSystemInfo != null)
            {
                var closestSystem = GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!;
                if (!ReferenceEquals(closestSystem, _waypointParentSystemInfo))
                {
                    var distanceToOther = closestSystem.DistanceTo(CenterX, CenterY);
                    var distanceToCurrent = _waypointParentSystemInfo.DistanceTo(CenterX, CenterY);
                    if (distanceToCurrent > distanceToOther * 0.33)
                    {
                        DispatchWaypointUpdate(closestSystem);
                    }
                }
            }

            InvalidateVisual();
        }
        else
        {
            if (IsOverSystem(currentPosition.X, currentPosition.Y) != null ||
                IsOverWaypoint(currentPosition.X, currentPosition.Y) != null)
            {
                Cursor = _handCursor;
            }
            else
            {
                Cursor = null;
            }
        }

        _lastPosition = currentPosition;
    }

    private SystemInformation? _waypointParentSystemInfo;
    private WaypointInformation[]? _waypointInformations;

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        var pointerPos = e.GetPosition(this);
        var positionRelativeToCenter = (x: pointerPos.X - CenterX, y: pointerPos.Y - CenterY);
        var globalZoomPositionRelativeToCenter = (x: positionRelativeToCenter.x * UniverseUnitsPerPixel,
            y: positionRelativeToCenter.y * UniverseUnitsPerPixel);
        var ratio = e.Delta.Y switch
        {
            < 0 => Math.Pow(1.1, -e.Delta.Y),
            > 0 => Math.Pow(0.9, e.Delta.Y),
            _ => 1
        };
        if (Math.Abs(ratio - 1) < 0.01) return;
        var newScale = Math.Clamp(ViewportScale * ratio, MinScale, MaxScale);
        var oldUnitsPerPixel = UniverseUnitsPerPixel;
        ratio = newScale / ViewportScale;
        ViewportCenterX += globalZoomPositionRelativeToCenter.x * (1 - ratio);
        ViewportCenterY += globalZoomPositionRelativeToCenter.y * (1 - ratio);
        ViewportScale = newScale;
        var newUnitsPerPixel = UniverseUnitsPerPixel;


        if (newUnitsPerPixel >= SystemViewUnloadUpp)
        {
            // Unload the waypoints here
            DispatchWaypointClear();
        }
        
        if ((newUnitsPerPixel < SystemViewLoadUpp || _waypointParentSystemInfo != null) && _initialized)
        {
            var closestSystem = GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!;
            DispatchWaypointUpdate(closestSystem);
        }

        _lastInvalidationType = ratio < 1 ? InvalidationTypeZoomIn : InvalidationTypeRecalculate;

        InvalidateVisual();
    }


    private static readonly Brush BackgroundBrush = new SolidColorBrush(Colors.Black);
    private static List<SystemInformation> _currentSystems = [];
    private static bool _initialized = false;

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(BackgroundBrush, new Rect(0, 0, Width, Height));
        if (!_initialized)
        {
            var text = new FormattedText("Loading Systems .. Please Wait", CultureInfo.CurrentUICulture,
                flowDirection: FlowDirection.LeftToRight, Typeface.Default, 12, WhiteDwarfDefaultBrush);
            context.DrawText(text, new Point(CenterX - text.Width / 2, CenterY - text.Height / 2));
            return;
        }

        var mousePosUniverse = (x: (_lastPosition.X - CenterX) * UniverseUnitsPerPixel + ViewportCenterX,
            y: (_lastPosition.Y - CenterY) * UniverseUnitsPerPixel + ViewportCenterY);

        var formattedText = new FormattedText(
            $"1 Pixel = {UniverseUnitsPerPixel:F4} units, width {ViewportScale:F4} units, drawing {_currentSystems.Count} systems\nLast Mouse Position: X: {mousePosUniverse.x:F4}, Y: {mousePosUniverse.y:F4}\nWaypoint count: {_waypointInformations?.Length ?? 0}",
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight, Typeface.Default, 12, WhiteDwarfDefaultBrush);
        context.DrawText(formattedText, new Point(10, 10));

        var currentRenderBoundsQuery = (sx: ViewportCenterX - ViewportScale / 1.95,
            sy: ViewportCenterY - ViewportScale / 1.95, ex: ViewportCenterX + ViewportScale / 1.95,
            ey: ViewportCenterY + ViewportScale / 1.95);

        if (_lastInvalidationType == InvalidationTypeZoomIn)
        {
            _currentSystems.RemoveAll(x =>
                !x.X.IsBoundedBy(currentRenderBoundsQuery.sx, currentRenderBoundsQuery.ex) ||
                !x.Y.IsBoundedBy(currentRenderBoundsQuery.sy, currentRenderBoundsQuery.ey));
        }
        else if (_lastInvalidationType == InvalidationTypeRecalculate)
        {
            var enumerable = GlobalStates.SystemTree.SearchInBounds(currentRenderBoundsQuery.sx,
                currentRenderBoundsQuery.sy, currentRenderBoundsQuery.ex, currentRenderBoundsQuery.ey);
            _currentSystems = enumerable.ToList();
        }

        // Here is where we might want to redraw stuff the next frame

        foreach (var system in _currentSystems)
        {
            RenderSystem(context, system);
        }

        if (_waypointParentSystemInfo != null)
        {
            foreach (var waypoint in _waypointInformations!)
            {
                RenderWaypointOrbitals(context, waypoint);
            }

            foreach (var waypoint in _waypointInformations!)
            {
                RenderWaypoint(context, waypoint);
            }
        }

        _lastInvalidationType = 0;
    }

    #region System Rendering

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

    // So we want to treat the system astronomical bodies as being "0.5 universe units wide" (i.e. a circle with a radius of 0.25 in the scale of the universe) by default
    // Though some will be rendered bigger or smaller (basically dwarfs at 0.25 universe units wide, hypergiants at 1)
    // But at a far enough zoom, they will just become points
    // And then at a close enough zoom, we will render text under the systems
    public void RenderSystem(DrawingContext context, SystemInformation system)
    {
        var baseSizeInPixels = ToViewportSize(system.Scale / 2);
        var centerPoint = new Point(ToViewportX(system.X), ToViewportY(system.Y));
        switch (system.System.Type)
        {
            case SystemType.NeutronStar:
                DrawSystem(context, NeutronStarDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(NeutronStarDefaultBrush));
                break;
            case SystemType.RedStar:
                DrawSystem(context, RedStarDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(RedStarDefaultBrush));
                break;
            case SystemType.OrangeStar:
                DrawSystem(context, OrangeStarDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(OrangeStarDefaultBrush));
                break;
            case SystemType.BlueStar:
                DrawSystem(context, BlueStarDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(BlueStarDefaultBrush));
                break;
            case SystemType.YoungStar:
                DrawSystem(context, YoungStarDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(YoungStarDefaultBrush));
                break;
            case SystemType.WhiteDwarf:
                DrawSystem(context, WhiteDwarfDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(WhiteDwarfDefaultBrush));
                break;
            case SystemType.BlackHole:
                DrawSystem(context, BlackHoleDefaultBrush, centerPoint, baseSizeInPixels, system, DrawBlackHole);
                break;
            case SystemType.Hypergiant:
                DrawSystem(context, HyperGiantDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(HyperGiantDefaultBrush));
                break;
            case SystemType.Nebula:
                DrawSystem(context, NebulaDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(NebulaBackground, NebulaStar, NebulaBorder));
                break;
            case SystemType.Unstable:
                DrawSystem(context, UnstableDefaultBrush, centerPoint, baseSizeInPixels, system,
                    DrawSpotted<SystemInformation>(UnstableDefaultBrush, border: new SolidColorBrush(Colors.Purple)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public void DrawSystem(DrawingContext context, Brush brush, Point location, double scaledSize,
        SystemInformation system,
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
                drawLarger(context, location, scaledSize, system);
            }
            else
            {
                context.DrawEllipse(brush, null, location, scaledSize / 2, scaledSize / 2);
            }
        }

        // At this point, we need to start drawing text and other decorations
        if (UniverseUnitsPerPixel <= 3)
        {
            DrawSystemDecorations(context, location, scaledSize, system);
        }
    }

    private void DrawSystemDecorations(DrawingContext context, Point location, double scaledSize,
        SystemInformation system)
    {
        var bottom = location.Y + scaledSize * 1.1 / 2;
        var topOfText = bottom + 3;
        double width;
        if (system.SystemNameText is { } systemNameText)
        {
            width = system.SystemNameWidth;
        }
        else
        {
            systemNameText = system.SystemNameText = new FormattedText($"{system.System.Symbol} - {system.System.Name}",
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, Typeface.Default, 14, WhiteDwarfDefaultBrush);
            width = system.SystemNameWidth = systemNameText.Width;
        }

        context.DrawText(systemNameText, new Point(location.X - width / 2, topOfText));
    }

    private static readonly Brush BlackHoleLargerInside = new SolidColorBrush(new Color(255, 16, 16, 16));

    private void DrawBlackHole(DrawingContext context, Point location, double scaledSize, SystemInformation system)
    {
        Pen blackHoleBorder = new(Brushes.OrangeRed, Math.Max(1, scaledSize / 20));
        context.DrawEllipse(BlackHoleLargerInside, blackHoleBorder, location, scaledSize / 2, scaledSize / 2);
    }

    private static readonly Brush NebulaBackground = new SolidColorBrush(new Color(255, 74, 95, 159));
    private static readonly Brush NebulaBorder = new SolidColorBrush(new Color(255, 199, 116, 87));
    private static readonly Brush NebulaStar = new SolidColorBrush(new Color(255, 174, 223, 235));

    // Todo find a more random arrangement of stars
    private static readonly (double x, double y, double relativeSize)[][] SpotLocations =
    [
        ComputeSpotLocations(10), ComputeSpotLocations(10), ComputeSpotLocations(10), ComputeSpotLocations(10),
        ComputeSpotLocations(15), ComputeSpotLocations(15), ComputeSpotLocations(15), ComputeSpotLocations(15),
        ComputeSpotLocations(20), ComputeSpotLocations(20), ComputeSpotLocations(20), ComputeSpotLocations(20),
        ComputeSpotLocations(25), ComputeSpotLocations(25), ComputeSpotLocations(25), ComputeSpotLocations(25),
        ComputeSpotLocations(30), ComputeSpotLocations(30), ComputeSpotLocations(30), ComputeSpotLocations(30),
    ];

    private static (double x, double y, double relativeSize)[] ComputeSpotLocations(int count)
    {
        var result = new List<(double, double, double)>(count);
        var random = new Random();
        while (result.Count < count)
        {
            var theta = 2 * Math.PI * random.NextDouble();
            var r = Math.Sqrt(random.NextDouble());
            var x = r * Math.Cos(theta);
            var y = r * Math.Sin(theta);
            var maxRadius = 1 - r;
            if (maxRadius < 0.02)
            {
                continue;
            }

            var radius = Uniform(0.2, Math.Min(0.02, maxRadius));
            bool collides = false;
            foreach (var star in result)
            {
                var (sx, sy, sr) = star;
                if ((x - sx) * (x - sx) + (y - sy) * (y - sy) <= (radius + sr) * (radius + sr))
                {
                    collides = true;
                    break;
                }
            }

            if (collides) continue;
            result.Add((x, y, radius));
        }

        return result.ToArray();

        double Uniform(double min, double max)
        {
            var distance = max - min;
            var value = random.NextDouble();
            return (value * distance) + min;
        }
    }

    private Action<DrawingContext, Point, double, T> DrawSpotted<T>(Brush backgroundBrush,
        Brush? innerBrush = null, Brush? border = null)
    {
        return Draw;

        void Draw(DrawingContext context, Point location, double scaledSize, T system)
        {
            if (innerBrush == null)
            {
                if (backgroundBrush is not SolidColorBrush brush)
                    throw new ArgumentNullException(nameof(backgroundBrush));
                innerBrush = new SolidColorBrush(new Color(255, (byte)(brush.Color.R * 0.75),
                    (byte)(brush.Color.G * 0.75), (byte)(brush.Color.B * 0.75)));
            }

            if (border == null)
            {
                if (backgroundBrush is not SolidColorBrush brush)
                    throw new ArgumentNullException(nameof(backgroundBrush));
                border = new SolidColorBrush(new Color(255, (byte)Math.Min(255, brush.Color.R * 1.25),
                    (byte)Math.Min(255, brush.Color.G * 1.25), (byte)Math.Min(255, brush.Color.B * 1.25)));
            }

            var borderPen = new Pen(border, Math.Max(1, scaledSize / 20));
            context.DrawEllipse(backgroundBrush, borderPen, location, scaledSize / 2, scaledSize / 2);
            var index = Math.Abs(system!.GetHashCode()) % SpotLocations.Length;
            foreach (var (x, y, diameter) in SpotLocations[index])
            {
                var trueX = x * (scaledSize * 0.9) / 2 + location.X;
                var trueY = y * (scaledSize * 0.9) / 2 + location.Y;
                var trueDiameter = diameter * (scaledSize * 0.9) / 2;
                if (trueDiameter >= 1)
                {
                    context.DrawEllipse(innerBrush, null, new Point(trueX, trueY), trueDiameter / 2, trueDiameter / 2);
                }
            }
        }
    }

    #endregion


    #region Waypoint Rendering

    private static readonly Pen OrbitPen = new(Brushes.Gray, 1, DashStyle.Dash, PenLineCap.Round);


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

    private void RenderWaypoint(DrawingContext context, WaypointInformation waypoint)
    {
        var baseSizeInPixels = ToViewportSize(waypoint.Scale / 5d);
        var actualX = ToViewportX(waypoint.X / 10 + _waypointParentSystemInfo!.X);
        var actualY = ToViewportY(waypoint.Y / 10 + _waypointParentSystemInfo.Y);

        var centerPoint = new Point(actualX, actualY);
        switch (waypoint.Waypoint.Type)
        {
            case WaypointType.Planet:
                DrawWaypoint(context, PlanetBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawSpotted<WaypointInformation>(PlanetBrush, GasGiantBrush));
                break;
            case WaypointType.GasGiant:
                DrawWaypoint(context, GasGiantBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawSpotted<WaypointInformation>(GasGiantBrush));
                break;
            case WaypointType.Moon:
                DrawWaypoint(context, MoonBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawSpotted<WaypointInformation>(MoonBrush));
                break;
            case WaypointType.OrbitalStation:
                DrawWaypoint(context, OrbitalStationBrush, centerPoint, baseSizeInPixels, waypoint);
                break;
            case WaypointType.JumpGate:
                DrawWaypoint(context, JumpGateBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawWell(JumpGateBrush, 5, 1 / 20d));
                break;
            case WaypointType.AsteroidField:
                DrawWaypoint(context, AsteroidFieldBrush, centerPoint, baseSizeInPixels, waypoint);
                break;
            case WaypointType.Asteroid:
                DrawWaypoint(context, AsteroidBrush, centerPoint, baseSizeInPixels, waypoint);
                break;
            case WaypointType.EngineeredAsteroid:
                DrawWaypoint(context, EngineeredAsteroidBrush, centerPoint, baseSizeInPixels, waypoint);
                break;
            case WaypointType.AsteroidBase:
                DrawWaypoint(context, AsteroidBaseBrush, centerPoint, baseSizeInPixels, waypoint);
                break;
            case WaypointType.Nebula:
                DrawWaypoint(context, NebulaDefaultBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawSpotted<WaypointInformation>(NebulaBackground, NebulaBorder, NebulaBorder));
                break;
            case WaypointType.DebrisField:
                DrawWaypoint(context, DebrisFieldBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawSpotted<WaypointInformation>(null!, DebrisFieldBrush, new SolidColorBrush(Colors.Transparent)));
                break;
            case WaypointType.GravityWell:
                DrawWaypoint(context, GravityWellBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawWell(GravityWellBrush, 15, 1 / 40d));
                break;
            case WaypointType.ArtificialGravityWell:
                DrawWaypoint(context, ArtificialGravityWellBrush, centerPoint, baseSizeInPixels, waypoint,
                    DrawWell(ArtificialGravityWellBrush, 15, 1 / 40d));
                break;
            case WaypointType.FuelStation:
                DrawWaypoint(context, FuelStationBrush, centerPoint, baseSizeInPixels, waypoint);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RenderWaypointOrbitals(DrawingContext context, WaypointInformation waypoint)
    {
        var actualX = ToViewportX(waypoint.X / 10 + _waypointParentSystemInfo!.X);
        var actualY = ToViewportY(waypoint.Y / 10 + _waypointParentSystemInfo.Y);
        var orbitCenterX = ToViewportX(_waypointParentSystemInfo!.X);
        var orbitCenterY = ToViewportY(_waypointParentSystemInfo.Y);
        if (waypoint.Waypoint.Orbits is { } orbits &&
            _waypointInformations!.FirstOrDefault(x => x.Waypoint.Symbol == orbits) is { } orbitedWaypoint)
        {
            orbitCenterX = ToViewportX(orbitedWaypoint.X / 10 + _waypointParentSystemInfo.X);
            orbitCenterY = ToViewportY(orbitedWaypoint.Y / 10 + _waypointParentSystemInfo.Y);
        }

        var radius = Math.Sqrt((actualX - orbitCenterX) * (actualX - orbitCenterX) +
                               (actualY - orbitCenterY) * (actualY - orbitCenterY));

        if (radius < 3 * Width)
        {
            context.DrawEllipse(null, OrbitPen, new Point(orbitCenterX, orbitCenterY), radius, radius);
        }
    }

    public void DrawWaypoint(DrawingContext context, Brush brush, Point location, double scaledSize,
        WaypointInformation waypoint,
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
                drawLarger(context, location, scaledSize, waypoint);
            }
            else
            {
                context.DrawEllipse(brush, null, location, scaledSize / 2, scaledSize / 2);
            }
        }

        DrawWaypointDecorations(context, location, scaledSize, waypoint);
    }

    private void DrawWaypointDecorations(DrawingContext context, Point location, double scaledSize,
        WaypointInformation waypoint)
    {
        var bottom = location.Y + scaledSize / 2;
        var topOfText = bottom + 3;
        double width;
        if (waypoint.WaypointNameText is { } waypointNameText)
        {
            width = waypoint.WaypointNameWidth;
        }
        else
        {
            waypointNameText = waypoint.WaypointNameText = new FormattedText($"{waypoint.Waypoint.Symbol}",
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, Typeface.Default, 10, WhiteDwarfDefaultBrush);
            width = waypoint.WaypointNameWidth = waypointNameText.Width;
        }

        context.DrawText(waypointNameText, new Point(location.X - width / 2, topOfText));
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

    #endregion

    #region Utilities

    public double ToUniverseSize(double viewportSize) => viewportSize * UniverseUnitsPerPixel;
    public double ToViewportSize(double universeSize) => universeSize / UniverseUnitsPerPixel;

    public double ToUniverseX(double viewportX) => ((viewportX - CenterX) * UniverseUnitsPerPixel) + ViewportCenterX;
    public double ToUniverseY(double viewportY) => ((viewportY - CenterY) * UniverseUnitsPerPixel) + ViewportCenterY;
    public double ToViewportX(double universeX) => ((universeX - ViewportCenterX) / UniverseUnitsPerPixel) + CenterX;
    public double ToViewportY(double universeY) => ((universeY - ViewportCenterY) / UniverseUnitsPerPixel) + CenterY;

    public SystemInformation? IsOverSystem(double mouseX, double mouseY)
    {
        var universeX = ToUniverseX(mouseX);
        var universeY = ToUniverseY(mouseY);
        return _currentSystems.FirstOrDefault(s =>
            (universeX - s.X) * (universeX - s.X) + (universeY - s.Y) * (universeY - s.Y) <=
            (s.Scale / 4) * (s.Scale / 4));
    }

    public WaypointInformation? IsOverWaypoint(double mouseX, double mouseY)
    {
        var universeX = ToUniverseX(mouseX);
        var universeY = ToUniverseY(mouseY);
        return _waypointInformations?.FirstOrDefault(w =>
        {
            var actualX = w.X / 10 + _waypointParentSystemInfo!.X;
            var actualY = w.Y / 10 + _waypointParentSystemInfo.Y;
            var squareDistance = (universeX - actualX) * (universeX - actualX) +
                                 (universeY - actualY) * (universeY - actualY);
            var radius = w.Scale / 10;
            return squareDistance <= (radius * radius);
        });
    }

    private CancellationTokenSource _source = new();
    private SystemInformation? _lastUpdateSystem;

    private void DispatchWaypointClear()
    {
        _source.Cancel();
        _waypointParentSystemInfo = null;
        _waypointInformations = null;
        _lastUpdateSystem = null;
    }

    public void RefreshWaypoints()
    {
        _waypointCacheGeneration++;
        _waypointInformations = null;
        if (_waypointParentSystemInfo == null) return;
        var parent = _waypointParentSystemInfo;
        _lastUpdateSystem = null;
        _waypointParentSystemInfo = null;
        DispatchWaypointUpdate(parent);
    }

    private void DispatchWaypointUpdate(SystemInformation currentSystemInfo)
    {
        _source.Cancel();
        if (_lastUpdateSystem == currentSystemInfo)
        {
            return;
        }

        if (currentSystemInfo.CachedWaypoints != null &&
            currentSystemInfo.CachedWaypointsGeneration == _waypointCacheGeneration)
        {
            _waypointInformations = currentSystemInfo.CachedWaypoints;
            _waypointParentSystemInfo = currentSystemInfo;
            return;
        }

        _lastUpdateSystem = currentSystemInfo;
        var currentSource = _source = new CancellationTokenSource();
        Dispatcher.UIThread.InvokeAsync(WaypointUpdateMethod, DispatcherPriority.Default, currentSource.Token);

        return;

        async Task WaypointUpdateMethod()
        {
            // Now we need to do the HTTP request
            // var pagedData = await GlobalStates.client.GetAllPaginatedData<Waypoint>()
            _lastUpdateSystem = currentSystemInfo;
            try
            {
                _waypointInformations =
                    (await GlobalStates.client.GetAllPaginatedData<Waypoint>(
                        $"https://api.spacetraders.io/v2/systems/{currentSystemInfo.System.Symbol}/waypoints",
                        cancellationToken: currentSource.Token))
                    .Select(x => new WaypointInformation(x)).ToArray();
            }
            catch (OperationCanceledException)
            {
                // If the operation is cancelled we want to return
                _lastUpdateSystem = null;
                return;
            }

            foreach (var waypoint in _waypointInformations.OrderBy(GetOrbitDepth))
            {
                var childWaypoints = waypoint.Waypoint.Orbitals
                    .Select(x => _waypointInformations.FirstOrDefault(y => y.Waypoint.Symbol == x.Symbol))
                    .Where(x => x != null).ToArray();
                if (childWaypoints.Length <= 0) continue;

                var radiusSpacing = Math.PI * 2 / childWaypoints.Length;
                var radius = waypoint.Scale * 3;
                var theta = 0d;
                foreach (var child in childWaypoints)
                {
                    var offsetX = Math.Cos(theta) * radius;
                    var offsetY = Math.Sin(theta) * radius;
                    child!.Scale = waypoint.Scale / 2;
                    child.OffsetX = offsetX + waypoint.OffsetX;
                    child.OffsetY = offsetY + waypoint.OffsetY;
                    theta += radiusSpacing;
                }
            }

            _waypointParentSystemInfo = currentSystemInfo;
            currentSystemInfo.CachedWaypoints = _waypointInformations;
            currentSystemInfo.CachedWaypointsGeneration = _waypointCacheGeneration;
            _lastInvalidationType = InvalidationTypeRecalculate;
            _lastUpdateSystem = null;
            InvalidateVisual();
            return;

            int GetOrbitDepth(WaypointInformation? waypoint)
            {
                var depth = 0;
                while (waypoint?.Waypoint.Orbits != null)
                {
                    depth += 1;
                    waypoint = _waypointInformations!.FirstOrDefault(x =>
                        x.Waypoint.Symbol == waypoint.Waypoint.Orbits);
                }

                return depth;
            }
        }
    }

    #endregion
}