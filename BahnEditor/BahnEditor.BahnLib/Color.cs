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
		public const uint FARBE_WIE_MIN = (FARBE_LOGISCH | 0x00000100);

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
				int  colorposition = 0;
				while (colorposition < color.Length)
				{
					int length = 0;
					uint lastcolor = color[colorposition];
					for (; colorposition < color.Length; colorposition++)
					{
						if (lastcolor != color[colorposition])
						{
							break;
						}
						length++;
						lastcolor = color[colorposition];
					}
					if (lastcolor == FARBE_TRANSPARENT)
					{
						colors.Add(FARBE_KOMPR_TR | (uint)(length - 2));
					}
					else
					{
						colors.Add(FARBE_KOMPRIMIERT | (uint)(length - 2));
						colors.Add(lastcolor);
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
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			int viewlen, i, aktColumn = 1;
			uint currentColor, f, count, wdhlen;  // i.e. COLORREFRGB
			List<uint> color = new List<uint>();

			viewlen = (int)input[0]; // Length of packed data in COLORREFRGB, see remark above
			while (aktColumn < viewlen)
			{
				currentColor = input[aktColumn++]; // read next value from file or from buffer
				if ((currentColor & FARBE_ZUSATZ) == FARBE_KOMPRIMIERT)
				// packed, more than 1 pixel
				{
					// Bit7..0 contain number of loops minus 2, ie 0..255 for 2..257
					count = (currentColor & FARBMASK_KOMPR_ZAHL) + 2;

					if ((currentColor & FARBMASK_KOMPR_TR) != 0)
					{
						// this "color" is needed more than any other...
						for (int k = 0; k < count; k++)
						{
							color.Add(FARBE_TRANSPARENT);
						}
						wdhlen = 1;
					}
					else
					{
						if ((currentColor & FARBMASK_KOMPR_SYS) != 0)
						{
							// this color is not RGB but a configurable color
							for (int k = 0; k < count; k++)
							{
								color.Add(((currentColor & FARBMASK_KOMPR_SFB) >> 8) + FARBE_WIE_MIN);
							}
							wdhlen = 1;
						}
						else
						{
							wdhlen = ((currentColor & FARBMASK_KOMPR_LEN) >> 8) + 1;
							wdhlen = Math.Min(wdhlen, MAX_FARBFOLGE_LEN); // for security, but otherwise you should cancel here
							// there follows a number of RGB colors
							for (int k = 0; k < count; k++)
							{
								for (i = 0; i < wdhlen; i++)
								{
									f = input[aktColumn];
									color.Add(f);
								}
							}
							aktColumn++;
						}
					}
				}
				else // not packed, single pixel
				{
					count = 1;
					wdhlen = 1;
					color.Add(currentColor);
				}
			}
			return color.ToArray();
		}

		private static uint GetRGB(byte red, byte green, byte blue)
		{
			return ((uint)((uint)((blue) | ((ushort)(green) << 8)) | (((uint)(red)) << 16)));
		}
	}
}
