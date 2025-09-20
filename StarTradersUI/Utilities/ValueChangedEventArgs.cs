using Avalonia.Interactivity;

namespace StarTradersUI.Utilities;

public class ValueChangedEventArgs<T>(T value) : RoutedEventArgs
{
    public T Value { get; } = value;
}