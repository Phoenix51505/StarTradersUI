namespace StarTradersUI.Utilities;

public static class DoubleExtensions
{
    // Check if value is within [minInclusive, maxExclusive)
    public static bool IsBoundedBy(this double d, double minInclusive, double maxExclusive)
    {
        return d >= minInclusive && d < maxExclusive;
    }
}