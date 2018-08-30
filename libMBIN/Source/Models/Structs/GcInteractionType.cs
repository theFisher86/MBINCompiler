﻿namespace libMBIN.Models.Structs
{
    public class GcInteractionType : NMSTemplate
    {
		public enum InteractionTypeEnum { None, Shop, NPC, NPC_Secondary, NPC_Anomaly, NPC_Anomaly_Secondary, Ship, Outpost, SpaceStation, RadioTower, Monolith,
            Factory, AbandonedShip, Harvester, Observatory, TradingPost, DistressBeacon, Portal, Plaque, AtlasStation, AbandonedBuildings,WeaponTerminal,
            SuitTerminal, SignalScanner, Teleporter_Base, Teleporter_Station, ClaimBase, NPC_Freighter_Captain, NPC_HIRE_Weapons,NPC_HIRE_Weapons_Wait,
            NPC_HIRE_Farmer, NPC_HIRE_Farmer_Wait, NPC_HIRE_Builder, NPC_HIRE_Builder_Wait, NPC_HIRE_Vehicles,NPC_HIRE_Vehicles_Wait, MessageBeacon,
            NPC_HIRE_Scientist, NPC_HIRE_Scientist_Wait, NPC_Recruit, NPC_Freighter_Captain_Secondary, NPC_Recruit_Secondary, Vehicle, MessageModule,
            TechShop, VehicleRaseStart, BuildingShop, MissionGiver, HoloHub, HoloExplorer, HoloSceptic, HoloNoone, PortalRunEntry, PortalActivate, CrashedFreighter,
            GraveInCave, GlitchyStroyBox, NetworkPlayer, NetworkMonument, AnomalyComputer, AtlasPlinth, Epilogue, GuildEnvoy, ManageFleet, ManageExpeditions,
            Frigate, CustomiseCharacter, CustomiseShip, CustomiseWeapon, CustomiseVehicle, ClaimBaseAnywhere, FleetNavigator, FleetCommandPost, StoryUtility,
            MPMissionGiver, SpecialsShop }
		public InteractionTypeEnum InteractionType;
    }
}
