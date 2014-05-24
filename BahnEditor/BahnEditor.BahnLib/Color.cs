using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public static class Color
	{
		public const uint FARBE_LOGISCH = 0x80000000;
		public const uint FARBE_TRANSPARENT = (FARBE_LOGISCH | 0x00000001);
		public const uint FARBE_LAMPE = 0x50000000;
		public const uint FARBE_ZUSATZ = 0xFf000000;

		public const uint FARBE_KOMPRIMIERT = (FARBE_LOGISCH | 0x40000000);
		public const uint FARBMASK_KOMPR_TR = 0x00010000;
		public const uint FARBE_KOMPR_TR = (FARBE_KOMPRIMIERT | FARBMASK_KOMPR_TR);
		public const uint FARBMASK_KOMPR_SYS = 0x00040000;
		public const uint FARBE_KOMPR_SYS = (FARBE_KOMPRIMIERT | FARBMASK_KOMPR_SYS);
		public const uint FARBMASK_KOMPR_SFB = 0x0000ff00;
		public const uint FARBMASK_KOMPR_ZAHL = 0x000000Ff;
		public const uint MAX_FARB_WDH = 257;
		public const uint FARBMASK_KOMPR_LEN = 0x0000Ff00;
		public const uint MAX_FARBFOLGE_LEN = 4;


		public static uint[] Compress(uint[] color)
		{
			try
			{
				if (color == null)
				{
					throw new ArgumentNullException("color");
				}
				
				List<uint> colors = new List<uint>();
				colors.Add(0);
				int colorcounter = 1, colorposition = 0;
				while (colorposition < color.Length)
				{
					colors[colorcounter] = FARBE_KOMPRIMIERT;
					int length = 0;
					uint lastcolor = color[colorcounter];
					for (; colorposition < color.Length; colorposition++)
					{
						if (lastcolor != color[colorposition])
						{
							break;
						}
						length++;
						lastcolor = color[colorposition];
					}
					colors[colorcounter] = colors[colorcounter] | (uint)(length - 2);
					if (color[colorposition] == FARBE_TRANSPARENT)
					{
						colors[colorcounter] = colors[colorcounter] | FARBE_KOMPR_TR;
						colorcounter++;
					}
					else
					{
						colors[colorcounter + 1] = lastcolor;
						colorcounter += 2;
					}


				}
				colors[0] = (uint)(colors.Count - 1);
				return colors.ToArray();
			}
			catch (IndexOutOfRangeException)
			{

				throw;
			}
			catch (ArgumentNullException)
			{

				throw;
			}
		}

		public static uint[] Decompress(uint[] input)
		{
			int viewlen, count, wdhlen, i, aktColor = 0;
			uint currentColor, f;  // i.e. COLORREFRGB
			List<uint> color = new List<uint>();

			//viewlen = file.Length; // Length of packed data in COLORREFRGB, see remark above

			currentColor = input[aktColor]; // read next value from file or from buffer
			if ((w32 & FARBE_ZUSATZ) == FARBE_KOMPRIMIERT)
			// packed, more than 1 pixel
			{
				// Bit7..0 contain number of loops minus 2, ie 0..255 for 2..257
				count = (w32 & FARBMASK_KOMPR_ZAHL) + 2;

				if (w32 & FARBMASK_KOMPR_TR)
				{
					// this "color" is needed more than any other...
					color[0] = FARBE_TRANSPARENT;
					wdhlen = 1;
				}
				else
				{
					if (w32 & FARBMASK_KOMPR_SYS)
					{
						// this color is not RGB but a configurable color
						color[0] = ((w32 & FARBMASK_KOMPR_SFB) >> 8) + FARBE_WIE_MIN;
						wdhlen = 1;
					}
					else
					{
						wdhlen = ((w32 & FARBMASK_KOMPR_LEN) >> 8) + 1;
						wdhlen = min(wdhlen, MAX_FARBFOLGE_LEN); // for security, but otherwise you should cancel here
						// there follows a number of RGB colors
						for (i = 0; i < wdhlen; i++)
						{
							f = ReadWord32();
							color[i] = f;
						}
					}
				}
			}
			else // not packed, single pixel
			{
				count = 1;
				wdhlen = 1;
				color[0] = w32;
			}

		}
	}
}
