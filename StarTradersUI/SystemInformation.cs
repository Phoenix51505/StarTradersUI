using StarTradersUI.Utilities;

namespace StarTradersUI;

public class SystemInformation(Api.System system) : IPositionable
{
    public Api.System System => system;
    public double X => system.X;
    public double Y => system.Y;

    public static implicit operator SystemInformation(Api.System system) => new(system);
}