using System;

namespace StarTradersUI.Utilities;

public interface IPositionable
{
    public double X { get; }
    public double Y { get; }

}

public static class PositionableExtensions
{
    public static double DistanceTo(this IPositionable cur, IPositionable other)
    {
        return cur.DistanceTo(other.X,other.Y);
    }

    public static double DistanceTo(this IPositionable cur, double x, double y)
    {
        return Math.Sqrt(Math.Pow(x - cur.X, 2) + Math.Pow(y - cur.Y, 2));
    }
}