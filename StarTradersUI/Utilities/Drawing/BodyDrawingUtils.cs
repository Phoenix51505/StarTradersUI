using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;

namespace StarTradersUI.Utilities.Drawing;

public static class BodyDrawingUtils
{
    
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

    public static Action<DrawingContext, Point, double, T> DrawSpotted<T>(Brush backgroundBrush,
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
            context.DrawEllipse(backgroundBrush, borderPen, location, scaledSize / 2 - scaledSize / 20, scaledSize / 2 - scaledSize / 20);
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
}