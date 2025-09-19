namespace StarTradersUI.Api;
public class Faction
{
    public FactionSymbol Symbol { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Traits[] Traits { get; set; }
    public bool IsRecruiting { get; set; }
    public string Headquarters { get; set; }
}