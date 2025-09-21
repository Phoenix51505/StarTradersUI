namespace StarTradersUI.Api.Ships;

public class Cooldown
{
    public string ShipSymbol { get; set; }
    public int TotalSeconds { get; set; }
    public int RemainingSeconds { get; set; }
    public DateTime? Expiration { get; set; }
}