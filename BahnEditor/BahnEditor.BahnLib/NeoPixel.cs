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
				else return ((uint)this.Property & Constants.COLOR_LOGIC) == 0; // If Property does nor have COLOR_LOGIC set, RGB is used
			}
		}

		public bool IsSpecial
		{
			get {
				return (this.Property != PixelProperty.None); // If Property is anything but None, it is special
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
				case PixelProperty.Always_Bright:
				case PixelProperty.Lamp_Yellow:
				case PixelProperty.Lamp_Red:
				case PixelProperty.Lamp_ColdWhite:
				case PixelProperty.Lamp_YellowWhite:
				case PixelProperty.Lamp_Gas_Yellow:
				case PixelProperty.Window_Yellow_0:
				case PixelProperty.Window_Yellow_1:
				case PixelProperty.Window_Yellow_2:
				case PixelProperty.Window_Neon_0:
				case PixelProperty.Window_Neon_1:
				case PixelProperty.Window_Neon_2:
					return Color.FromArgb(this.Red, this.Green, this.Blue);

				//Without RGB
				case PixelProperty.Transparent:
					return Color.FromArgb(0, 112, 0);
				case PixelProperty.BehindGlass:
					return Color.FromArgb(0, 50, 100);
				case PixelProperty.As_BG:
					return Color.FromArgb(0, 112, 0);
				case PixelProperty.As_Sleepers0:
					return Color.FromArgb(188, 188, 188);
				case PixelProperty.As_Sleepers1:
					return Color.FromArgb(84, 40, 0);
				case PixelProperty.As_Sleepers3:
					return Color.FromArgb(84, 40, 0);
				case PixelProperty.As_Rails_Road0:
					return Color.FromArgb(168, 168, 168);
				case PixelProperty.As_Rails_Road1:
					return Color.FromArgb(60, 60, 60);
				case PixelProperty.As_Rails_Road2:
					return Color.FromArgb(168, 168, 168);
				case PixelProperty.As_Rails_Road3:
					return Color.FromArgb(104, 104, 104);
				case PixelProperty.As_Rails_Trackbed0:
					return Color.FromArgb(104, 104, 104);
				case PixelProperty.As_Rails_Trackbed1:
					return Color.FromArgb(148, 148, 148);
				case PixelProperty.As_Rails_Trackbed2:
					return Color.FromArgb(148, 148, 148);
				case PixelProperty.As_Rails_Trackbed3:
					return Color.FromArgb(104, 104, 104);
				case PixelProperty.As_Marking_Point_Bus0:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.As_Marking_Point_Bus1:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.As_Marking_Point_Bus2:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.As_Marking_Point_Bus3:
					return Color.FromArgb(252, 252, 252);
				case PixelProperty.As_Marking_Point_Water:
					return Color.FromArgb(84, 252, 252);
				case PixelProperty.As_Gravel:
					return Color.FromArgb(60, 60, 60);
				case PixelProperty.As_Small_Gravel:
					return Color.FromArgb(168, 136, 0);
				case PixelProperty.As_Grassy:
					return Color.FromArgb(0, 168, 0);
				case PixelProperty.As_Path_BG:
					return Color.FromArgb(30, 180, 20);
				case PixelProperty.As_Path_FG:
					return Color.FromArgb(168, 140, 0);
				case PixelProperty.As_Text:
					return Color.FromArgb(252, 252, 252);

				default:
					throw new ArgumentOutOfRangeException("property", "A property which was not defined here was encountered");
			}
			
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
			Always_Bright = Constants.COLOR_ALWAYSBRIGHT,
			Lamp_Yellow = (Constants.COLOR_LAMP | 0x00000000),
			Lamp_Red = (Constants.COLOR_LAMP | 0x01000000),
			Lamp_ColdWhite = (Constants.COLOR_LAMP | 0x02000000),
			Lamp_YellowWhite = (Constants.COLOR_LAMP | 0x03000000),
			Lamp_Gas_Yellow = (Constants.COLOR_LAMP | 0x04000000),
			Window_Yellow_0 = Constants.COLOR_WINDOW_0,
			Window_Yellow_1 = Constants.COLOR_WINDOW_1,
			Window_Yellow_2 = Constants.COLOR_WINDOW_2,
			Window_Neon_0 = (Constants.COLOR_WINDOW_0 | 0x04000000),
			Window_Neon_1 = (Constants.COLOR_WINDOW_1 | 0x04000000),
			Window_Neon_2 = (Constants.COLOR_WINDOW_2 | 0x04000000),
			//Without RGB
			Transparent = Constants.COLOR_TRANSPARENT,
			BehindGlass = (Constants.COLOR_LOGIC),
			As_BG = (Constants.COLOR_LOGIC | 0x00000100),
			As_Sleepers0 = (Constants.COLOR_LOGIC | 0x00000101),
			As_Sleepers1 = (Constants.COLOR_LOGIC | 0x00000102),
			As_Sleepers3 = (Constants.COLOR_LOGIC | 0x00000103),
			As_Rails_Road0 = (Constants.COLOR_LOGIC | 0x00000104),
			As_Rails_Road1 = (Constants.COLOR_LOGIC | 0x00000105),
			As_Rails_Road2 = (Constants.COLOR_LOGIC | 0x00000106),
			As_Rails_Road3 = (Constants.COLOR_LOGIC | 0x00000107),
			As_Rails_Trackbed0 = (Constants.COLOR_LOGIC | 0x00000108),
			As_Rails_Trackbed1 = (Constants.COLOR_LOGIC | 0x00000109),
			As_Rails_Trackbed2 = (Constants.COLOR_LOGIC | 0x0000010A),
			As_Rails_Trackbed3 = (Constants.COLOR_LOGIC | 0x0000010B),
			As_Marking_Point_Bus0 = (Constants.COLOR_LOGIC | 0x0000010C),
			As_Marking_Point_Bus1 = (Constants.COLOR_LOGIC | 0x0000010D),
			As_Marking_Point_Bus2 = (Constants.COLOR_LOGIC | 0x0000010E),
			As_Marking_Point_Bus3 = (Constants.COLOR_LOGIC | 0x0000010F),
			As_Marking_Point_Water = (Constants.COLOR_LOGIC | 0x00000110),
			As_Gravel = (Constants.COLOR_LOGIC | 0x00000111),
			As_Small_Gravel = (Constants.COLOR_LOGIC | 0x00000112),
			As_Grassy = (Constants.COLOR_LOGIC | 0x00000113),
			As_Path_BG = (Constants.COLOR_LOGIC | 0x00000114),
			As_Path_FG = (Constants.COLOR_LOGIC | 0x00000115),
			As_Text = (Constants.COLOR_LOGIC | 0x00000116)
		}
		#endregion Enums
	}
}
