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
	}
}
