namespace StarTradersUI.Api.Trading;

using System.Runtime.Serialization;

/// <summary>
/// The good's symbol.
/// </summary>
public enum TradeSymbol {
    [EnumMember(Value = "PRECIOUS_STONES")]
    PreciousStones,
    [EnumMember(Value = "QUARTZ_SAND")]
    QuartzSand,
    [EnumMember(Value = "SILICON_CRYSTALS")]
    SiliconCrystals,
    [EnumMember(Value = "AMMONIA_ICE")]
    AmmoniaIce,
    [EnumMember(Value = "LIQUID_HYDROGEN")]
    LiquidHydrogen,
    [EnumMember(Value = "LIQUID_NITROGEN")]
    LiquidNitrogen,
    [EnumMember(Value = "ICE_WATER")]
    IceWater,
    [EnumMember(Value = "EXOTIC_MATTER")]
    ExoticMatter,
    [EnumMember(Value = "ADVANCED_CIRCUITRY")]
    AdvancedCircuitry,
    [EnumMember(Value = "GRAVITON_EMITTERS")]
    GravitonEmitters,
    [EnumMember(Value = "IRON")]
    Iron,
    [EnumMember(Value = "IRON_ORE")]
    IronOre,
    [EnumMember(Value = "COPPER")]
    Copper,
    [EnumMember(Value = "COPPER_ORE")]
    CopperOre,
    [EnumMember(Value = "ALUMINUM")]
    Aluminum,
    [EnumMember(Value = "ALUMINUM_ORE")]
    AluminumOre,
    [EnumMember(Value = "SILVER")]
    Silver,
    [EnumMember(Value = "SILVER_ORE")]
    SilverOre,
    [EnumMember(Value = "GOLD")]
    Gold,
    [EnumMember(Value = "GOLD_ORE")]
    GoldOre,
    [EnumMember(Value = "PLATINUM")]
    Platinum,
    [EnumMember(Value = "PLATINUM_ORE")]
    PlatinumOre,
    [EnumMember(Value = "DIAMONDS")]
    Diamonds,
    [EnumMember(Value = "URANITE")]
    Uranite,
    [EnumMember(Value = "URANITE_ORE")]
    UraniteOre,
    [EnumMember(Value = "MERITIUM")]
    Meritium,
    [EnumMember(Value = "MERITIUM_ORE")]
    MeritiumOre,
    [EnumMember(Value = "HYDROCARBON")]
    Hydrocarbon,
    [EnumMember(Value = "ANTIMATTER")]
    Antimatter,
    [EnumMember(Value = "FAB_MATS")]
    FabMats,
    [EnumMember(Value = "FERTILIZERS")]
    Fertilizers,
    [EnumMember(Value = "FABRICS")]
    Fabrics,
    [EnumMember(Value = "FOOD")]
    Food,
    [EnumMember(Value = "JEWELRY")]
    Jewelry,
    [EnumMember(Value = "MACHINERY")]
    Machinery,
    [EnumMember(Value = "FIREARMS")]
    Firearms,
    [EnumMember(Value = "ASSAULT_RIFLES")]
    AssaultRifles,
    [EnumMember(Value = "MILITARY_EQUIPMENT")]
    MilitaryEquipment,
    [EnumMember(Value = "EXPLOSIVES")]
    Explosives,
    [EnumMember(Value = "LAB_INSTRUMENTS")]
    LabInstruments,
    [EnumMember(Value = "AMMUNITION")]
    Ammunition,
    [EnumMember(Value = "ELECTRONICS")]
    Electronics,
    [EnumMember(Value = "SHIP_PLATING")]
    ShipPlating,
    [EnumMember(Value = "SHIP_PARTS")]
    ShipParts,
    [EnumMember(Value = "EQUIPMENT")]
    Equipment,
    [EnumMember(Value = "FUEL")]
    Fuel,
    [EnumMember(Value = "MEDICINE")]
    Medicine,
    [EnumMember(Value = "DRUGS")]
    Drugs,
    [EnumMember(Value = "CLOTHING")]
    Clothing,
    [EnumMember(Value = "MICROPROCESSORS")]
    Microprocessors,
    [EnumMember(Value = "PLASTICS")]
    Plastics,
    [EnumMember(Value = "POLYNUCLEOTIDES")]
    Polynucleotides,
    [EnumMember(Value = "BIOCOMPOSITES")]
    Biocomposites,
    [EnumMember(Value = "QUANTUM_STABILIZERS")]
    QuantumStabilizers,
    [EnumMember(Value = "NANOBOTS")]
    Nanobots,
    [EnumMember(Value = "AI_MAINFRAMES")]
    AiMainframes,
    [EnumMember(Value = "QUANTUM_DRIVES")]
    QuantumDrives,
    [EnumMember(Value = "ROBOTIC_DRONES")]
    RoboticDrones,
    [EnumMember(Value = "CYBER_IMPLANTS")]
    CyberImplants,
    [EnumMember(Value = "GENE_THERAPEUTICS")]
    GeneTherapeutics,
    [EnumMember(Value = "NEURAL_CHIPS")]
    NeuralChips,
    [EnumMember(Value = "MOOD_REGULATORS")]
    MoodRegulators,
    [EnumMember(Value = "VIRAL_AGENTS")]
    ViralAgents,
    [EnumMember(Value = "MICRO_FUSION_GENERATORS")]
    MicroFusionGenerators,
    [EnumMember(Value = "SUPERGRAINS")]
    Supergrains,
    [EnumMember(Value = "LASER_RIFLES")]
    LaserRifles,
    [EnumMember(Value = "HOLOGRAPHICS")]
    Holographics,
    [EnumMember(Value = "SHIP_SALVAGE")]
    ShipSalvage,
    [EnumMember(Value = "RELIC_TECH")]
    RelicTech,
    [EnumMember(Value = "NOVEL_LIFEFORMS")]
    NovelLifeforms,
    [EnumMember(Value = "BOTANICAL_SPECIMENS")]
    BotanicalSpecimens,
    [EnumMember(Value = "CULTURAL_ARTIFACTS")]
    CulturalArtifacts,
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
    [EnumMember(Value = "REACTOR_SOLAR_I")]
    ReactorSolarI,
    [EnumMember(Value = "REACTOR_FUSION_I")]
    ReactorFusionI,
    [EnumMember(Value = "REACTOR_FISSION_I")]
    ReactorFissionI,
    [EnumMember(Value = "REACTOR_CHEMICAL_I")]
    ReactorChemicalI,
    [EnumMember(Value = "REACTOR_ANTIMATTER_I")]
    ReactorAntimatterI,
    [EnumMember(Value = "ENGINE_IMPULSE_DRIVE_I")]
    EngineImpulseDriveI,
    [EnumMember(Value = "ENGINE_ION_DRIVE_I")]
    EngineIonDriveI,
    [EnumMember(Value = "ENGINE_ION_DRIVE_II")]
    EngineIonDriveIi,
    [EnumMember(Value = "ENGINE_HYPER_DRIVE_I")]
    EngineHyperDriveI,
    [EnumMember(Value = "MODULE_MINERAL_PROCESSOR_I")]
    ModuleMineralProcessorI,
    [EnumMember(Value = "MODULE_GAS_PROCESSOR_I")]
    ModuleGasProcessorI,
    [EnumMember(Value = "MODULE_CARGO_HOLD_I")]
    ModuleCargoHoldI,
    [EnumMember(Value = "MODULE_CARGO_HOLD_II")]
    ModuleCargoHoldIi,
    [EnumMember(Value = "MODULE_CARGO_HOLD_III")]
    ModuleCargoHoldIii,
    [EnumMember(Value = "MODULE_CREW_QUARTERS_I")]
    ModuleCrewQuartersI,
    [EnumMember(Value = "MODULE_ENVOY_QUARTERS_I")]
    ModuleEnvoyQuartersI,
    [EnumMember(Value = "MODULE_PASSENGER_CABIN_I")]
    ModulePassengerCabinI,
    [EnumMember(Value = "MODULE_MICRO_REFINERY_I")]
    ModuleMicroRefineryI,
    [EnumMember(Value = "MODULE_SCIENCE_LAB_I")]
    ModuleScienceLabI,
    [EnumMember(Value = "MODULE_JUMP_DRIVE_I")]
    ModuleJumpDriveI,
    [EnumMember(Value = "MODULE_JUMP_DRIVE_II")]
    ModuleJumpDriveIi,
    [EnumMember(Value = "MODULE_JUMP_DRIVE_III")]
    ModuleJumpDriveIii,
    [EnumMember(Value = "MODULE_WARP_DRIVE_I")]
    ModuleWarpDriveI,
    [EnumMember(Value = "MODULE_WARP_DRIVE_II")]
    ModuleWarpDriveIi,
    [EnumMember(Value = "MODULE_WARP_DRIVE_III")]
    ModuleWarpDriveIii,
    [EnumMember(Value = "MODULE_SHIELD_GENERATOR_I")]
    ModuleShieldGeneratorI,
    [EnumMember(Value = "MODULE_SHIELD_GENERATOR_II")]
    ModuleShieldGeneratorIi,
    [EnumMember(Value = "MODULE_ORE_REFINERY_I")]
    ModuleOreRefineryI,
    [EnumMember(Value = "MODULE_FUEL_REFINERY_I")]
    ModuleFuelRefineryI,
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
    [EnumMember(Value = "SHIP_PROBE")]
    ShipProbe,
    [EnumMember(Value = "SHIP_MINING_DRONE")]
    ShipMiningDrone,
    [EnumMember(Value = "SHIP_SIPHON_DRONE")]
    ShipSiphonDrone,
    [EnumMember(Value = "SHIP_INTERCEPTOR")]
    ShipInterceptor,
    [EnumMember(Value = "SHIP_LIGHT_HAULER")]
    ShipLightHauler,
    [EnumMember(Value = "SHIP_COMMAND_FRIGATE")]
    ShipCommandFrigate,
    [EnumMember(Value = "SHIP_EXPLORER")]
    ShipExplorer,
    [EnumMember(Value = "SHIP_HEAVY_FREIGHTER")]
    ShipHeavyFreighter,
    [EnumMember(Value = "SHIP_LIGHT_SHUTTLE")]
    ShipLightShuttle,
    [EnumMember(Value = "SHIP_ORE_HOUND")]
    ShipOreHound,
    [EnumMember(Value = "SHIP_REFINING_FREIGHTER")]
    ShipRefiningFreighter,
    [EnumMember(Value = "SHIP_SURVEYOR")]
    ShipSurveyor,
    [EnumMember(Value = "SHIP_BULK_FREIGHTER")]
    ShipBulkFreighter,
}