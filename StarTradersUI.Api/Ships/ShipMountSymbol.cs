namespace StarTradersUI.Api.Ships;
using System.Runtime.Serialization;

public enum ShipMountSymbol {
    [EnumMember(Value = "MOUNT_GAS_SIPHON_I")]
    MountGasSiphonI,
    [EnumMember(Value = "MOUNT_GAS_SIPHON_II")]
    MountGasSiphonIi,
    [EnumMember(Value = "MOUNT_GAS_SIPHON_III")]
    MountGasSiphonIii,
    [EnumMember(Value = "MOUNT_SURVEYOR_I")]
    MountSurveyorI,
    [EnumMember(Value = "MOUNT_SURVEYOR_II")]
    MountSurveyorIi,
    [EnumMember(Value = "MOUNT_SURVEYOR_III")]
    MountSurveyorIii,
    [EnumMember(Value = "MOUNT_SENSOR_ARRAY_I")]
    MountSensorArrayI,
    [EnumMember(Value = "MOUNT_SENSOR_ARRAY_II")]
    MountSensorArrayIi,
    [EnumMember(Value = "MOUNT_SENSOR_ARRAY_III")]
    MountSensorArrayIii,
    [EnumMember(Value = "MOUNT_MINING_LASER_I")]
    MountMiningLaserI,
    [EnumMember(Value = "MOUNT_MINING_LASER_II")]
    MountMiningLaserIi,
    [EnumMember(Value = "MOUNT_MINING_LASER_III")]
    MountMiningLaserIii,
    [EnumMember(Value = "MOUNT_LASER_CANNON_I")]
    MountLaserCannonI,
    [EnumMember(Value = "MOUNT_MISSILE_LAUNCHER_I")]
    MountMissileLauncherI,
    [EnumMember(Value = "MOUNT_TURRET_I")]
    MountTurretI,
}