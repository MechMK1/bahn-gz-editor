using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BahnEditor.BahnLib
{
	public class Pixel
	{
		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
		public bool IsTransparent
		{
			get
			{
				return this.SpecialWithoutRGB == SpecialColorWithoutRGB.Transparent;
			}
			set
			{
				if (value)
					this.SpecialWithoutRGB = SpecialColorWithoutRGB.Transparent;
				else
					this.SpecialWithoutRGB = SpecialColorWithoutRGB.None;
			}
		}
		public bool IsSpecialColorWithoutRGB
		{
			get
			{
				return SpecialWithoutRGB != SpecialColorWithoutRGB.None;
			}
		}
		public bool IsSpecialColorWithRGB
		{
			get
			{
				return SpecialWithRGB != SpecialColorWithRGB.None;
			}
		}
		public SpecialColorWithoutRGB SpecialWithoutRGB { get; private set; }

		public SpecialColorWithRGB SpecialWithRGB { get; private set; }

		private Pixel()
		{

		}

		/// <summary>
		/// Get a pixel from RGB
		/// </summary>
		/// <param name="red">red byte</param>
		/// <param name="green">green byte</param>
		/// <param name="blue">blue byte</param>
		/// <returns>pixel with color</returns>
		public static Pixel RGBPixel(byte red, byte green, byte blue)
		{
			Pixel p = new Pixel();
			p.Red = red;
			p.Green = green;
			p.Blue = blue;
			p.SpecialWithoutRGB = SpecialColorWithoutRGB.None;
			p.SpecialWithRGB = SpecialColorWithRGB.None;
			return p;
		}
		//HACK constructor?

		/// <summary>
		/// Get a transparent pixel
		/// </summary>
		/// <returns>transparent pixel</returns>
		public static Pixel TransparentPixel()
		{
			return Pixel.SpecialPixelWithoutRGB(SpecialColorWithoutRGB.Transparent);
		}
		//HACK should be static instance

		/// <summary>
		/// Get a pixel with configurable color
		/// </summary>
		/// <param name="specialColor">Enum of different configurable colors</param>
		/// <returns>Special pixel</returns>
		public static Pixel SpecialPixelWithoutRGB(SpecialColorWithoutRGB specialColor)
		{
			Pixel p = new Pixel();
			p.SpecialWithoutRGB = specialColor;
			p.SpecialWithRGB = SpecialColorWithRGB.None;
			return p;
		}

		/// <summary>
		/// Get a special pixel with RGB
		/// </summary>
		/// <param name="specialColor">Enum of different special colors</param>
		/// <param name="red">red byte</param>
		/// <param name="green">green byte</param>
		/// <param name="blue">blue byte</param>
		/// <returns>Pixel</returns>
		public static Pixel SpecialPixelWithRGB(SpecialColorWithRGB specialColor, byte red, byte green, byte blue)
		{
			Pixel p = Pixel.RGBPixel(red, green, blue);
			p.SpecialWithRGB = specialColor;
			return p;
		}

		//TODO Remove magic numbers
		public static Pixel FromUInt(uint data)
		{
			Pixel p = new Pixel();
			if ((data & Constants.FARBE_LOGISCH) != 0)
			{
				p.SpecialWithoutRGB = (SpecialColorWithoutRGB)data;
			}
			else if(((data & Constants.FARBE_LAMPE) != 0) || ((data & Constants.FARBE_IMMERHELL) != 0) || ((data & Constants.FARBE_FENSTER) != 0))
			{
				p.SpecialWithRGB = (SpecialColorWithRGB)(data & 0xFF000000);
				p.Blue = (byte)data;
				p.Green = (byte)(data >> 8);
				p.Red = (byte)(data >> 16);
			}
			else
			{
				p.Blue = (byte)data;
				p.Green = (byte)(data >> 8);
				p.Red = (byte)(data >> 16);
			}
			return p;
		}

		//HACK Convert what to uint
		public uint ConvertToUInt()
		{
			if (this.IsTransparent)
			{
				return Constants.FARBE_TRANSPARENT;
			}
			else if (this.IsSpecialColorWithoutRGB)
			{
				return (uint)this.SpecialWithoutRGB;
			}
			else
			{
				uint result = 0;
				result = 
					(
						(
							(uint)
							(
								(Blue)
								|
								(
									(Green) << 8
								)
							)
							|
							(
								(
									(uint)(Red)
								) << 16
							)
						)
					);
				if (this.IsSpecialColorWithRGB)
				{
					result = result | (uint)this.SpecialWithRGB;
				}
				return result;
			}
		}

		public static Pixel FromColor(Color color)
		{
			return Pixel.RGBPixel(color.R, color.G, color.B);
		}

		public Color ConvertToColor()
		{
			switch (this.SpecialWithoutRGB)
			{
				case SpecialColorWithoutRGB.None:
					return Color.FromArgb(this.Red, this.Green, this.Blue);
				case SpecialColorWithoutRGB.Transparent:
					return Color.FromArgb(0, 112, 0);
				case SpecialColorWithoutRGB.BehindGlass:
					return Color.FromArgb(0, 50, 100);
				case SpecialColorWithoutRGB.As_BG:
					return Color.FromArgb(0, 112, 0);
				case SpecialColorWithoutRGB.As_Sleepers0:
					return Color.FromArgb(188, 188, 188);
				case SpecialColorWithoutRGB.As_Sleepers1:
					return Color.FromArgb(84, 40, 0);
				case SpecialColorWithoutRGB.As_Sleepers3:
					return Color.FromArgb(84, 40, 0);
				case SpecialColorWithoutRGB.As_Rails_Road0:
					return Color.FromArgb(168, 168, 168);
				case SpecialColorWithoutRGB.As_Rails_Road1:
					return Color.FromArgb(60, 60, 60);
				case SpecialColorWithoutRGB.As_Rails_Road2:
					return Color.FromArgb(168, 168, 168);
				case SpecialColorWithoutRGB.As_Rails_Road3:
					return Color.FromArgb(104, 104, 104);
				case SpecialColorWithoutRGB.As_Rails_Trackbed0:
					return Color.FromArgb(104, 104, 104);
				case SpecialColorWithoutRGB.As_Rails_Trackbed1:
					return Color.FromArgb(148, 148, 148);
				case SpecialColorWithoutRGB.As_Rails_Trackbed2:
					return Color.FromArgb(148, 148, 148);
				case SpecialColorWithoutRGB.As_Rails_Trackbed3:
					return Color.FromArgb(104, 104, 104);
				case SpecialColorWithoutRGB.As_Marking_Point_Bus0:
					return Color.FromArgb(252, 252, 252);
				case SpecialColorWithoutRGB.As_Marking_Point_Bus1:
					return Color.FromArgb(252, 252, 252);
				case SpecialColorWithoutRGB.As_Marking_Point_Bus2:
					return Color.FromArgb(252, 252, 252);
				case SpecialColorWithoutRGB.As_Marking_Point_Bus3:
					return Color.FromArgb(252, 252, 252);
				case SpecialColorWithoutRGB.As_Marking_Point_Water:
					return Color.FromArgb(84, 252, 252);
				case SpecialColorWithoutRGB.As_Gravel:
					return Color.FromArgb(84, 84, 84);
				case SpecialColorWithoutRGB.As_Small_Gravel:
					return Color.FromArgb(168, 136, 0);
				case SpecialColorWithoutRGB.As_Grassy:
					return Color.FromArgb(0, 168, 0);
				case SpecialColorWithoutRGB.As_Path_BG:
					return Color.FromArgb(30, 180, 20);
				case SpecialColorWithoutRGB.As_Path_FG:
					return Color.FromArgb(168, 140, 0);
				case SpecialColorWithoutRGB.As_Text:
					return Color.FromArgb(252, 252, 252);
				default:
					throw new Exception("WTF");
			}
		}

		public override bool Equals(object obj)
		{
			Pixel p = obj as Pixel;
			if (p != null)
			{
				if
				(
					(
						this.SpecialWithoutRGB != SpecialColorWithoutRGB.None
						&&
						p.SpecialWithoutRGB != SpecialColorWithoutRGB.None
						&&
						this.SpecialWithoutRGB == p.SpecialWithoutRGB
					)
					||
					(
						this.Red == p.Red
						&&
						this.Green == p.Green
						&&
						this.Blue == p.Blue
						&&
						this.SpecialWithoutRGB == SpecialColorWithoutRGB.None
						&&
						p.SpecialWithoutRGB == SpecialColorWithoutRGB.None
					)
					||
					(
						this.IsSpecialColorWithRGB
						&&
						p.IsSpecialColorWithRGB
						&&
						this.Red == p.Red
						&&
						this.Green == p.Green
						&&
						this.Blue == p.Blue
						&&
						this.SpecialWithoutRGB == SpecialColorWithoutRGB.None
						&&
						p.SpecialWithoutRGB == SpecialColorWithoutRGB.None
					)
				)
				{
					return true;
				}
			}
			return false;
		}

		// HACK Why?
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		//TODO Remove magic numbers
		public enum SpecialColorWithoutRGB : uint
		{
			None = 0,
			Transparent = (Constants.FARBE_LOGISCH | 0x00000001),
			BehindGlass = (Constants.FARBE_LOGISCH),
			As_BG = (Constants.FARBE_LOGISCH | 0x00000100),
			As_Sleepers0 = (Constants.FARBE_LOGISCH | 0x00000101),
			As_Sleepers1 = (Constants.FARBE_LOGISCH | 0x00000102),
			As_Sleepers3 = (Constants.FARBE_LOGISCH | 0x00000103),
			As_Rails_Road0 = (Constants.FARBE_LOGISCH | 0x00000104),
			As_Rails_Road1 = (Constants.FARBE_LOGISCH | 0x00000105),
			As_Rails_Road2 = (Constants.FARBE_LOGISCH | 0x00000106),
			As_Rails_Road3 = (Constants.FARBE_LOGISCH | 0x00000107),
			As_Rails_Trackbed0 = (Constants.FARBE_LOGISCH | 0x00000108),
			As_Rails_Trackbed1 = (Constants.FARBE_LOGISCH | 0x00000109),
			As_Rails_Trackbed2 = (Constants.FARBE_LOGISCH | 0x0000010A),
			As_Rails_Trackbed3 = (Constants.FARBE_LOGISCH | 0x0000010B),
			As_Marking_Point_Bus0 = (Constants.FARBE_LOGISCH | 0x0000010C),
			As_Marking_Point_Bus1 = (Constants.FARBE_LOGISCH | 0x0000010D),
			As_Marking_Point_Bus2 = (Constants.FARBE_LOGISCH | 0x0000010E),
			As_Marking_Point_Bus3 = (Constants.FARBE_LOGISCH | 0x0000010F),
			As_Marking_Point_Water = (Constants.FARBE_LOGISCH | 0x00000110),
			As_Gravel = (Constants.FARBE_LOGISCH | 0x00000111),
			As_Small_Gravel = (Constants.FARBE_LOGISCH | 0x00000112),
			As_Grassy = (Constants.FARBE_LOGISCH | 0x00000113),
			As_Path_BG = (Constants.FARBE_LOGISCH | 0x00000114),
			As_Path_FG = (Constants.FARBE_LOGISCH | 0x00000115),
			As_Text = (Constants.FARBE_LOGISCH | 0x00000116)
		}

		public enum SpecialColorWithRGB : uint
		{
			None = 0,
			Always_Bright = Constants.FARBE_IMMERHELL,
			Lamp_Yellow = (Constants.FARBE_LAMPE | 0x00000000),
			Lamp_Red = (Constants.FARBE_LAMPE | 0x01000000),
			Lamp_ColdWhite = (Constants.FARBE_LAMPE | 0x02000000),
			Lamp_YellowWhite = (Constants.FARBE_LAMPE | 0x03000000),
			Lamp_Gas_Yellow = (Constants.FARBE_LAMPE | 0x04000000),
			Window_Yellow = (Constants.FARBE_FENSTER | 0x00000000),
			Window_Neon = (Constants.FARBE_FENSTER | 0x04000000) 
		}
	}
}
