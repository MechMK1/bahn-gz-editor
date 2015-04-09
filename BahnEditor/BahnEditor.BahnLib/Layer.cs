using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Represents a layer of a graphic
	/// </summary>
	public class Layer
	{
		#region Fields and Properties
		/// <summary>
		/// Gets or sets the layer id
		/// </summary>
		public LayerId LayerId { get; set; }

		/// <summary>
		/// Gets the element of the layer
		/// </summary>
		public Pixel[,] Element { get; private set; }
		#endregion Fields and Properties

		#region Constructors
		private Layer()
		{
			// TODO remove other constructor. use named props instead
		}

		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.Layer class
		/// </summary>
		/// <param name="layerID">Layer id</param>
		/// <param name="element">Element</param>
		public Layer(LayerId layerID, Pixel[,] element)
		{
			this.LayerId = layerID;
			this.Element = element;
		}
		#endregion Constructors

		#region Public Methods
		/// <summary>
		/// Writes the Layer to the BinaryWriter
		/// </summary>
		/// <param name="bw">BinaryWriter</param>
		/// <param name="zoomFactor">Zoomfactor of the graphic, used to correctly downscale the element</param>
		/// <exception cref="System.ArgumentNullException"/>
		public void WriteLayerToStream(BinaryWriter bw, ZoomFactor zoomFactor)
		{
			if (bw == null)
				throw new ArgumentNullException("bw");
			short x0;
			short y0;
			Pixel[,] element = TrimElement(out x0, out y0, this.Element, zoomFactor);
			bw.Write((short)this.LayerId); //layer
			bw.Write(x0); //x0
			bw.Write(y0); //y0
			bw.Write((short)element.GetLength(1)); //width
			bw.Write((short)element.GetLength(0)); //height
			WriteElementToStreamVersion2(element, bw);
		}
		#endregion Public Methods

		#region Static Methods
		/// <summary>
		/// Reads a layer from a BinaryReader.
		/// </summary>
		/// <param name="br">The BinaryReader to read the data from</param>
		/// <param name="zoomFactor">Zoomfactor of the graphic, used to correctly upscale the element</param>
		/// <param name="version">Version of the graphic, used to determine which format the data is saved</param>
		/// <returns>Layer</returns>
		/// <exception cref="System.ArgumentNullException"/>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public static Layer ReadLayerFromStream(BinaryReader br, ZoomFactor zoomFactor, GraphicVersion version)
		{
			if (br == null)
				throw new ArgumentNullException("br");
			Layer layer = new Layer();
			short layerId = br.ReadInt16();
			layer.LayerId = (LayerId)Enum.Parse(typeof(LayerId), layerId.ToString(CultureInfo.InvariantCulture));
			short x0 = br.ReadInt16();
			short y0 = br.ReadInt16();
			short width = br.ReadInt16();
			short height = br.ReadInt16();
			Pixel[,] element = null;
			if (version == GraphicVersion.Version2)
			{
				element = ReadElementFromStreamVersion2(br, width, height);
			}
			else if(version < GraphicVersion.Version2)
			{
				element = ReadElementFromStreamVersion0(br, width, height);
			}
			else
			{
				throw new ArgumentOutOfRangeException("version");
			}
			layer.Element = LoadElement(x0, y0, element, zoomFactor);
			return layer;
		}

		/// <summary>
		/// Writes the element to the BinaryWriter in version2-format
		/// </summary>
		/// <param name="element">Element</param>
		/// <param name="bw">BinaryWriter</param>
		private static void WriteElementToStreamVersion2(Pixel[,] element, BinaryWriter bw)
		{
			List<uint> pixels = new List<uint>();
			for (int j = 0; j <= element.GetLength(0) - 1; j++)
			{
				for (int k = 0; k <= element.GetLength(1) - (short)1; k++)
				{
					pixels.Add(element[j, k].ToUInt());
				}
			}
			List<uint> colors = new List<uint>();
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
				else if (lastcolor == Constants.COLOR_TRANSPARENT)
				{
					colors.Add(Constants.COLOR_COMPRESSED_TRANSPARENT | (uint)(length - 2));
				}
				else if (((lastcolor & Constants.COLOR_LOGIC) != 0) && lastcolor != (uint)Pixel.SpecialPixelWithoutRGB.BehindGlass)
				{
					uint color = lastcolor - Constants.COLOR_AS_MINIMUM;
					color = color << 8;
					color = color | Constants.COLOR_COMPRESSED_SYSTEMCOLOR;
					colors.Add(color | (uint)(length - 2));
				}
				else
				{
					colors.Add(Constants.COLOR_COMPRESSED | (uint)(length - 2));
					colors.Add(lastcolor);
				}


			}
			colors.Insert(0, (uint)(colors.Count));
			uint[] compressed = colors.ToArray();
			for (int j = 0; j < compressed.Length; j++)
			{
				bw.Write(compressed[j]);
			}
			return;
		}
		#endregion Static Methods

		#region Private Methods
		/// <summary>
		/// Reads the element from the BinaryReader in version0-format
		/// </summary>
		/// <param name="br">BinaryReader</param>
		/// <param name="width">Width of the element</param>
		/// <param name="height">Height of the element</param>
		/// <returns>Element</returns>
		private static Pixel[,] ReadElementFromStreamVersion0(BinaryReader br, short width, short height)
		{
			List<Pixel> colors = new List<Pixel>();
			int elementSize = width * height;
			while (elementSize > 0)
			{
				uint item = br.ReadUInt32();
				if ((item & Constants.COLOR_ADDITIONAL_DATA_MASK) == Constants.COLOR_COMPRESSED)
				{
					int count = (int)(item & Constants.COLORMASK_COMPRESSED_COUNT) + 2;
					if ((item & Constants.COLORMASK_COMPRESSED_TRANSPARENT) != 0)
					{
						// item is transparent
						for (int k = 0; k < count; k++)
						{
							colors.Add(Pixel.TransparentPixel());
						}
					}
					else
					{
						// item is a color
						uint color = br.ReadUInt32();
						for (int k = 0; k < count; k++)
						{
							colors.Add(Pixel.FromUInt(color));
						}
					}
					elementSize -= count;
				}
				else
				{
					// not packed, single pixel
					elementSize--;
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

		/// <summary>
		/// Reads the element from the BinaryReader in version2-format
		/// </summary>
		/// <param name="br">BinaryReader</param>
		/// <param name="width">Width of the element</param>
		/// <param name="height">Height of the element</param>
		/// <returns>Element</returns>
		/// <exception cref="System.IO.InvalidDataException"/>
		private static Pixel[,] ReadElementFromStreamVersion2(BinaryReader br, short width, short height)
		{
			List<Pixel> colors = new List<Pixel>();
			int elementSize = br.ReadInt32();
			for (int i = 0; i < elementSize; i++)
			{
				uint item = br.ReadUInt32();
				int count = 0;
				if ((item & Constants.COLOR_ADDITIONAL_DATA_MASK) == Constants.COLOR_COMPRESSED)
				{
					count = (int)(item & Constants.COLORMASK_COMPRESSED_COUNT) + 2;
					if ((item & Constants.COLORMASK_COMPRESSED_TRANSPARENT) != 0)
					{
						// item is transparent
						for (int k = 0; k < count; k++)
						{
							colors.Add(Pixel.TransparentPixel());
						}
					}
					else if ((item & Constants.COLORMASK_COMPRESSED_SYSTEMCOLOR) != 0)
					{
						// item is a system-color
						for (int k = 0; k < count; k++)
						{
							colors.Add(new Pixel((Pixel.SpecialPixelWithoutRGB)(((item & Constants.COLORMASK_COMPRESSED_SFB) >> 8) + Constants.COLOR_AS_MINIMUM)));
						}
					}
					else
					{
						// item is a color, may be a set of colors
						uint wdhlen = ((item & Constants.COLORMASK_COMPRESSED_LENGTH) >> 8) + 1;
						if (wdhlen > Constants.MAX_REPEATED_COLORS_LENGTH)
							throw new InvalidDataException("color repetition length out of range");
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

		/// <summary>
		/// Upscales the element to the full format
		/// </summary>
		/// <param name="x0">lower left corner x co-ordinate in pixels</param>
		/// <param name="y0">lower left corner y co-ordinate in pixels</param>
		/// <param name="element">Element</param>
		/// <param name="zoomFactor">Zoomfactor for upscaling</param>
		/// <returns>Upscaled element</returns>
		private static Pixel[,] LoadElement(int x0, int y0, Pixel[,] element, ZoomFactor zoomFactor)
		{
			Pixel[,] newElement = new Pixel[Constants.ELEMENTHEIGHT * 8 * (int)zoomFactor, Constants.ELEMENTWIDTH * 3 * (int)zoomFactor];
			x0 = (int)(x0 + Constants.ELEMENTWIDTH * (int)zoomFactor);
			y0 = (int)(y0 + Constants.ELEMENTHEIGHT * (int)zoomFactor);
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

		/// <summary>
		/// Trims (Downscales) the element
		/// </summary>
		/// <param name="x0">lower left corner x co-ordinate in pixels</param>
		/// <param name="y0">lower left corner y co-ordinate in pixels</param>
		/// <param name="element">Element</param>
		/// <param name="zoomFactor">Zoomfactor for downscaling</param>
		/// <returns>Donwscaled Element</returns>
		private static Pixel[,] TrimElement(out short x0, out short y0, Pixel[,] element, ZoomFactor zoomFactor)
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
			if (maxx == 0 && maxy == 0)
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
			x0 = (short)(minx - Constants.ELEMENTWIDTH * (int)zoomFactor);
			y0 = (short)(miny - Constants.ELEMENTHEIGHT * (int)zoomFactor);
			return newElement;
		}
		#endregion Private Methods
	}
}
