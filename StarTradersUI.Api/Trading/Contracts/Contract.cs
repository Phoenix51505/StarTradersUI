namespace StarTradersUI.Api.Trading.Contracts;

public class Contract
{
    public string Id { get; set; }
    public string FactionSymbol { get; set; }
    public ContractType ContractType { get; set; }
    public ContractTerms Terms { get; set; }
    public bool Accepted { get; set; }

    [Obsolete("Use DeadlineToAccept instead")]
    public DateTime Expiration { get; set; }

    public DateTime? DeadlineToAccept { get; set; }
}