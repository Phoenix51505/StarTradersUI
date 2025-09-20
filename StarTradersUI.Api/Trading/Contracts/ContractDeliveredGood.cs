namespace StarTradersUI.Api.Trading.Contracts;

public class ContractDeliveredGood
{
    public TradeSymbol TradeSymbol { get; set; }
    public string DestinationSymbol { get; set; }
    public int UnitsRequired { get; set; }
    public int UnitsFulfilled { get; set; }
}