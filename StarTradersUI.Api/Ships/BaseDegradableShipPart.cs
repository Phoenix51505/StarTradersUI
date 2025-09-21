namespace StarTradersUI.Api.Ships;

public class BaseDegradableShipPart<TSymbol> : BaseShipPart<TSymbol>
{
    public double Condition { get; set; }
    public double Integrity { get; set; }
    public int Quality { get; set; }
}