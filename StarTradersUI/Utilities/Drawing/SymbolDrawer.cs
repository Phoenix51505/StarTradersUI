using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace StarTradersUI.Utilities.Drawing;

public class SymbolDrawer<T>
{
    public List<(Rect bounds, T value)> LastDrawnSymbols = [];
    public void ResetSymbolList() => LastDrawnSymbols.Clear();
    public void DrawSymbolsLarge(DrawingContext context, Point systemCenter, double scaledSize,
        (T value,Bitmap image)[] symbols, bool isOnLeft = false)
    {
        if (symbols.Length == 0) return;
        // We usually want the factions to take up a 1/3rd by 1/3rd area in the upper right corner
        double step;
        int countX = 0;
        double rectSize;
        var origX = isOnLeft ? systemCenter.X - scaledSize / 2 : systemCenter.X + scaledSize / 6;
        var curX = origX;
        var curY = systemCenter.Y - ((scaledSize / 2) * 1.05) - scaledSize / 3;
        switch (symbols.Length)
        {
            case 1:
                step = 0;
                rectSize = scaledSize / 3;
                break;
            case 2:
                step = scaledSize / 6;
                curY += step;
                rectSize = scaledSize / 6;
                break;
            case <= 4:
                step = scaledSize / 6;
                countX = 2;
                rectSize = step * 0.975;
                break;
            case <= 6:
                step = scaledSize / 9;
                countX = 3;
                rectSize = step * 0.975;
                curY += step;
                break;
            case <= 9:
                step = scaledSize / 9;
                countX = 3;
                rectSize = step * 0.975;
                break;
            case <= 12:
                step = scaledSize / 12;
                countX = 4;
                rectSize = step * 0.975;
                curY += step;
                break;
            case <= 16:
                step = scaledSize / 12;
                countX = 4;
                rectSize = step * 0.975;
                break;
            case <= 20:
                step = scaledSize / 15;
                curY += step;
                countX = 5;
                rectSize = step * 0.975;
                break;
            case <= 25:
                step = scaledSize / 15;
                countX = 5;
                rectSize = step * 0.975;
                break;
            case <= 30:
                step = scaledSize / 18;
                curY += step;
                countX = 6;
                rectSize = step * 0.975;
                break;
            case <= 36:
                step = scaledSize / 18;
                countX = 6;
                rectSize = step * 0.975;
                break;
            case <= 42:
                step = scaledSize / 21;
                curY += step;
                countX = 7;
                rectSize = step * 0.975;
                break;
            case <= 49:
                step = scaledSize / 21;
                countX = 7;
                rectSize = step * 0.975;
                break;
            case <= 54:
                step = scaledSize / 24;
                curY += step;
                countX = 8;
                rectSize = step * 0.975;
                break;
            case <= 64:
                step = scaledSize / 24;
                countX = 8;
                rectSize = step * 0.975;
                break;
            case <= 72:
                step = scaledSize / 27;
                curY += step;
                countX = 9;
                rectSize = step * 0.975;
                break;
            default:
                // We assume that for any case where stuff is being drawn like this that the maximum is 81 symbols
                step = scaledSize / 27;
                countX = 9;
                rectSize = step * 0.975;
                break;
        }

        var currentX = 0;
        foreach (var (value, image) in symbols)
        {
            var sourceRect = new Rect(0, 0, image.Size.Width, image.Size.Height);
            var destRect = new Rect(curX, curY, rectSize, rectSize);
            LastDrawnSymbols.Add((destRect, value));
            context.DrawImage(image, sourceRect, destRect);
            currentX += 1;
            if (currentX != countX) continue;
            currentX = 0;
            curX = origX;
            curY += step;
        }
    }
}