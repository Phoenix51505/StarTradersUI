namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

/// <summary>
/// Symbol of the frame.
/// </summary>
public enum ShipFrameSymbol {
    [EnumMember(Value = "FRAME_PROBE")]
    FrameProbe,
    [EnumMember(Value = "FRAME_DRONE")]
    FrameDrone,
    [EnumMember(Value = "FRAME_INTERCEPTOR")]
    FrameInterceptor,
    [EnumMember(Value = "FRAME_RACER")]
    FrameRacer,
    [EnumMember(Value = "FRAME_FIGHTER")]
    FrameFighter,
    [EnumMember(Value = "FRAME_FRIGATE")]
    FrameFrigate,
    [EnumMember(Value = "FRAME_SHUTTLE")]
    FrameShuttle,
    [EnumMember(Value = "FRAME_EXPLORER")]
    FrameExplorer,
    [EnumMember(Value = "FRAME_MINER")]
    FrameMiner,
    [EnumMember(Value = "FRAME_LIGHT_FREIGHTER")]
    FrameLightFreighter,
    [EnumMember(Value = "FRAME_HEAVY_FREIGHTER")]
    FrameHeavyFreighter,
    [EnumMember(Value = "FRAME_TRANSPORT")]
    FrameTransport,
    [EnumMember(Value = "FRAME_DESTROYER")]
    FrameDestroyer,
    [EnumMember(Value = "FRAME_CRUISER")]
    FrameCruiser,
    [EnumMember(Value = "FRAME_CARRIER")]
    FrameCarrier,
    [EnumMember(Value = "FRAME_BULK_FREIGHTER")]
    FrameBulkFreighter,
}