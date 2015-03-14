using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class Layer
	{
		public short LayerID { get; set; }
		public Pixel[,] Element { get; set; }

		public Layer()
		{

		}

		public Layer(short layerID, Pixel[,] element)
		{
			this.LayerID = layerID;
			this.Element = element;
		}

		public void WriteLayerToStream(BinaryWriter bw)
		{
			if (bw == null)
				throw new ArgumentNullException("bw");
			short x0;
			short y0;
			Pixel[,] element = TrimElement(out x0, out y0, this.Element);
			bw.Write(this.LayerID); //layer
			bw.Write(x0); //x0
			bw.Write(y0); //y0
			bw.Write((short)element.GetLength(1)); //width
			bw.Write((short)element.GetLength(0)); //height
			WriteElementToStream(element, bw);
		}

		public static Layer ReadLayerFromStream(BinaryReader br)
		{
			if (br == null)
				throw new ArgumentNullException("br");
			Layer layer = new Layer();
			layer.LayerID = br.ReadInt16();
			short x0 = br.ReadInt16();
			short y0 = br.ReadInt16();
			short width = br.ReadInt16();
			short height = br.ReadInt16();
			Pixel[,] element = ReadElementFromStream(br, width, height);
			layer.Element = LoadElement(x0, y0, element);
			return layer;
		}

		private static void WriteElementToStream(Pixel[,] element, BinaryWriter bw)
		{
			try
			{
				List<uint> pixels = new List<uint>();
				for (int j = 0; j <= element.GetLength(0) - 1; j++)
				{
					for (int k = 0; k <= element.GetLength(1) - (short)1; k++)
					{
						pixels.Add(element[j, k].ConvertToUInt());
					}
				}
				List<uint> colors = new List<uint>();
				colors.Add(0);
				int colorposition = 0;
				while (colorposition < pixels.Count)
				{
					int length = 0;
					uint lastcolor = pixels[colorposition];
					for (; colorposition < pixels.Count; colorposition++)
					{
						if (lastcolor != pixels[colorposition] || length > 256)
						{
							break;
						}
						length++;
						lastcolor = pixels[colorposition];
					}
					if (length <= 1)
					{
						colors.Add(lastcolor);
					}
					else if (lastcolor == Constants.FARBE_TRANSPARENT)
					{
						colors.Add(Constants.FARBE_KOMPR_TR | (uint)(length - 2));
					}
					else if (((lastcolor & Constants.FARBE_LOGISCH) != 0) && lastcolor != (uint)Pixel.SpecialColorWithoutRGB.BehindGlass)
					{
						uint color = lastcolor - Constants.FARBE_WIE_MIN;
						color = color << 8;
						color = color | Constants.FARBE_KOMPR_SYS;
						colors.Add(color | (uint)(length - 2));
					}
					else
					{
						colors.Add(Constants.FARBE_KOMPRIMIERT | (uint)(length - 2));
						colors.Add(lastcolor);
					}


				}
				colors[0] = (uint)(colors.Count - 1);
				uint[] compressed = colors.ToArray();
				for (int j = 0; j < compressed.Length; j++)
				{
					bw.Write(compressed[j]);
				}
				return;
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

		private static Pixel[,] ReadElementFromStream(BinaryReader br, short width, short height)
		{
			List<Pixel> colors = new List<Pixel>();
			int elementSize = br.ReadInt32();
			for (int i = 0; i < elementSize; i++)
			{
				uint item = br.ReadUInt32();
				int count = 0;
				if((item & Constants.FARBE_ZUSATZ) == Constants.FARBE_KOMPRIMIERT)
				{
					count = (int)(item & Constants.FARBMASK_KOMPR_ZAHL) + 2;
					if((item & Constants.FARBMASK_KOMPR_TR) != 0)
					{
						// item is transparent
						for (int k = 0; k < count; k++)
						{
							colors.Add(Pixel.TransparentPixel());
						}
					}
					else if((item & Constants.FARBMASK_KOMPR_SYS) != 0)
					{
						// item is a system-color
						for (int k = 0; k < count; k++)
						{
							colors.Add(Pixel.SpecialPixelWithoutRGB((Pixel.SpecialColorWithoutRGB) (((item & Constants.FARBMASK_KOMPR_SFB) >> 8) + Constants.FARBE_WIE_MIN)));
						}
					}
					else
					{
						// item is a color, may be a set of colors
						uint wdhlen = ((item & Constants.FARBMASK_KOMPR_LEN) >> 8) + 1;
						wdhlen = Math.Min(wdhlen, Constants.MAX_FARBFOLGE_LEN);
						List<uint> buffer = new List<uint>();
						for (int j = 0; j < wdhlen; j++)
						{
							buffer.Add(br.ReadUInt32());
							i++;
						}
						for (int j = 0; j < count; j++)
						{
							foreach (var b in buffer)
							{
								colors.Add(Pixel.FromUInt(b));
							}
						}
					}
				}
				else
				{
					// not packed, single pixel
					count = 1;
					colors.Add(Pixel.FromUInt(item));
				}
			}

			Pixel[,] element = new Pixel[height, width];
			int position = 0;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					element[i, j] = colors[position];
					position++;
				}
			}
			return element;
		}

		private static Pixel[,] LoadElement(int x0, int y0, Pixel[,] element)
		{
			Pixel[,] newElement = new Pixel[Constants.SYMHOEHE * 8, Constants.SYMBREITE * 3];
			//Layer layer = this.graphic.Layers[0];
			x0 = (int)(x0 + Constants.SYMBREITE);
			y0 = (int)(y0 + Constants.SYMHOEHE);
			for (int i = 0; i < newElement.GetLength(0); i++)
			{
				for (int j = 0; j < newElement.GetLength(1); j++)
				{
					if (i >= y0 && i < y0 + element.GetLength(0) && j >= x0 && j < x0 + element.GetLength(1))
					{
						newElement[i, j] = element[i - y0, j - x0];
					}
					else
					{
						newElement[i, j] = Pixel.TransparentPixel();
					}
				}
			}
			return newElement;
		}

		internal static Pixel[,] TrimElement(out short x0, out short y0, Pixel[,] element)
		{
			int minx = element.GetLength(1);
			int miny = element.GetLength(0);
			int maxx = 0;
			int maxy = 0;
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					if (element[i, j].IsTransparent == false)
					{
						if (minx > j)
						{
							minx = j;
						}
						if (maxx < j)
						{
							maxx = j;
						}
						if (miny > i)
						{
							miny = i;
						}
						if (maxy < i)
						{
							maxy = i;
						}
					}
				}
			}
			if(maxx == 0 && maxy == 0)
			{
				throw new ElementIsEmptyException("Element is Empty");
			}
			maxx++;
			maxy++;
			Pixel[,] newElement = new Pixel[maxy - miny, maxx - minx];
			for (int i = 0; i < newElement.GetLength(0); i++)
			{
				for (int j = 0; j < newElement.GetLength(1); j++)
				{
					newElement[i, j] = element[i + miny, j + minx];
				}
			}
			x0 = (short)(minx - Constants.SYMBREITE);
			y0 = (short)(miny - Constants.SYMHOEHE);
			return newElement;
		}
	}
}
