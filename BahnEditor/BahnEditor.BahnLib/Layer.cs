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
		public short Height { get; set; }
		public short Width { get; set; }
		public short X0 { get; set; }
		public short Y0 { get; set; }
		public short LayerID { get; set; }
		public Pixel[,] Element { get; set; }

		public Layer()
		{

		}

		public Layer(short height, short width, short x0, short y0, short layerID, Pixel[,] element)
		{
			this.Height = height;
			this.Width = width;
			this.X0 = x0;
			this.Y0 = y0;
			this.LayerID = layerID;
			this.Element = element;
		}

		public void WriteLayerToStream(BinaryWriter bw)
		{
			bw.Write(this.LayerID); //layer
			bw.Write(this.X0); //x0
			bw.Write(this.Y0); //y0
			bw.Write(this.Width); //width
			bw.Write(this.Height); //height
			WriteElementToStream(bw);
		}

		public static Layer ReadLayerFromStream(BinaryReader br)
		{
			Layer layer = new Layer();
			layer.LayerID = br.ReadInt16();
			layer.X0 = br.ReadInt16();
			layer.Y0 = br.ReadInt16();
			layer.Width = br.ReadInt16();
			layer.Height = br.ReadInt16();
			layer.Element = ReadElementFromStream(br, layer.Width, layer.Height);
			return layer;
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
						throw new NotImplementedException("System colors not yet implemented");
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

		private void WriteElementToStream(BinaryWriter bw)
		{
			try
			{
				List<uint> pixels = new List<uint>();
				for (int j = 0; j <= this.Height - 1; j++)
				{
					for (int k = 0; k <= this.Width - (short)1; k++)
					{
						pixels.Add(this.Element[j, k].ConvertToUInt());
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
	}
}
