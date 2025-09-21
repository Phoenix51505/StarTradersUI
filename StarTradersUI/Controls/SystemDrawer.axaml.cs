using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using StarTradersUI.Api;
using StarTradersUI.Api.FactionInfo;
using StarTradersUI.Api.SystemInfo;
using StarTradersUI.Api.WaypointInfo;
using StarTradersUI.Utilities;
using StarTradersUI.Utilities.Drawing;
using StarTradersUI.Utilities.Interfaces;

namespace StarTradersUI.Controls;

public partial class SystemDrawer : UserControl
{
    #region Events And Properties

    public static readonly RoutedEvent<ValueChangedEventArgs<SystemInformation>> SystemSelectedEvent =
        RoutedEvent.Register<SystemDrawer, ValueChangedEventArgs<SystemInformation>>(nameof(SystemSelected),
            RoutingStrategies.Direct);

    public event EventHandler<ValueChangedEventArgs<SystemInformation>> SystemSelected
    {
        add => AddHandler(SystemSelectedEvent, value);
        remove => RemoveHandler(SystemSelectedEvent, value);
    }

    public static readonly RoutedEvent<ValueChangedEventArgs<WaypointInformation>> WaypointSelectedEvent =
        RoutedEvent.Register<SystemDrawer, ValueChangedEventArgs<WaypointInformation>>(nameof(WaypointSelected),
            RoutingStrategies.Direct);

    public event EventHandler<ValueChangedEventArgs<WaypointInformation>> WaypointSelected
    {
        add => AddHandler(WaypointSelectedEvent, value);
        remove => RemoveHandler(WaypointSelectedEvent, value);
    }

    #endregion

    private static Cursor _defaultCursor = Cursor.Default;
    private static Cursor _handCursor = new Cursor(StandardCursorType.Hand);

    // This is meant to draw the entire system, including waypoints when zoomed in far enough


    public double ViewportCenterX = 201; // This will change
    public double ViewportCenterY = 494; // This will change
    public double ViewportScale = 15; // MaxScale / 2; // This is the width of the Viewport in universe units
    public bool IsDragging;

    private const double SystemViewLoadUpp = 0.5; // 100 pixels wide is how big we want to load in a system
    private const double SystemViewUnloadUpp = 0.75;

    private const double MaxScale = 64000;
    private const double MinScale = 0.1;


    private int _lastInvalidationType = 0;
    private const int InvalidationTypeZoomIn = 1;
    private const int InvalidationTypeRecalculate = 2;

    private double ActualWidth => Bounds.Width;
    private double ActualHeight => Bounds.Height;

    private double CenterX => ActualWidth / 2;
    private double CenterY => ActualHeight / 2;

    internal double UniverseUnitsPerPixel => ViewportScale / ActualWidth;


    // Since we are going to draw waypoints at 1/10th the scale 

    public SystemDrawer()
    {
        InitializeComponent();
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.None);
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
    private DateTime _dragStart;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Cursor = _handCursor;
        var point = e.GetCurrentPoint(this);
        _lastPosition = e.GetPosition(this);
        if (!point.Properties.IsLeftButtonPressed &&
            (GetSystemAt(_lastPosition.X, _lastPosition.Y) is not null ||
             GetWaypointAt(_lastPosition.X, _lastPosition.Y) is not null)) return;
        IsDragging = true;
        _dragStart = DateTime.Now;
    }

    public WaypointInformation? LastSelectedWaypoint = null;
    public SystemInformation? LastSelectedSystem = null;


    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        Cursor = null;
        IsDragging = false;
        var currentPosition = e.GetPosition(this);
        var dragEnd = DateTime.Now;
        var span = dragEnd - _dragStart;

        if (e.InitialPressMouseButton == MouseButton.Right)
        {
            if (GetSystemAt(currentPosition.X, currentPosition.Y) is { } system &&
                Resources["System Context Menu"] is ContextMenu menu)
            {
                LastSelectedSystem = system;
                menu.Open(this);
                e.Handled = true;
            }
            else if (GetWaypointAt(currentPosition.X, currentPosition.Y) is { } waypoint &&
                     Resources["Waypoint Context Menu"] is ContextMenu menu2)
            {
                LastSelectedWaypoint = waypoint;
                menu2.Open(this);
                e.Handled = true;
            }
        }
        else
        {
            if (span >= TimeSpan.FromMilliseconds(250)) return;

            if (GetSystemAt(currentPosition.X, currentPosition.Y) is { } system)
            {
                Console.WriteLine($"Selected system: {system.System.Name}");
                var args = new ValueChangedEventArgs<SystemInformation>(system)
                {
                    RoutedEvent = SystemSelectedEvent
                };
                RaiseEvent(args);
            }
            else if (GetWaypointAt(currentPosition.X, currentPosition.Y) is { } waypoint)
            {
                Console.WriteLine($"Selected waypoint: {waypoint.Waypoint.Symbol}");
                var args = new ValueChangedEventArgs<WaypointInformation>(waypoint)
                {
                    RoutedEvent = WaypointSelectedEvent
                };
                RaiseEvent(args);
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var currentPosition = e.GetPosition(this);
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
                    var distanceToOther = closestSystem.DistanceTo(ViewportCenterX, ViewportCenterY);
                    var distanceToCurrent = _waypointParentSystemInfo.DistanceTo(ViewportCenterX, ViewportCenterY);
                    if (distanceToCurrent > distanceToOther * 0.33)
                    {
                        DispatchWaypointUpdate(closestSystem);
                    }
                }
            }
            else if (_initialized && UniverseUnitsPerPixel <= SystemViewLoadUpp)
            {
                var closestSystem = GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!;
                DispatchWaypointUpdate(closestSystem);
            }

            InvalidateVisual();
        }
        else
        {
            if (GetHoveredTrait(currentPosition.X, currentPosition.Y) is { } trait)
            {
                SetAndShowToolTip($"{trait.Name}\n{trait.Description}");
            }
            else if (GetHoveredFactionSymbol(currentPosition.X, currentPosition.Y) is { } factionSymbol)
            {
                var fact = GlobalStates.Factions.FirstOrDefault(x => x.Symbol == factionSymbol);
                SetAndShowToolTip($"{fact?.Name ?? "?"}\n{fact?.Description}");
            }
            else
            {
                HideToolTip();
            }

            if (GetSystemAt(currentPosition.X, currentPosition.Y) != null ||
                GetWaypointAt(currentPosition.X, currentPosition.Y) != null)
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

    private string? _currentToolTipText = null;

    private void SetAndShowToolTip(string text)
    {
        if (_currentToolTipText != text)
        {
            ToolTip.SetTip(this, text);
            // Manually open the tooltip with a short delay
            ToolTip.SetIsOpen(this, true);
            _currentToolTipText = text;
        }
    }

    private void HideToolTip()
    {
        if (_currentToolTipText != null)
        {
            ToolTip.SetIsOpen(this, false);
            _currentToolTipText = null;
        }
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


        if (_waypointParentSystemInfo != null)
        {
            var closestSystem = GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!;
            if (!ReferenceEquals(closestSystem, _waypointParentSystemInfo))
            {
                var distanceToOther = closestSystem.DistanceTo(ViewportCenterX, ViewportCenterY);
                var distanceToCurrent = _waypointParentSystemInfo.DistanceTo(ViewportCenterX, ViewportCenterY);
                if (distanceToCurrent > distanceToOther * 0.33)
                {
                    DispatchWaypointUpdate(closestSystem);
                }
            }
        }

        if (newUnitsPerPixel < SystemViewLoadUpp && _initialized && _waypointParentSystemInfo == null)
        {
            var closestSystem = GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!;
            DispatchWaypointUpdate(closestSystem);
        }

        _lastInvalidationType = Math.Max(_lastInvalidationType,
            ratio < 1 ? InvalidationTypeZoomIn : InvalidationTypeRecalculate);

        InvalidateVisual();
    }


    private static readonly Brush BackgroundBrush = new SolidColorBrush(Colors.Black);
    private static List<SystemInformation> _currentSystems = [];
    private static bool _initialized = false;

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(BackgroundBrush, new Rect(0, 0, ActualWidth, ActualHeight));
        FactionDrawer.ResetSymbolList();
        WaypointDrawer.ResetSymbolList();
        if (!_initialized)
        {
            var text = new FormattedText("Loading Systems .. Please Wait", CultureInfo.CurrentUICulture,
                flowDirection: FlowDirection.LeftToRight, Typeface.Default, 12, Brushes.White);
            context.DrawText(text, new Point(CenterX - text.Width / 2, CenterY - text.Height / 2));
            return;
        }

        var mousePosUniverse = (x: (_lastPosition.X - CenterX) * UniverseUnitsPerPixel + ViewportCenterX,
            y: (_lastPosition.Y - CenterY) * UniverseUnitsPerPixel + ViewportCenterY);

        var formattedText = new FormattedText(
            $"1 Pixel = {UniverseUnitsPerPixel:F4} units, width {ViewportScale:F4} units, drawing {_currentSystems.Count} systems\nLast Mouse Position: X: {mousePosUniverse.x:F4}, Y: {mousePosUniverse.y:F4}\nWaypoint count: {_waypointInformations?.Length ?? 0}",
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight, Typeface.Default, 12, Brushes.White);
        context.DrawText(formattedText, new Point(10, 10));

        var currentRenderBoundsQuery = (sx: ViewportCenterX - ViewportScale / 2 - 4,
            sy: ViewportCenterY - ViewportScale / 2 - 4, ex: ViewportCenterX + ViewportScale / 2 + 4,
            ey: ViewportCenterY + ViewportScale / 2 + 4);

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

        if (UniverseUnitsPerPixel <= SystemViewLoadUpp && _waypointParentSystemInfo == null)
        {
            DispatchWaypointUpdate(GlobalStates.SystemTree.ClosestTo(ViewportCenterX, ViewportCenterY)!);
        }
        // Here is where we might want to redraw stuff the next frame

        foreach (var system in _currentSystems)
        {
            RenderSystem(context, system);
        }

        if (_waypointParentSystemInfo != null)
        {
            foreach (var waypoint in _waypointInformations!.Where(x =>
                         x.Waypoint.Type is not WaypointType.Asteroid and not WaypointType.AsteroidBase
                             and not WaypointType.EngineeredAsteroid))
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

    // So we want to treat the system astronomical bodies as being "0.5 universe units wide" (i.e. a circle with a radius of 0.25 in the scale of the universe) by default
    // Though some will be rendered bigger or smaller (basically dwarfs at 0.25 universe units wide, hypergiants at 1)
    // But at a far enough zoom, they will just become points
    // And then at a close enough zoom, we will render text under the systems
    private void RenderSystem(DrawingContext context, SystemInformation system)
    {
        var baseSizeInPixels = ToViewportSize(system.Scale / 2);
        var centerPoint = new Point(ToViewportX(system.X), ToViewportY(system.Y));
        system.Render(this, context, centerPoint, baseSizeInPixels);
    }

    #endregion


    #region Waypoint Rendering

    private static readonly Pen OrbitPen = new(Brushes.Gray, 1, DashStyle.Dash, PenLineCap.Round);

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

        if (radius < 3 * ActualWidth)
        {
            context.DrawEllipse(null, OrbitPen, new Point(orbitCenterX, orbitCenterY), radius, radius);
        }
    }

    private void RenderWaypoint(DrawingContext context, WaypointInformation waypoint)
    {
        var baseSizeInPixels = ToViewportSize(waypoint.Scale / 5d);
        var actualX = ToViewportX(waypoint.X / 10 + _waypointParentSystemInfo!.X);
        var actualY = ToViewportY(waypoint.Y / 10 + _waypointParentSystemInfo.Y);
        var centerPoint = new Point(actualX, actualY);
        waypoint.Render(this, context, centerPoint, baseSizeInPixels);
    }

    #endregion

    #region Utilities

    public double ToUniverseSize(double viewportSize) => viewportSize * UniverseUnitsPerPixel;
    public double ToViewportSize(double universeSize) => universeSize / UniverseUnitsPerPixel;

    public double ToUniverseX(double viewportX) => ((viewportX - CenterX) * UniverseUnitsPerPixel) + ViewportCenterX;
    public double ToUniverseY(double viewportY) => ((viewportY - CenterY) * UniverseUnitsPerPixel) + ViewportCenterY;
    public double ToViewportX(double universeX) => ((universeX - ViewportCenterX) / UniverseUnitsPerPixel) + CenterX;
    public double ToViewportY(double universeY) => ((universeY - ViewportCenterY) / UniverseUnitsPerPixel) + CenterY;

    public SystemInformation? GetSystemAt(double mouseX, double mouseY)
    {
        var universeX = ToUniverseX(mouseX);
        var universeY = ToUniverseY(mouseY);
        return _currentSystems.FirstOrDefault(s =>
        {
            var dx = Math.Abs(universeX - s.X);
            var dy = Math.Abs(universeY - s.Y);
            return dx <= s.Scale / 4 && dy <= s.Scale / 4;
        });
    }

    public WaypointInformation? GetWaypointAt(double mouseX, double mouseY)
    {
        var universeX = ToUniverseX(mouseX);
        var universeY = ToUniverseY(mouseY);
        return _waypointInformations?.FirstOrDefault(w =>
        {
            var actualX = w.X / 10 + _waypointParentSystemInfo!.X;
            var actualY = w.Y / 10 + _waypointParentSystemInfo.Y;
            // var squareDistance = (universeX - actualX) * (universeX - actualX) +
            //                      (universeY - actualY) * (universeY - actualY);
            var dx = Math.Abs(universeX - actualX);
            var dy = Math.Abs(universeY - actualY);
            var radius = w.Scale / 10;
            return dx <= radius && dy <= radius;
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
        _waypointInformations = null;
        if (_waypointParentSystemInfo == null) return;
        GlobalStates.GlobalDataCache.Remove($"{_waypointParentSystemInfo.System.Symbol}/Waypoints");

        var parent = _waypointParentSystemInfo;
        _lastUpdateSystem = null;
        _waypointParentSystemInfo = null;
        DispatchWaypointUpdate(parent);
    }

    private void DispatchWaypointUpdate(SystemInformation currentSystemInfo)
    {
        if (_lastUpdateSystem == currentSystemInfo)
        {
            return;
        }

        _source.Cancel();

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
                        cacheKey: $"{currentSystemInfo.System.Symbol}/Waypoints", authToken: GlobalStates.authorization,
                        cancellationToken:
                        currentSource.Token))
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
            // currentSystemInfo.CachedWaypoints = _waypointInformations;
            // currentSystemInfo.CachedWaypointsGeneration = _waypointCacheGeneration;
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

    public readonly SymbolDrawer<WaypointTrait> WaypointDrawer = new();
    public readonly SymbolDrawer<FactionSymbol> FactionDrawer = new();

    private WaypointTrait? GetHoveredTrait(double mouseX, double mouseY)
    {
        foreach (var (rect, obj) in WaypointDrawer.LastDrawnSymbols)
        {
            if (rect.Contains(new Point(mouseX, mouseY)))
            {
                return obj;
            }
        }

        return null;
    }

    private FactionSymbol? GetHoveredFactionSymbol(double mouseX, double mouseY)
    {
        foreach (var (rect, obj) in FactionDrawer.LastDrawnSymbols)
        {
            if (rect.Contains(new Point(mouseX, mouseY)))
            {
                return obj;
            }
        }

        return null;
    }

    #endregion
}