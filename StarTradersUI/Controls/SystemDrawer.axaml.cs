using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using StarTradersUI.Api;
using StarTradersUI.Utilities;

namespace StarTradersUI.Controls;

public partial class SystemDrawer : UserControl
{
    private static Cursor _defaultCursor = Cursor.Default;
    private static Cursor _handCursor = new Cursor(StandardCursorType.Hand);

    // This is meant to draw the entire system, including waypoints when zoomed in far enough


    public double ViewportCenterX;
    public double ViewportCenterY;
    public double ViewportScale = MaxScale/2; // This is the width of the Viewport in universe units
    public bool IsDragging;

    private const double SystemViewLoadScale = 16; // 100 pixels wide is how big we want to load in a system
    private const double SystemViewUnloadScale = 24;

    private const double MaxScale = 64000;
    private const double MinScale = 1;


    private int _lastInvalidationType = 0;
    private const int InvalidationTypeZoomIn = 1;
    private const int InvalidationTypeRecalculate = 2;

    private double CenterX => Width / 2;
    private double CenterY => Height / 2;

    private double UniverseUnitsPerPixel => ViewportScale / Width;


    // Since we are going to draw waypoints at 1/10th the scale 

    public SystemDrawer()
    {
        InitializeComponent();
        GlobalStates.DoWhenInitialized(() =>
        {
            _initialized = true;
            _lastInvalidationType = InvalidationTypeRecalculate;
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
            InvalidateVisual();
        }

        _lastPosition = currentPosition;
    }

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
        ratio = newScale/ViewportScale;
        ViewportCenterX += globalZoomPositionRelativeToCenter.x * (1-ratio);
        ViewportCenterY += globalZoomPositionRelativeToCenter.y * (1-ratio);
        ViewportScale = newScale;
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
            var text = new FormattedText("Loading Systems .. Please Wait", CultureInfo.CurrentCulture,
                flowDirection: FlowDirection.LeftToRight, Typeface.Default, 12, WhiteDwarfDefaultBrush);
            context.DrawText(text, new Point(CenterX - text.Width / 2, CenterY - text.Height / 2));
            return;
        }

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

        foreach (var system in _currentSystems)
        {
            RenderSystem(context, system);
        }
        _lastInvalidationType = 0;
    }


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

    // So we want to treat the system astronomical bodies as being "1 universe unit wide" (i.e. a circle with a radius of 0.5 in the scale of the universe) by default
    // Though some will be rendered bigger or smaller (basically dwarfs at 0.5 universe units wide, hypergiants at 2)
    // But at a far enough zoom, they will just become points
    // And then at a close enough zoom, we will render text under the systems
    public void RenderSystem(DrawingContext context, SystemInformation system)
    {
        var baseSizeInPixels = 1 / UniverseUnitsPerPixel;
        var offsetFromCenter = (x: system.X - ViewportCenterX, y: system.Y - ViewportCenterY);
        var offsetFromCenterPixels = (x: offsetFromCenter.x / UniverseUnitsPerPixel,
            y: offsetFromCenter.y / UniverseUnitsPerPixel);
        var centerPoint = new Point(CenterX + offsetFromCenterPixels.x, CenterY + offsetFromCenterPixels.y);
        switch (system.System.Type)
        {
            case SystemType.NeutronStar:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(NeutronStarDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(NeutronStarDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.RedStar:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(RedStarDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(RedStarDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.OrangeStar:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(OrangeStarDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(OrangeStarDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.BlueStar:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(BlueStarDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(BlueStarDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.YoungStar:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(YoungStarDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(YoungStarDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.WhiteDwarf:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(WhiteDwarfDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(WhiteDwarfDefaultBrush, null, centerPoint, baseSizeInPixels / 4,
                        baseSizeInPixels / 4);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.BlackHole:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(BlackHoleDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(BlackHoleDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.Hypergiant:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(HyperGiantDefaultBrush, null, centerPoint, 1, 1);
                }
                else
                {
                    context.DrawEllipse(HyperGiantDefaultBrush, null, centerPoint, baseSizeInPixels,
                        baseSizeInPixels);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.Nebula:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(NebulaDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(NebulaDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            case SystemType.Unstable:
                if (baseSizeInPixels <= 1)
                {
                    context.DrawEllipse(UnstableDefaultBrush, null, centerPoint, 0.5, 0.5);
                }
                else
                {
                    context.DrawEllipse(UnstableDefaultBrush, null, centerPoint, baseSizeInPixels / 2,
                        baseSizeInPixels / 2);
                    // Here we may want to draw some more information but we won't for now
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}