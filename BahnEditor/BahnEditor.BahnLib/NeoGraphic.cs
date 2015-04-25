using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	class NeoGraphic
	{
		private const int Height = 0;
		private const int Width = 1;

		private int layercount = 0;

		private Dictionary<LayerId, uint[,]> layers;

		public ZoomFactor ZoomFactor { get; private set; }

		public string InfoText { get; set; }

		public NeoGraphicProperties Properties { get; private set; }

		public GraphicVersion Version { get; set; }

		//TODO Move to NeoGraphicProperties
		public List<DrivingWayElement> DrivingWay { get; private set; }

		public NeoGraphic(string infoText, ZoomFactor zoomFactor = ZoomFactor.Zoom1, GraphicVersion version = GraphicVersion.Version2)
			: this()
		{
			this.Version = version;
			this.ZoomFactor = zoomFactor;
			this.InfoText = infoText;

		}

		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.Graphic class, used in Load-Function
		/// </summary>
		private NeoGraphic()
		{
			this.layers = new Dictionary<LayerId, uint[,]>();
			this.DrivingWay = new List<DrivingWayElement>();
		}

		public void AddTransparentLayer(LayerId layerID)
		{
			uint[,] layer = new uint[Constants.ElementHeight * 8 * (byte)this.ZoomFactor, Constants.ElementWidth * 3 * (byte)this.ZoomFactor];
			for (int x = 0; x < layer.GetLength(Width); x++)
			{
				for (int y = 0; y < layer.GetLength(Height); y++)
				{
					layer[y, x] = Constants.ColorTransparent;
				}
			}
			this.layers[layerID] = layer;
		}

		public void SetLayer(LayerId layerID, uint[,] layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			this.layers[layerID] = layer;
		}

		public uint[,] GetLayer(LayerId layerID)
		{
			return layers[layerID];
		}

		public bool IsElementEmpty()
		{
			foreach (var layer in this.layers)
			{
				for (int x = 0; x < layer.Value.GetLength(Width); x++)
				{
					for (int y = 0; y < layer.Value.GetLength(Height); y++)
					{
						if ((layer.Value[y, x] & Constants.ColorTransparent) != Constants.ColorTransparent)
							return false;
					}
				}
			}
			return true;
		}

		internal static NeoGraphic LoadHeader(BinaryReader br)
		{
			NeoGraphic graphic = new NeoGraphic();
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
			graphic.Properties = new NeoGraphicProperties() { RawData = (NeoGraphicProperties.Flags)p };
			
			//If either Smoke or Steam is set in the GraphicProperties
			if (graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleX = br.ReadInt32();
				graphic.Properties.ParticleY = br.ReadInt32();
				graphic.Properties.ParticleWidth = br.ReadInt32();
			}

			//If either Clock is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(NeoGraphicProperties.Flags.Clock))
			{
				if (graphic.Version == GraphicVersion.Version0)
				{
					throw new InvalidDataException("Clock is set, but invalid for the version of the graphic");
				}
				br.ReadInt32(); //skipping unused data (for future use)
				
				graphic.Properties.ClockProperties = (ClockProperties)br.ReadInt32();
				graphic.Properties.ClockX = br.ReadInt32();
				graphic.Properties.ClockY = br.ReadInt32();
				graphic.Properties.ClockZ = (LayerId)br.ReadInt32();
				graphic.Properties.ClockWidth = br.ReadInt32();
				graphic.Properties.ClockHeight = br.ReadInt32();
				
				graphic.Properties.ClockColorHoursPointer = br.ReadUInt32();
				graphic.Properties.ClockColorMinutesPointer = br.ReadUInt32();
				br.ReadInt32(); //skipping unused data (for future use)
			}

			//If Cursor is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(NeoGraphicProperties.Flags.Cursor))
			{
				if (graphic.Version < GraphicVersion.Version2)
				{
					throw new InvalidDataException("Cursor is set, but invalid for the version of the graphic");
				}
				graphic.Properties.CursorNormalDirection = (Direction)br.ReadInt32();
				graphic.Properties.CursorReverseDirection = (Direction)br.ReadInt32();
			}

			//If ColorInSchematicMode is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(NeoGraphicProperties.Flags.ColorSchematicMode))
			{
				if (graphic.Version < GraphicVersion.Version2)
				{
					throw new InvalidDataException("ColorSchematicMode is set, but invalid for the version of the graphic");
				}
				graphic.Properties.ColorInSchematicMode = br.ReadUInt32();
				br.ReadUInt32(); //skipping unused data
			}

			//If DrivingWay is set in the GraphicProperties
			if (graphic.Properties.RawData.HasFlag(NeoGraphicProperties.Flags.DrivingWay))
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


			graphic.layercount = br.ReadInt16();
			br.ReadUInt16(); //skipping unknown data, see more in save-method
			char c;
			StringBuilder sb = new StringBuilder();
			while ((c = br.ReadChar()) != Constants.UnicodeNull)
			{
				sb.Append(c);
			}
			graphic.InfoText = sb.ToString();
			return graphic;
		}

		//CALL STACK: LoadData
		internal void LoadData(BinaryReader br)
		{
			if (br == null)
				throw new ArgumentNullException();
			bool backgroundLayerExists = false;
			for (int i = 0; i < this.layercount; i++)
			{
				LayerId id = (LayerId)br.ReadInt16();
				uint[,] layer = ReadLayerFromStream(br, this.ZoomFactor, this.Version);
				if (id == LayerId.Background0)
				{
					if (!backgroundLayerExists)
					{
						backgroundLayerExists = true;
					}
					else
					{
						id = LayerId.Background1;
					}
				}
				this.SetLayer(id, layer);
			}
		}

		//CALL STACK: LoadData -> ReadLayerFromStream
		private static uint[,] ReadLayerFromStream(BinaryReader br, BahnLib.ZoomFactor zoomFactor, GraphicVersion graphicVersion)
		{
			if (br == null)
				throw new ArgumentNullException("br");
			
			short x0 = br.ReadInt16();
			short y0 = br.ReadInt16();
			short width = br.ReadInt16();
			short height = br.ReadInt16();
			uint[,] layer = null;
			if (graphicVersion == GraphicVersion.Version2)
			{
				layer = _ReadLayerFromSteamVersion2(br, width, height);
			}
			else if (graphicVersion < GraphicVersion.Version2)
			{
				layer = _ReadLayerFromSteamVersion0(br, width, height);
			}
			else
			{
				throw new ArgumentOutOfRangeException("version");
			}
			_FillLayer(ref layer, x0, y0, zoomFactor);
			return layer;
		}

		//CALL STACK: LoadData -> ReadLayerFromStream -> _ReadLayerFromSteamVersion2
		private static uint[,] _ReadLayerFromSteamVersion2(BinaryReader br, short width, short height)
		{
			List<uint> colors = new List<uint>();
			int elementSize = br.ReadInt32();
			for (int i = 0; i < elementSize; i++)
			{
				uint item = br.ReadUInt32();
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
							buffer.Add(br.ReadUInt32());
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

		//CALL STACK: LoadData -> ReadLayerFromStream -> _ReadLayerFromSteamVersion0
		private static uint[,] _ReadLayerFromSteamVersion0(BinaryReader br, short width, short height)
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

		//CALL STACK: LoadData -> ReadLayerFromStream -> _FillLayer
		private static void _FillLayer(ref uint[,] layer, int x0, int y0, ZoomFactor zoomFactor)
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
			layer = newLayer;
		}
	}
}
