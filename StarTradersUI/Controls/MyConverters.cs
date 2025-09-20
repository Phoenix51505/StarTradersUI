using Avalonia.Data.Converters;

namespace StarTradersUI.Controls;

public static class MyConverters
{
    public static FuncValueConverter<double?, double?> WindowSizeConverter { get; } = new(v => v * 1.5);
}