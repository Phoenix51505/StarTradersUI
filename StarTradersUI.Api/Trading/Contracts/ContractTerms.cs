namespace StarTradersUI.Api.Trading.Contracts;

public class ContractTerms
{
    public DateTime Deadline { get; set; }
    public ContractPayment Payment { get; set; }
    public ContractDeliveredGood[]? Deliver { get; set; }
    
}