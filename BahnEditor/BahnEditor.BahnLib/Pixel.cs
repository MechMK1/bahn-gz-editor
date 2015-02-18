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
				return this.Logical == LogicalColor.Transparent;
			}
			set
			{
				if (value)
					this.Logical = LogicalColor.Transparent;
				else
					this.Logical = LogicalColor.None;
			}
		}
		public bool IsLogicalColor
		{
			get
			{
				return Logical != LogicalColor.None;
			}
		}
		public LogicalColor Logical { get; set; }

		private Pixel()
		{

		}

		public static Pixel RGBPixel(byte red, byte green, byte blue)
		{
			Pixel p = new Pixel();
			p.Red = red;
			p.Green = green;
			p.Blue = blue;
			p.Logical = LogicalColor.None;
			return p;
		}

		public static Pixel TransparentPixel()
		{
			return Pixel.LogicalPixel(LogicalColor.Transparent);
		}

		public static Pixel LogicalPixel(LogicalColor logicalColor)
		{
			Pixel p = new Pixel();
			p.Logical = logicalColor;
			return p;
		}

		public static Pixel FromUInt(uint data)
		{
			Pixel p = new Pixel();
			if ((data & Constants.FARBE_LOGISCH) != 0)
			{
				p.Logical = (LogicalColor)data;
			}
			else
			{
				p.Blue = (byte)data;
				p.Green = (byte)(data >> 8);
				p.Red = (byte)(data >> 16);
			}
			return p;
		}

		public uint ConvertToUInt()
		{
			if (this.IsTransparent)
			{
				return Constants.FARBE_TRANSPARENT;
			}
			else if (this.IsLogicalColor)
			{
				return (uint)this.Logical;
			}
			else
			{
				return
					(
						(
							(uint)
							(
								(Blue) |
								(
									(Green) << 8
								)
							) |
							(
								(
									(uint)(Red)
								) << 16
							)
						)
					);
			}
		}

		public static Pixel FromColor(Color color)
		{
			return Pixel.RGBPixel(color.R, color.G, color.B);
		}

		public Color ConvertToColor()
		{
			if (this.IsTransparent == true)
			{
				return Color.FromArgb(0, 112, 0);
			}
			return Color.FromArgb(this.Red, this.Green, this.Blue);
		}

		public override bool Equals(object obj)
		{
			Pixel p = obj as Pixel;
			if (p != null)
			{
				if ((this.Logical != LogicalColor.None && p.Logical != LogicalColor.None && this.Logical == p.Logical)
					|| (this.Red == p.Red && this.Green == p.Green && this.Blue == p.Blue && this.Logical == LogicalColor.None && p.Logical == LogicalColor.None))
				{
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public enum LogicalColor : uint
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
	}
}
