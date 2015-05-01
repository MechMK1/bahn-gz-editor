namespace BahnEditor.BahnLib
{
	public static class Pixel
	{
		public static byte GetRed(uint pixel)
		{
			return (byte)(pixel >> 16);
		}

		public static byte GetGreen(uint pixel)
		{
			return (byte)(pixel >> 8);
		}

		public static byte GetBlue(uint pixel)
		{
			return (byte)(pixel);
		}

		public static PixelProperty GetProperty(uint pixel)
		{
			return (PixelProperty)(pixel & 0xFF000000);
		}

		public static uint SetRed(uint pixel, byte red)
		{
			return (pixel & 0xFF00FFFF) | (uint)(red << (16));
		}

		public static uint SetGreen(uint pixel, byte green)
		{
			return (pixel & 0xFFFF00FF) | (uint)(green << (8));
		}

		public static uint SetBlue(uint pixel, byte blue)
		{
			return (pixel & 0xFFFFFF00) | (uint)(blue);
		}

		public static uint SetProperty(uint pixel, PixelProperty property)
		{
			uint prop = (uint)property;
			return ((prop & Constants.ColorLogic) == Constants.ColorLogic) ? prop : (pixel & 0x00FFFFFF) | prop;
		}

		public static bool IsSpecial(uint pixel)
		{
			return (pixel >> 24) != 0;
		}

		public static bool UsesRgb(uint pixel)
		{
			return (pixel & Constants.ColorLogic) == 0;
		}

		public static bool IsTransparent(uint pixel)
		{
			return pixel == Constants.ColorTransparent;
		}

		public static uint Create(byte red, byte green, byte blue, PixelProperty property = PixelProperty.None)
		{
			uint result = (uint)property;
			if (UsesRgb(result))
			{
				result |= ((uint)blue       );
				result |= ((uint)green << 8 );
				result |= ((uint)red   << 16);
			}
			return result;
		}

		#region Enums
		// TODO Use Constants in Enum
		public enum PixelProperty : uint
		{
			None = 0,

			//With RGB
			AlwaysBright =        Constants.ColorAlwaysBright,
			LampYellow =          Constants.ColorLamp | 0x00000000,
			LampRed =             Constants.ColorLamp | 0x01000000,
			LampColdWhite =       Constants.ColorLamp | 0x02000000,
			LampYellowWhite =     Constants.ColorLamp | 0x03000000,
			LampGasYellow =       Constants.ColorLamp | 0x04000000,
			WindowYellow0 =       Constants.ColorWindow0,
			WindowYellow1 =       Constants.ColorWindow1,
			WindowYellow2 =       Constants.ColorWindow2,
			WindowNeon0 =         Constants.ColorWindow0 | 0x04000000,
			WindowNeon1 =         Constants.ColorWindow1 | 0x04000000,
			WindowNeon2 =         Constants.ColorWindow2 | 0x04000000,
			
			//Without RGB
			Transparent =         Constants.ColorTransparent,
			BehindGlass =         Constants.ColorLogic,
			AsBG =                Constants.ColorLogic | 0x00000100,
			AsSleepers0 =         Constants.ColorLogic | 0x00000101,
			AsSleepers1 =         Constants.ColorLogic | 0x00000102,
			AsSleepers3 =         Constants.ColorLogic | 0x00000103,
			AsRailsRoad0 =        Constants.ColorLogic | 0x00000104,
			AsRailsRoad1 =        Constants.ColorLogic | 0x00000105,
			AsRailsRoad2 =        Constants.ColorLogic | 0x00000106,
			AsRailsRoad3 =        Constants.ColorLogic | 0x00000107,
			AsRailsTrackbed0 =    Constants.ColorLogic | 0x00000108,
			AsRailsTrackbed1 =    Constants.ColorLogic | 0x00000109,
			AsRailsTrackbed2 =    Constants.ColorLogic | 0x0000010A,
			AsRailsTrackbed3 =    Constants.ColorLogic | 0x0000010B,
			AsMarkingPointBus0 =  Constants.ColorLogic | 0x0000010C,
			AsMarkingPointBus1 =  Constants.ColorLogic | 0x0000010D,
			AsMarkingPointBus2 =  Constants.ColorLogic | 0x0000010E,
			AsMarkingPointBus3 =  Constants.ColorLogic | 0x0000010F,
			AsMarkingPointWater = Constants.ColorLogic | 0x00000110,
			AsGravel =            Constants.ColorLogic | 0x00000111,
			AsSmallGravel =       Constants.ColorLogic | 0x00000112,
			AsGrassy =            Constants.ColorLogic | 0x00000113,
			AsPathBG =            Constants.ColorLogic | 0x00000114,
			AsPathFG =            Constants.ColorLogic | 0x00000115,
			AsText =              Constants.ColorLogic | 0x00000116
		}
		#endregion Enums
	}
}
