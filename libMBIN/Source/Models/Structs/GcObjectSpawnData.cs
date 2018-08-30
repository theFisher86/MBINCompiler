﻿using System.Collections.Generic;

namespace libMBIN.Models.Structs
{
    public class GcObjectSpawnData : NMSTemplate // 0x3B0 bytes
    {
        [NMS(Size = 0x10)]
        /* 0x000 */ public string DebugName;
		public enum TypeEnum { Instanced, Single }
		public TypeEnum Type;
        [NMS(Size = 4, Ignore = true)]
        /* 0x014 */ public byte[] Padding14;

        /* 0x018 */ public GcResourceElement Resource;
        /* 0x2C0 */ public List<GcResourceElement> AltResources;

        /* 0x2D0 */ public List<GcTerrainTileType> ExtraTileTypes;

        //* 0x2C0 */ public GcTerrainTileType PlacementTileType;

        [NMS(Size = 0x10)]
        /* 0x2E0 */ public string Placement;
        /* 0x2F0 */ public GcSeed PlacementSeed;
		public enum PlacementPriorityEnum { Low, Normal, High }
		public PlacementPriorityEnum PlacementPriority;

        /* 0x304 */ public float Coverage;
        /* 0x308 */ public float FlatDensity;
        /* 0x30C */ public float SlopeDensity;
        /* 0x310 */ public float SlopeMultiplier;

		public enum LargeObjectCoverageEnum { DoNotPlace, DoNotPlaceClose, OnlyPlaceAround, AlwaysPlace }
		public LargeObjectCoverageEnum LargeObjectCoverage;
		public enum OverlapStyleEnum { None, SameSeed, All }
		public OverlapStyleEnum OverlapStyle;
        /* 0x31C */ public float MinHeight;
        /* 0x320 */ public float MaxHeight;
        /* 0x324 */ public bool RelativeToSeaLevel;
        /* 0x328 */ public float MinAngle;
        /* 0x32C */ public float MaxAngle;

        /* 0x330 */ public int FadeMinRegionRadius;
        /* 0x334 */ public int FadeMaxRegionRadius;
        /* 0x338 */ public int FadeMaxImposterRadius;

        /* 0x33C */ public float FadeInStartDistance;
        /* 0x340 */ public float FadeInEndDistance;
        /* 0x344 */ public float FadeInOffsetDistance;
        /* 0x348 */ public float FadeOutStartDistance;
        /* 0x34C */ public float FadeOutEndDistance;
        /* 0x350 */ public float FadeOutOffsetDistance;
        [NMS(Size = 5)]
        /* 0x354 */ public float[] LodDistances;

        /* 0x368 */ public bool MatchGroundColour;
		public enum GroundColourIndexEnum { Auto, Main, Patch }
		public GroundColourIndexEnum GroundColourIndex;

        /* 0x370 */ public bool SwapPrimaryForSecondaryColour;
        /* 0x371 */ public bool SwapPrimaryForRandomColour;
        /* 0x372 */ public bool AlignToNormal;
        /* 0x374 */ public float MinScale;
        /* 0x378 */ public float MaxScale;
        /* 0x37C */ public float MinScaleY;
        /* 0x380 */ public float MaxScaleY;
        /* 0x384 */ public float SlopeScaling;
        /* 0x388 */ public float PatchEdgeScaling;
        /* 0x38C */ public float MaxXZRotation;

        /* 0x390 */ public bool AutoCollision;
        /* 0x391 */ public bool CollideWithPlayer;
        /* 0x392 */ public bool CollideWithPlayerVehicle;
        /* 0x393 */ public bool DestroyedByPlayerVehicle;
        /* 0x394 */ public bool DestroyedByPlayerShip;
        /* 0x395 */ public bool DestroyedByTerrainEdit;
        /* 0x396 */ public bool InvisibleToCamera;
        /* 0x397 */ public bool ObjectCreaturesCanEat;
        /* 0x398 */ public float ObjectShearWindStrength;
        [NMS(Size = 0x4, Ignore =true)]
        /* 0x394 */ public byte[] Padding394;
        [NMS(Size = 0x10)]
        /* 0x3A0 */ public string DestroyedByVehicleEffect;
    }
}
