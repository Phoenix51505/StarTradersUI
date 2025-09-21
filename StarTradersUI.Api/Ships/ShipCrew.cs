namespace StarTradersUI.Api.Ships;

public class ShipCrew
{
    public int Current { get; set; }
    public int Required { get; set; }
    public int Capacity { get; set; }
    public ShipCrewRotation Rotation { get; set; }
    public int Morale { get; set; }
    public int Wages { get; set; }
}