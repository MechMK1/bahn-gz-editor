using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BahnEditor.BahnLib
{
	// TODO Write documentation
	public class Graphic
	{
		#region Private Fields

		/// <summary>
		/// 0th Dimension
		/// </summary>
		private const int Height = 0;

		/// <summary>
		/// 1st Dimension
		/// </summary>
		private const int Width = 1;

		private Dictionary<LayerID, CompressedLayer> layersCompressed;

		#endregion Private Fields

		#region Public Properties

		//TODO Move to GraphicProperties
		//TODO Remove direct access
		public List<DrivingWayElement> DrivingWay { get; private set; }

		public string InfoText { get; set; }

		public GraphicProperties Properties { get; private set; }

		public GraphicVersion Version { get; set; }

		public ZoomFactor ZoomFactor { get; private set; }

		#endregion Public Properties

		#region Public Constructors

		public Graphic(string infoText, ZoomFactor zoomFactor = ZoomFactor.Zoom1, GraphicVersion version = GraphicVersion.Version2)
			: this()
		{
			this.Version = version;
			this.ZoomFactor = zoomFactor;
			this.InfoText = infoText;
		}

		#endregion Public Constructors

		#region Private Constructors

		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.Graphic class, used in Load-Function
		/// </summary>
		private Graphic()
		{
			this.layersCompressed = new Dictionary<LayerID, CompressedLayer>();
			this.DrivingWay = new List<DrivingWayElement>();
			this.Properties = new GraphicProperties();
			this.Properties.RawData |= GraphicProperties.Properties.ColorFormat24BPP; //Set ColorFormat24BPP in Properties because the pixel-data is saved in 24bpp-format
		}

		#endregion Private Constructors

		#region Public Methods

		public static Graphic Load(string path)
		{
			if (File.Exists(path))
			{
				using (FileStream stream = File.OpenRead(path))
				{
					return Load(stream);
				}
			}
			else throw new FileNotFoundException("File not found", path);
		}

		/// <summary>
		/// Gets or sets a layer of the graphic. Null-value represents an empty or fully transparent layer.
		/// </summary>
		/// <param name="layerID">The LayerID</param>
		/// <returns>The layer</returns>
		public uint[,] this[LayerID layerID]
		{
			get
			{
				if (!layersCompressed.ContainsKey(layerID))
				{
					uint[,] layer = new uint[Constants.ElementHeight * 8 * (byte)this.ZoomFactor, Constants.ElementWidth * 3 * (byte)this.ZoomFactor];
					for (int x = 0; x < layer.GetLength(Width); x++)
					{
						for (int y = 0; y < layer.GetLength(Height); y++)
						{
							layer[y, x] = Constants.ColorTransparent;
						}
					}
					return layer;
				}
				return DecompressLayer(this.layersCompressed[layerID], this.ZoomFactor);
			}
			set
			{
				if (value == null)
					this.layersCompressed.Remove(layerID);
				else
				{
					CompressedLayer compressedLayer = CompressLayer(value, this.ZoomFactor);
					if (compressedLayer == null)
						this.layersCompressed.Remove(layerID);
					else
						this.layersCompressed[layerID] = compressedLayer;
				}
			}
		}

		public uint this[LayerID layerID, int x, int y]
		{
			get
			{
				return this[layerID][y, x];
			}
			set
			{
				uint[,] layer = this[layerID];
				layer[y, x] = value;
				this[layerID] = layer;
			}
		}

		public bool IsTransparent()
		{
			return this.layersCompressed.Count == 0;
		}

		public bool IsTransparent(LayerID layerID)
		{
			return !layersCompressed.ContainsKey(layerID);
		}

		public void Save(string path, bool overwrite)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (File.Exists(path) && !overwrite)
			{
				return;
			}
			else
			{
				using (FileStream stream = File.OpenWrite(path))
				{
					Save(stream);
				}
			}
		}

		#endregion Public Methods

		#region Internal Methods

		internal static Graphic Load(Stream path)
		{
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				Graphic graphic = Load(br);
				return graphic;
			}
		}

		internal static Graphic Load(BinaryReader br)
		{
			if (br == null)
				throw new ArgumentNullException("br");

			// Load Header
			Graphic graphic = new Graphic();
			while (br.ReadByte() != Constants.HeaderTextTerminator) { } //Headertext, can be skipped
			byte[] readIdentification = br.ReadBytes(3); //Read indentification string
			if (readIdentification[0] != 71 || readIdentification[1] != 90 || readIdentification[2] != 71) //identification string "GZG" ASCII-format
			{
				throw new InvalidDataException("wrong identification string");
			}

			byte zoomFactor = (byte)(br.ReadByte() - 48); //Zoomfactor is read as ASCII-character, then "converted" to a byte
			//ASCII 0 => 48, ASCII 1 => 49, etc...
			switch (zoomFactor)
			{
				case 1:
				case 2:
				case 4:
					graphic.ZoomFactor = (ZoomFactor)zoomFactor;
					break;

				default:
					throw new InvalidDataException("unknown zoom factor");
			}
			ushort readMajorVersion = br.ReadUInt16();
			byte[] readMinorVersion = br.ReadBytes(2);
			if (readMajorVersion != Constants.GraphicFileFormat || readMinorVersion[0] != 0x00)
			{
				throw new InvalidDataException("wrong version");
			}
			switch (readMinorVersion[1])
			{
				case (byte)GraphicVersion.Version0:
				case (byte)GraphicVersion.Version1:
				case (byte)GraphicVersion.Version2:
					graphic.Version = (GraphicVersion)readMinorVersion[1];
					break;

				default:
					throw new InvalidDataException("wrong version");
			}

			int p = br.ReadInt32(); //Properties
			graphic.Properties = new GraphicProperties() { RawData = (GraphicProperties.Properties)p };

			//If either Smoke or Steam is set in the GraphicProperties
			if (graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleX = br.ReadInt32();
				graphic.Properties.ParticleY = br.ReadInt32();
				graphic.Properties.ParticleWidth = br.ReadInt32();
			}

			//If Clock is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				if (graphic.Version == GraphicVersion.Version0)
				{
					throw new InvalidDataException("Clock is set, but invalid for the version of the graphic");
				}
				br.ReadInt32(); //skipping unused data (for future use)

				graphic.Properties.ClockProperties = (ClockProperties)br.ReadInt32();
				graphic.Properties.ClockX = br.ReadInt32();
				graphic.Properties.ClockY = br.ReadInt32();
				graphic.Properties.ClockZ = (LayerID)br.ReadInt32();
				graphic.Properties.ClockWidth = br.ReadInt32();
				graphic.Properties.ClockHeight = br.ReadInt32();

				graphic.Properties.ClockColorHoursPointer = br.ReadUInt32();
				graphic.Properties.ClockColorMinutesPointer = br.ReadUInt32();
				br.ReadInt32(); //skipping unused data (for future use)
			}

			//If Cursor is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Cursor))
			{
				if (graphic.Version < GraphicVersion.Version2)
				{
					throw new InvalidDataException("Cursor is set, but invalid for the version of the graphic");
				}
				graphic.Properties.CursorNormalDirection = (Direction)br.ReadInt32();
				graphic.Properties.CursorReverseDirection = (Direction)br.ReadInt32();
			}

			//If ColorInSchematicMode is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.ColorSchematicMode))
			{
				if (graphic.Version < GraphicVersion.Version2)
				{
					throw new InvalidDataException("ColorSchematicMode is set, but invalid for the version of the graphic");
				}
				graphic.Properties.ColorInSchematicMode = br.ReadUInt32();
				br.ReadUInt32(); //skipping unused data
			}

			//If DrivingWay is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.DrivingWay))
			{
				if (graphic.Version < GraphicVersion.Version2)
					throw new InvalidDataException("DrivingWay is set, but invalid for the version of the graphic");
				graphic.DrivingWay = new List<DrivingWayElement>();
				int count = br.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					graphic.DrivingWay.Add(DrivingWayElement.FromBytes(br.ReadBytes(12)));
				}
			}

			int layercount = br.ReadInt16();
			br.ReadUInt16(); //skipping unknown data, see more in save-method
			char c;
			StringBuilder sb = new StringBuilder();
			while ((c = br.ReadChar()) != Constants.UnicodeNull)
			{
				sb.Append(c);
			}
			graphic.InfoText = sb.ToString();

			// Load Data
			bool backgroundLayerExists = false;
			for (int i = 0; i < layercount; i++)
			{
				LayerID id = (LayerID)br.ReadInt16();
				CompressedLayer compressedLayer = _ReadLayerFromStream(br, graphic.ZoomFactor, graphic.Version);
				if (id == LayerID.Background0)
				{
					if (!backgroundLayerExists)
					{
						backgroundLayerExists = true;
					}
					else
					{
						id = LayerID.Background1;
					}
				}
				graphic.layersCompressed.Add(id, compressedLayer);
			}

			return graphic;
		}

		internal void Save(Stream path)
		{
			if (this.IsTransparent())
				return; //If there is nothing to do, just don't do anything. Duh.

			BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode);
			bw.Write(Constants.HeaderText.ToArray()); //Headertext
			bw.Write(new byte[] { 71, 90, 71 });      //identification string "GZG" ASCII-format
			bw.Write((byte)(48 + this.ZoomFactor));   //Zoomfactor, as ASCII-format
			bw.Write(Constants.GraphicFileFormat);    //Major Version
			bw.Write((byte)0);                        //Minor Version, normally a byte[2] array, but first element of the array is empty
			bw.Write((byte)this.Version);             //Minor Version
			bw.Write((int)this.Properties.RawData); //Properties
			if (this.Properties.HasParticles)
			{
				bw.Write(this.Properties.ParticleX);
				bw.Write(this.Properties.ParticleY);
				bw.Write(this.Properties.ParticleWidth);
			}
			if (this.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				bw.Write(1); //reserved for future use
				bw.Write((int)this.Properties.ClockProperties);
				bw.Write(this.Properties.ClockX);
				bw.Write(this.Properties.ClockY);
				bw.Write((int)this.Properties.ClockZ);
				bw.Write(this.Properties.ClockWidth);
				bw.Write(this.Properties.ClockHeight);
				bw.Write(this.Properties.ClockColorHoursPointer);
				bw.Write(this.Properties.ClockColorMinutesPointer);
				bw.Write(this.Properties.ClockColorHoursPointer); //HoursPointer is written twice because space is reserved for future use
			}

			if (this.Properties.RawData.HasFlag(GraphicProperties.Properties.Cursor))
			{
				bw.Write((int)this.Properties.CursorNormalDirection);
				bw.Write((int)this.Properties.CursorReverseDirection);
			}

			if (this.Properties.RawData.HasFlag(GraphicProperties.Properties.ColorSchematicMode))
			{
				bw.Write(this.Properties.ColorInSchematicMode);
				bw.Write(Constants.ColorTransparent); //Not used, reserved for future use
			}

			if (this.Properties.RawData.HasFlag(GraphicProperties.Properties.DrivingWay))
			{
				bw.Write(this.DrivingWay.Count);
				foreach (var item in this.DrivingWay)
				{
					bw.Write(item.ToBytes());
				}
			}
			bw.Write((short)this.layersCompressed.Count); //layer
			bw.Write((ushort)0xFFFE); //unknown, got data through analysis of existing files, doesn't work without
			bw.Write(this.InfoText.ToCharArray());
			bw.Write(Constants.UnicodeNull);
			foreach (var item in this.layersCompressed)
			{
				if (item.Key == LayerID.Background1)
					bw.Write((short)LayerID.Background0);
				else
					bw.Write((short)item.Key); //layer

				_WriteLayerToStream(item.Value, bw, this.ZoomFactor);
			}
			bw.Flush();
		}

		#endregion Internal Methods

		#region Private Methods

		private bool IsLayerTransparent(uint[,] layer)
		{
			for (int x = 0; x < layer.GetLength(Width); x++)
			{
				for (int y = 0; y < layer.GetLength(Height); y++)
				{
					if (!Pixel.IsTransparent(layer[y, x]))
						return false;
				}
			}
			return true;
		}

		private static uint[,] DecompressLayer(CompressedLayer layer, ZoomFactor zoomFactor)
		{
			int arrayPosition = 0;
			List<uint> colors = new List<uint>();
			for (int i = 0; i < layer.LayerData.Length; i++)
			{
				uint item = layer.LayerData[arrayPosition++];
				int count = 0;
				if ((item & Constants.ColorAdditionalDataMask) == Constants.ColorCompressed)
				{
					count = (int)(item & Constants.ColorMaskCompressedCount) + 2;
					if ((item & Constants.ColorMaskCompressedTransparent) != 0)
					{
						// item is transparent
						for (int k = 0; k < count; k++)
						{
							colors.Add(Constants.ColorTransparent);
						}
					}
					else if ((item & Constants.ColorMaskCompressedSystemcolor) != 0)
					{
						// item is a system-color
						for (int k = 0; k < count; k++)
						{
							colors.Add(((item & Constants.ColorMaskCompressedSFB) >> 8) + Constants.ColorAsMin);
						}
					}
					else
					{
						// item is a color, may be a set of colors
						uint wdhlen = ((item & Constants.ColorMaskCompressedLength) >> 8) + 1;
						if (wdhlen > Constants.MaxRepeatedColorsLength)
							throw new InvalidDataException("color repetition length out of range");
						List<uint> buffer = new List<uint>();
						for (int j = 0; j < wdhlen; j++)
						{
							buffer.Add(layer.LayerData[arrayPosition++]);
							i++;
						}
						for (int j = 0; j < count; j++)
						{
							foreach (var b in buffer)
							{
								colors.Add(b);
							}
						}
					}
				}
				else
				{
					// not packed, single pixel
					count = 1;
					colors.Add(item);
				}
			}
			int height = Constants.ElementHeight * 8 * (int)zoomFactor;
			int width = Constants.ElementWidth * 3 * (int)zoomFactor;
			uint[,] layerElement = new uint[height, width];
			int x0 = (int)(layer.X0 + Constants.ElementWidth * (int)zoomFactor);
			int y0 = (int)(layer.Y0 + Constants.ElementHeight * (int)zoomFactor);
			int _x0 = x0 + layer.Width;
			int _y0 = y0 + layer.Height;

			int position = 0;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (i >= y0 && i < _y0 && j >= x0 && j < _x0)
					{
						layerElement[i, j] = colors[position++];
					}
					else
					{
						layerElement[i, j] = Constants.ColorTransparent;
					}
				}
			}
			return layerElement;
		}

		//CALL STACK: LoadData -> ReadLayerFromStream -> _FillLayer
		private static uint[,] _FillLayer(uint[,] layer, int x0, int y0, ZoomFactor zoomFactor)
		{
			uint[,] newLayer = new uint[Constants.ElementHeight * 8 * (int)zoomFactor, Constants.ElementWidth * 3 * (int)zoomFactor];
			x0 = (int)(x0 + Constants.ElementWidth * (int)zoomFactor);
			y0 = (int)(y0 + Constants.ElementHeight * (int)zoomFactor);
			for (int i = 0; i < newLayer.GetLength(0); i++)
			{
				for (int j = 0; j < newLayer.GetLength(1); j++)
				{
					if (i >= y0 && i < y0 + layer.GetLength(0) && j >= x0 && j < x0 + layer.GetLength(1))
					{
						newLayer[i, j] = layer[i - y0, j - x0];
					}
					else
					{
						newLayer[i, j] = Constants.ColorTransparent;
					}
				}
			}
			return newLayer;
		}

		private static CompressedLayer CompressLayer(uint[,] decompressedLayer, ZoomFactor zoomFactor)
		{
			int layerWidth = decompressedLayer.GetLength(Width);
			int layerHeight = decompressedLayer.GetLength(Height);
			int minx = layerWidth;
			int miny = layerHeight;
			int maxx = 0;
			int maxy = 0;
			for (int i = 0; i < layerHeight; i++)
			{
				for (int j = 0; j < layerWidth; j++)
				{
					if (decompressedLayer[i, j] != Constants.ColorTransparent)
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
				return null;
			}
			maxx++;
			maxy++;
			List<uint> pixels = new List<uint>();
			int trimmedHeight = maxy - miny;
			int trimmedWidth = maxx - minx;
			for (int i = 0; i < trimmedHeight; i++)
			{
				for (int j = 0; j < trimmedWidth; j++)
				{
					pixels.Add(decompressedLayer[i + miny, j + minx]);
				}
			}
			short x0 = (short)(minx - Constants.ElementWidth * (int)zoomFactor);
			short y0 = (short)(miny - Constants.ElementHeight * (int)zoomFactor);

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
				else if (lastcolor == Constants.ColorTransparent)
				{
					colors.Add(Constants.ColorCompressedTransparent | (uint)(length - 2));
				}
				else if (((lastcolor & Constants.ColorLogic) != 0) && lastcolor != (uint)Pixel.PixelProperty.BehindGlass)
				{
					uint color = lastcolor - Constants.ColorAsMin;
					color = color << 8;
					color = color | Constants.ColorCompressedSystemcolor;
					colors.Add(color | (uint)(length - 2));
				}
				else
				{
					colors.Add(Constants.ColorCompressed | (uint)(length - 2));
					colors.Add(lastcolor);
				}
			}
			CompressedLayer layer = new CompressedLayer();
			layer.LayerData = colors.ToArray();
			layer.X0 = x0;
			layer.Y0 = y0;
			layer.Height = (short)trimmedHeight;
			layer.Width = (short)trimmedWidth;
			return layer;
		}
		private static void _WriteLayerToStream(CompressedLayer layer, BinaryWriter bw, BahnLib.ZoomFactor zoomFactor)
		{
			if (bw == null)
				throw new ArgumentNullException("bw");

			bw.Write(layer.X0); //x0
			bw.Write(layer.Y0); //y0
			bw.Write(layer.Width); //width
			bw.Write(layer.Height); //height
			bw.Write(layer.LayerData.Length);
			foreach (uint item in layer.LayerData)
			{
				bw.Write(item);
			}
		}

		//CALL STACK: LoadData -> ReadLayerFromStream
		private static CompressedLayer _ReadLayerFromStream(BinaryReader br, BahnLib.ZoomFactor zoomFactor, GraphicVersion graphicVersion)
		{
			if (br == null)
				throw new ArgumentNullException("br");

			CompressedLayer l = new CompressedLayer();
			l.X0 = br.ReadInt16();
			l.Y0 = br.ReadInt16();
			l.Width = br.ReadInt16();
			l.Height = br.ReadInt16();
			if (graphicVersion == GraphicVersion.Version2)
			{
				int layerLength = br.ReadInt32();
				List<uint> data = new List<uint>();
				for (int i = 0; i < layerLength; i++)
				{
					data.Add(br.ReadUInt32());
				}
				l.LayerData = data.ToArray();
			}
			else if (graphicVersion < GraphicVersion.Version2)
			{
				uint[,] decompressedLayer = _ReadLayerFromStreamVersion0(br, l.Width, l.Height);
				l = CompressLayer(_FillLayer(decompressedLayer, l.X0, l.Y0, zoomFactor), zoomFactor);
			}
			else
			{
				throw new ArgumentOutOfRangeException("graphicVersion");
			}
			return l;
		}

		//CALL STACK: LoadData -> ReadLayerFromStream -> _ReadLayerFromSteamVersion0
		private static uint[,] _ReadLayerFromStreamVersion0(BinaryReader br, short width, short height)
		{
			List<uint> colors = new List<uint>();
			int elementSize = width * height;
			while (elementSize > 0)
			{
				uint item = br.ReadUInt32();
				if ((item & Constants.ColorAdditionalDataMask) == Constants.ColorCompressed)
				{
					int count = (int)(item & Constants.ColorMaskCompressedCount) + 2;
					if ((item & Constants.ColorMaskCompressedTransparent) != 0)
					{
						// item is transparent
						for (int k = 0; k < count; k++)
						{
							colors.Add(Constants.ColorTransparent);
						}
					}
					else
					{
						// item is a color
						uint color = br.ReadUInt32();
						for (int k = 0; k < count; k++)
						{
							colors.Add(color);
						}
					}
					elementSize -= count;
				}
				else
				{
					// not packed, single pixel
					elementSize--;
					colors.Add(item);
				}
			}

			uint[,] element = new uint[height, width];
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

		#endregion Private Methods

		#region Nested Classes

		private class CompressedLayer
		{
			public uint[] LayerData { get; set; }

			public short Width { get; set; }

			public short Height { get; set; }

			public short X0 { get; set; }

			public short Y0 { get; set; }
		}

		#endregion Nested Classes
	}
}