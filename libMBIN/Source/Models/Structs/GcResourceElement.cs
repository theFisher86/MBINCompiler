﻿namespace libMBIN.Models.Structs
{
    public class GcResourceElement : NMSTemplate // size = 0x2A8
    {
        [NMS(Size = 0x80)]
        /* 0x000 */ public string Filename;

        [NMS(Size = 8, Ignore = true)]
        /* 0x080 */ public byte[] Padding80;

        /* 0x088 */ public GcSeed Seed;

        [NMS(Size = 0x200)]
        /* 0x098 */ public string AltId;

        /* 0x298 */ public TkProceduralTextureChosenOptionList ProceduralTexture;
    }
}
