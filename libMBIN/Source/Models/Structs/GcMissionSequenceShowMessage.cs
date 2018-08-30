﻿namespace libMBIN.Models.Structs
{
    [NMS(Alignment = 0x10)]
    public class GcMissionSequenceShowMessage : NMSTemplate     // size: 0x230
    {
        /* 0x000 */ public GcMissionCategory Category;
        /* 0x004 */ public float Time;
        [NMS(Size = 0x80)]
        /* 0x008 */ public string Message;
        /* 0x088 */ public GcAudioWwiseEvents AudioEvent;
        /* 0x08C */ public float OSDTime;
        [NMS(Size = 0x80)]
        /* 0x090 */ public string OSDMessage;
        [NMS(Size = 0x80)]
        /* 0x110 */ public string OSDMessageSubtitle;
        /* 0x190 */ public Colour OSDMessageColour;
		public enum OSDMessageStyleEnum { Standard, Fancy }
		public OSDMessageStyleEnum OSDMessageStyle;
        /* 0x1A4 */ public bool OSDUseMissionIcon;
        /* 0x1A5 */ public bool DisableIcon;
        /* 0x1A6 */ public bool DisableTitlePrefix;
        [NMS(Size = 0x80)]
        /* 0x1A7 */ public string DebugText;
        [NMS(Size = 0x9, Ignore = true)]
        public byte[] EndPadding;
    }
}
