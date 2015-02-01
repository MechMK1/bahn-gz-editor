using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class Pixel
	{
		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
		public bool IsTransparent { get; set; }

		private Pixel()
		{

		}

		public static Pixel RGBPixel(byte red, byte green, byte blue)
		{
			Pixel p = new Pixel();
			p.Red = red;
			p.Green = green;
			p.Blue = blue;
			return p;
		}

		public static Pixel TransparentPixel()
		{
			Pixel p = new Pixel();
			p.IsTransparent = true;
			return p;
		}

		public static Pixel FromUInt(uint data)
		{
			Pixel p = new Pixel();
			if ((data & Constants.FARBE_LOGISCH) != 0)
			{
				if ((data & Constants.FARBE_TRANSPARENT) != 0)
				{
					p.IsTransparent = true;
				}
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
			if (IsTransparent)
			{
				return Constants.FARBE_TRANSPARENT;
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

		public override bool Equals(object obj)
		{
			if (obj is Pixel)
			{
				Pixel p = (Pixel)obj;
				if ((this.IsTransparent && p.IsTransparent) || (this.Red == p.Red && this.Green == p.Green && this.Blue == p.Blue))
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
	}
}
