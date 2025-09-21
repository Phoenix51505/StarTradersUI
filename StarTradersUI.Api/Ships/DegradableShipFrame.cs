namespace StarTradersUI.Api.Ships;

public class DegradableShipFrame : BaseDegradableShipPart<ShipFrameSymbol>
{
    public int ModuleSlots { get; set; }
    public int MountingPoints { get; set; }
    public int FuelCapacity { get; set; }
}