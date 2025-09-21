using Avalonia;
using Avalonia.Media;
using StarTradersUI.Controls;

namespace StarTradersUI.Utilities.Interfaces;

public interface ISystemRenderable : IPositionable
{
    public double Scale { get; }
    public void Render(SystemDrawer drawer, DrawingContext context, Point center, double scaledSize);
}