using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class NeoPixel
	{
		#region Properties
		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
		public PixelProperty Property { get; set; }
		#endregion Properties

		#region Helpers
		public bool UsesRGB {
			get {
				if (!this.IsSpecial) return true;  // If Property is null or None, RGB is used
				else return ((uint)this.Property & Constants.ColorLogic) == 0; // If Property does not have ColorLogic set, RGB is used
			}
		}

		public bool IsSpecial
		{
			get {
				return (this.Property != PixelProperty.None); // If Property is anything but None, it is special
			}
		}

		public bool IsTransparent
		{
			get
			{
				return this.Property == PixelProperty.Transparent;
			}
		}
		#endregion Helpers

		#region Constructors
		public NeoPixel(byte red, byte green, byte blue, PixelProperty property = PixelProperty.None)
		{
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
			this.Property = property;
		}
		#endregion Constructors

		#region Converters
		public uint ToUInt()
		{
			uint result = (uint)this.Property;
			if (this.UsesRGB)
			{
				result |= ((uint)this.Blue       );
				result |= ((uint)this.Green << 8 );
				result |= ((uint)this.Red   << 16);
			}

			// Result: |0000|0000|0000|0000|
			//         | SPC| RED| GRN| BLU|
			
			return result;
		}
		public Color ToColor()
		{
			switch (this.Property)
			{
				//With RGB
				case PixelProperty.None:
				case PixelProperty.AlwaysBright:
				case PixelProperty.LampYellow:
				case PixelProperty.LampRed:
				case PixelProperty.LampColdWhite:
				case PixelProperty.LampYellowWhite:
				case PixelProperty.LampGasYellow:
				case PixelProperty.WindowYellow0:
				case PixelProperty.WindowYellow1:
				case PixelProperty.WindowYellow2:
				case PixelProperty.WindowNeon0:
				case PixelProperty.WindowNeon1:
				case PixelProperty.WindowNeon2:
					return Color.FromArgb(this.Red, this.Green, this.Blue);

				//Without RGB
				case PixelProperty.Transparent:
					return Color.FromArgb(0, 112, 0);
				case PixelProperty.BehindGlass:
					return Color.FromArgb(0, 50, 100);
				case PixelProperty.AsBG:
					return Color.FromArgb(0, 112, 0);
				case PixelProperty.AsSleepers0:
					return Color.FromArgb(188, 188, 188);
				case PixelProperty.AsSleepers1:
					return Color.FromArgb(84, 40, 0);
				case PixelProperty.AsSleepers3:
					return Color.FromArgb(84, 40, 0);
				case PixelProperty.AsRailsRoad0:
					return Color.FromArgb(168, 168, 168);
				case PixelProperty.AsRailsRoad1:
					return Color.FromArgb(60, 60, 60);
				case PixelProperty.AsRailsRoad2:
					return Color.FromArgb(168, 168, 168);
				case PixelProperty.AsRailsRoad3:
					return Color.FromArgb(104, 104, 104);
				case PixelProperty.AsRailsTrackbed0:
					return Color.FromArgb(104, 104, 104);
				case PixelProperty.AsRailsTrackbed1:
					return Color.FromArgb(148, 148, 148);
				case PixelProperty.AsRailsTrackbed2:
					return Color.FromArgb(148, 148, 148);
				case PixelProperty.AsRailsTrackbed3:
					return Color.FromArgb(104, 104, 104);
				case PixelProperty.AsMarkingPointBus0:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.AsMarkingPointBus1:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.AsMarkingPointBus2:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.AsMarkingPointBus3:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.AsMarkingPointWater:
					return Color.FromArgb(84, 252, 252);
				case PixelProperty.AsGravel:
					return Color.FromArgb(60, 60, 60);
				case PixelProperty.AsSmallGravel:
					return Color.FromArgb(168, 136, 0);
				case PixelProperty.AsGrassy:
					return Color.FromArgb(0, 168, 0);
				case PixelProperty.AsPathBG:
					return Color.FromArgb(30, 180, 20);
				case PixelProperty.AsPathFG:
					return Color.FromArgb(168, 140, 0);
				case PixelProperty.AsText:
					return Color.FromArgb(252, 252, 252);

				default:
					throw new ArgumentOutOfRangeException("property", "A property which was not defined here was encountered");
			}			
		}

		public static NeoPixel FromUInt(uint data)
		{
			//If ColorLogic is set, RGB data is not needed and "data" is interpreted as PixelProperty
			if ((data & Constants.ColorLogic) != 0)
			{
				return new NeoPixel(0, 0, 0, (PixelProperty)data);
			}

			//If either ColorLamp, ColorAlwaysBright or ColorWindow are set, data is interpreted as R, G, B and PixelProperty
			//All PixelProperty values which use RGB values have either of these set
			else if (((data & Constants.ColorLamp) != 0) || ((data & Constants.ColorAlwaysBright) != 0) || ((data & Constants.ColorWindow) != 0))
			{
				return new NeoPixel((byte)(data >> 16), (byte)(data >> 8), (byte)data, (PixelProperty)(data & 0xFF000000));
			}

			//Else, R, G and B are interpreted normally
			else
			{
				return new NeoPixel((byte)(data >> 16), (byte)(data >> 8), (byte)data);
			}
		}

		public static NeoPixel FromColor(Color color)
		{
			return new NeoPixel(color.R, color.G, color.B);
		}
		#endregion Converters

		#region Comparison
		public override bool Equals(object obj)
		{
			NeoPixel p = obj as NeoPixel;
			if (p == null)
				return false;

			if (this.Property != p.Property) return false;
			if (this.UsesRGB)
			{
				return this.CompareRGBOnly(p);
			}
			else return true;
		}

		public override int GetHashCode()
		{
			return (this.Red ^ this.Green ^ this.Blue ^ (int)this.Property);
		} 

		private bool CompareRGBOnly(NeoPixel p)
		{
			if (this.Red   != p.Red  ) return false;
			if (this.Green != p.Green) return false;
			if (this.Blue  != p.Blue ) return false;
			return true;
		}
		#endregion Comparison

		#region Enums
		// TODO Use Constants in Enum
		public enum PixelProperty : uint
		{
			None = 0,
			//With RGB
			AlwaysBright = Constants.ColorAlwaysBright,
			LampYellow = (Constants.ColorLamp | 0x00000000),
			LampRed = (Constants.ColorLamp | 0x01000000),
			LampColdWhite = (Constants.ColorLamp | 0x02000000),
			LampYellowWhite = (Constants.ColorLamp | 0x03000000),
			LampGasYellow = (Constants.ColorLamp | 0x04000000),
			WindowYellow0 = Constants.ColorWindow0,
			WindowYellow1 = Constants.ColorWindow1,
			WindowYellow2 = Constants.ColorWindow2,
			WindowNeon0 = (Constants.ColorWindow0 | 0x04000000),
			WindowNeon1 = (Constants.ColorWindow1 | 0x04000000),
			WindowNeon2 = (Constants.ColorWindow2 | 0x04000000),
			//Without RGB
			Transparent = Constants.ColorTransparent,
			BehindGlass = (Constants.ColorLogic),
			AsBG = (Constants.ColorLogic | 0x00000100),
			AsSleepers0 = (Constants.ColorLogic | 0x00000101),
			AsSleepers1 = (Constants.ColorLogic | 0x00000102),
			AsSleepers3 = (Constants.ColorLogic | 0x00000103),
			AsRailsRoad0 = (Constants.ColorLogic | 0x00000104),
			AsRailsRoad1 = (Constants.ColorLogic | 0x00000105),
			AsRailsRoad2 = (Constants.ColorLogic | 0x00000106),
			AsRailsRoad3 = (Constants.ColorLogic | 0x00000107),
			AsRailsTrackbed0 = (Constants.ColorLogic | 0x00000108),
			AsRailsTrackbed1 = (Constants.ColorLogic | 0x00000109),
			AsRailsTrackbed2 = (Constants.ColorLogic | 0x0000010A),
			AsRailsTrackbed3 = (Constants.ColorLogic | 0x0000010B),
			AsMarkingPointBus0 = (Constants.ColorLogic | 0x0000010C),
			AsMarkingPointBus1 = (Constants.ColorLogic | 0x0000010D),
			AsMarkingPointBus2 = (Constants.ColorLogic | 0x0000010E),
			AsMarkingPointBus3 = (Constants.ColorLogic | 0x0000010F),
			AsMarkingPointWater = (Constants.ColorLogic | 0x00000110),
			AsGravel = (Constants.ColorLogic | 0x00000111),
			AsSmallGravel = (Constants.ColorLogic | 0x00000112),
			AsGrassy = (Constants.ColorLogic | 0x00000113),
			AsPathBG = (Constants.ColorLogic | 0x00000114),
			AsPathFG = (Constants.ColorLogic | 0x00000115),
			AsText = (Constants.ColorLogic | 0x00000116)
		}
		#endregion Enums
	}
}
