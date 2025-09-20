namespace StarTradersUI.Api.WaypointInfo.WaypointConstruction;

public class Construction
{
    public string Symbol { get; set; }
    public ConstructionMaterial[] Materials { get; set; }
    public bool IsComplete { get; set; }
}