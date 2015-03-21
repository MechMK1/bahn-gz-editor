using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BahnEditor.BahnLib
{
	// TODO Write doc for class
	public class Graphic
	{
		#region Fields and Properties
		private List<Layer> layers;

		/// <summary>
		/// Gets the zoom factor
		/// </summary>
		public ZoomFactor ZoomFactor { get; private set; }

		/// <summary>
		/// Gets or sets the info-text
		/// </summary>
		public string InfoText { get; set; }

		/// <summary>
		/// Gets or sets the color in map / schematic view
		/// <para>Makes only sense if ColorSchematicMode is set in properties, else data is ignored</para>
		/// </summary>
		public Pixel ColorInSchematicMode { get; set; }

		/// <summary>
		/// Gets or sets the properties of the graphic
		/// </summary>
		public GraphicProperties Properties { get; set; }

		/// <summary>
		/// Gets or sets the x position where steam/smoke begins
		/// <para>Makes only sense if steam or smoke is set in properties, else data is ignored</para>
		/// </summary>
		public int SteamXPosition { get; set; }

		/// <summary>
		/// Gets or sets the y position where steam/smoke begins
		/// <para>Makes only sense if steam or smoke is set in properties, else data is ignored</para>
		/// </summary>
		public int SteamYPosition { get; set; }

		/// <summary>
		/// Gets or sets the width (length) of steam/smoke
		/// <para>Makes only sense if steam or smoke is set in properties, else data is ignored</para>
		/// </summary>
		public int SteamWidth { get; set; }

		/// <summary>
		/// Gets or sets the properties of the clock
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public ClockProperties ClockProperties { get; set; }

		/// <summary>
		/// Gets or sets the x position of the center of the clock
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public int ClockXPosition { get; set; }

		/// <summary>
		/// Gets or sets the y position of the center of the clock
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public int ClockYPosition { get; set; }

		/// <summary>
		/// Gets or sets the z position (layer) of the center of the clock
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public int ClockZPosition { get; set; }

		/// <summary>
		/// Gets or sets the width of the clock
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public int ClockWidth { get; set; }

		/// <summary>
		/// Gets or sets the height of the clock
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public int ClockHeight { get; set; }

		/// <summary>
		/// Gets or sets the color of the hours pointer
		/// <para>Colors that light at night are possible</para>
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public Pixel ClockColorHoursPointer { get; set; }

		/// <summary>
		/// Gets or sets the color of the minutes pointer
		/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
		/// </summary>
		public Pixel ClockColorMinutesPointer { get; set; }

		/// <summary>
		/// Gets or sets the normal cursor direction
		/// <para>Makes only sense if cursor is set in properties, else data is ignored</para>
		/// </summary>
		public CursorDirection CursorNormalDirection { get; set; }

		/// <summary>
		/// Gets or sets the reverse cursor direction
		/// <para>Mostly, this is the opposite direction</para>
		/// <para>Makes only sense if cursor is set in properties, else data is ignored</para>
		/// </summary>
		public CursorDirection CursorReverseDirection { get; set; }
		#endregion Fields and Properties

		#region Constructors

		/// <summary>
		/// Initializes a new Instance of BahnEditor.Graphic class
		/// </summary>
		/// <param name="infoText">Infotext</param>
		/// <param name="zoomFactor">Zoom factor of the graphic</param>
		public Graphic(string infoText, ZoomFactor zoomFactor)
		{
			this.ZoomFactor = zoomFactor;
			this.InfoText = infoText;
			layers = new List<Layer>();
		}

		/// <summary>
		/// Initializes a new Instance of BahnEditor.Graphic class
		/// </summary>
		/// <param name="infoText">Infotext</param>
		public Graphic(string infoText)
			: this(infoText, ZoomFactor.Zoom1)
		{
		}

		/// <summary>
		/// Initializes a new Instance of BahnEditor.Graphic class, used in Load-Function
		/// </summary>
		private Graphic()
		{
			layers = new List<Layer>();
		}
		#endregion Constructors

		#region Public Methods

		/// <summary>
		/// Adds a transparent layer (every pixel of layer is transparent) to the graphic
		/// </summary>
		/// <param name="layerID">LayerId (Defined in Constants)</param>
		public void AddTransparentLayer(short layerID)
		{
			Pixel[,] element = new Pixel[Constants.SYMHOEHE * 8 * (byte)this.ZoomFactor, Constants.SYMBREITE * 3 * (byte)this.ZoomFactor];
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					element[i, j] = Pixel.TransparentPixel();
				}
			}
			Layer layer = new Layer(layerID, element);
			this.AddLayer(layer);
		}

		/// <summary>
		/// Adds a layer to the graphic
		/// </summary>
		/// <param name="layer">Layer</param>
		/// <exception cref="System.ArgumentNullException"/>
		public void AddLayer(Layer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			this.layers.Add(layer);
		}

		/// <summary>
		/// Get layer
		/// </summary>
		/// <param name="id">LayerId (defined in Constants)</param>
		/// <returns>Layer</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public Layer GetLayer(short id)
		{
			if(id < Constants.LAYER_MIN || id > Constants.LAYER_MAX)
			{
				throw new ArgumentOutOfRangeException("id");
			}
			return layers.SingleOrDefault(x => x.LayerID == id);
		}

		/// <summary>
		/// Saves the graphic into a file
		/// </summary>
		/// <param name="path">Path for the file</param>
		/// <param name="overwrite">If the file should be overwritten when existing</param>
		/// <returns>Returns true if succeeded, else false</returns>
		/// <exception cref="BahnEditor.BahnLib.ElementIsEmptyException"/>
		/// <exception cref="System.ArgumentNullException"/>
		public bool Save(string path, bool overwrite)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (File.Exists(path) && !overwrite)
			{
				return false;
			}
			else
			{
				using (FileStream stream = File.OpenWrite(path))
				{
					return Save(stream);
				}
			}
		}

		/// <summary>
		/// Checks if the graphic is empty (every pixel is transparent)
		/// </summary>
		/// <returns>True if graphic is empty, else false</returns>
		public bool IsElementEmpty()
		{
			foreach (var item in layers)
			{
				for (int i = 0; i < item.Element.GetLength(0); i++)
				{
					for (int j = 0; j < item.Element.GetLength(1); j++)
					{
						if (item.Element[i, j].IsTransparent == false)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// TODO write doc
		/// </summary>
		/// <returns>Element</returns>
		public Pixel[,] ElementPreview()
		{
			Pixel[,] element = new Pixel[Constants.SYMHOEHE, Constants.SYMBREITE];
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					element[i, j] = this.GetLayer(Constants.LAYER_VG).Element[i + Constants.SYMHOEHE, j + Constants.SYMBREITE];
				}
			}
			return element;
		}
		#endregion Public Methods

		#region Static Methods
		/// <summary>
		/// Loads a Graphic from a file
		/// </summary>
		/// <param name="path">Path to the file</param>
		/// <returns>Loaded Graphic</returns>
		/// <exception cref="System.IO.FileNotFoundException"/>
		/// <exception cref="System.IO.InvalidDataException"/>
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

		//TODO Reduce complexity (optional)
		// TODO Remove magic numbers
		internal static Graphic Load(Stream path)
		{
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				Graphic graphic = new Graphic();
				while (br.ReadByte() != 26) { }
				byte[] read = br.ReadBytes(3);
				if (read[0] != 71 || read[1] != 90 || read[2] != 71)
				{
					throw new InvalidDataException("wrong identification string");
				}

				byte zoomFactor = (byte)(br.ReadByte() - 48);
				switch (zoomFactor)
				{
					case 1:
					case 2:
					case 4:
						graphic.ZoomFactor = (ZoomFactor)Enum.Parse(typeof(ZoomFactor), zoomFactor.ToString(CultureInfo.InvariantCulture));
						break;
					default:
						throw new InvalidDataException("unknown zoom factor");
				}

				read = null;
				read = br.ReadBytes(4);
				if (read[0] != 0x03 || read[1] != 0x84 || read[2] != 0x00 || read[3] != 0x05)
				{
					throw new InvalidDataException("wrong version");
				}
				int p = br.ReadInt32();
				graphic.Properties = (GraphicProperties)Enum.Parse(typeof(GraphicProperties), p.ToString(CultureInfo.InvariantCulture));
				if (((graphic.Properties & GraphicProperties.Smoke) == GraphicProperties.Smoke) || ((graphic.Properties & GraphicProperties.Steam) == GraphicProperties.Steam))
				{
					graphic.SteamXPosition = br.ReadInt32();
					graphic.SteamYPosition = br.ReadInt32();
					graphic.SteamWidth = br.ReadInt32();
				}
				if ((graphic.Properties & GraphicProperties.Clock) == GraphicProperties.Clock)
				{
					br.ReadInt32(); //skipping unused data (for future use)
					int clockProperties = br.ReadInt32();
					graphic.ClockProperties = (ClockProperties)Enum.Parse(typeof(ClockProperties), clockProperties.ToString(CultureInfo.InvariantCulture));
					graphic.ClockXPosition = br.ReadInt32();
					graphic.ClockYPosition = br.ReadInt32();
					graphic.ClockZPosition = br.ReadInt32();
					graphic.ClockWidth = br.ReadInt32();
					graphic.ClockHeight = br.ReadInt32();
					graphic.ClockColorHoursPointer = Pixel.FromUInt(br.ReadUInt32());
					graphic.ClockColorMinutesPointer = Pixel.FromUInt(br.ReadUInt32());
					br.ReadInt32(); //skipping unused data (for future use)
				}
				if ((graphic.Properties & GraphicProperties.Cursor) == GraphicProperties.Cursor)
				{
					int cursorNormalDirection = br.ReadInt32();
					int cursorReverseDirection = br.ReadInt32();
					graphic.CursorNormalDirection = (CursorDirection)Enum.Parse(typeof(CursorDirection), cursorNormalDirection.ToString(CultureInfo.InvariantCulture));
					graphic.CursorReverseDirection = (CursorDirection)Enum.Parse(typeof(CursorDirection), cursorReverseDirection.ToString(CultureInfo.InvariantCulture));
				}
				if ((graphic.Properties & GraphicProperties.ColorSchematicMode) == GraphicProperties.ColorSchematicMode)
				{
					graphic.ColorInSchematicMode = Pixel.FromUInt(br.ReadUInt32());
					br.ReadUInt32();
				}
				if ((graphic.Properties & GraphicProperties.DrivingWay) == GraphicProperties.DrivingWay)
				{
					// TODO implement driving-way
					int count = br.ReadInt32();
					for (int i = 0; i < count; i++)
					{
						br.ReadInt32();
					}
				}
				short layer = br.ReadInt16();
				br.ReadUInt16();
				char c;
				StringBuilder sb = new StringBuilder();
				while ((c = br.ReadChar()) != Constants.UNICODE_NULL)
				{
					sb.Append(c);
				}
				graphic.InfoText = sb.ToString();


				for (int i = 0; i < layer; i++)
				{
					graphic.AddLayer(Layer.ReadLayerFromStream(br, graphic.ZoomFactor));
				}
				return graphic;
			}
		}
		#endregion Static Methods

		#region Internal Methods
		//TODO Remove magic numbers
		internal bool Save(Stream path)
		{
			if (this.IsElementEmpty())
				throw new ElementIsEmptyException("element is empty");

			BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode);
			int layer = this.layers.Count;
			bw.Write(Constants.HEADERTEXT.ToArray()); //Headertext 
			bw.Write((byte)26); // text end, defined in doc
			bw.Write(new byte[] { 71, 90, 71 }); //identification string GZG ASCII
			bw.Write((byte)(48 + this.ZoomFactor)); //Zoom faktor ASCII
			bw.Write((byte)0x03); //version
			bw.Write((byte)0x84); //version
			bw.Write((byte)0x00); //subversion
			bw.Write((byte)0x05); //subversion
			this.Properties |= GraphicProperties.ColorFormat24BPP;
			bw.Write((int)this.Properties); //Properties 
			if (((this.Properties & GraphicProperties.Smoke) == GraphicProperties.Smoke) || ((this.Properties & GraphicProperties.Steam) == GraphicProperties.Steam))
			{
				bw.Write(this.SteamXPosition);
				bw.Write(this.SteamYPosition);
				bw.Write(this.SteamWidth);
			}
			if ((this.Properties & GraphicProperties.Clock) == GraphicProperties.Clock)
			{
				bw.Write(1); //reserved for future use
				bw.Write((int)this.ClockProperties);
				bw.Write(this.ClockXPosition);
				bw.Write(this.ClockYPosition);
				bw.Write(this.ClockZPosition);
				bw.Write(this.ClockWidth);
				bw.Write(this.ClockHeight);
				bw.Write(this.ClockColorHoursPointer.ConvertToUInt());
				bw.Write(this.ClockColorMinutesPointer.ConvertToUInt());
				bw.Write(this.ClockColorHoursPointer.ConvertToUInt()); //reserved for future use
			}
			if ((this.Properties & GraphicProperties.Cursor) == GraphicProperties.Cursor)
			{
				bw.Write((int)this.CursorNormalDirection);
				bw.Write((int)this.CursorReverseDirection);
			}
			if ((this.Properties & GraphicProperties.ColorSchematicMode) == GraphicProperties.ColorSchematicMode)
			{
				bw.Write(this.ColorInSchematicMode.ConvertToUInt());
				bw.Write(Constants.FARBE_TRANSPARENT); //Not used, reserved for future use
			}
			bw.Write((short)layer); //layer
			bw.Write((ushort)0xFFFE);
			bw.Write(this.InfoText.ToCharArray());
			bw.Write(Constants.UNICODE_NULL);
			for (int i = 0; i < layer; i++)
			{
				this.layers[i].WriteLayerToStream(bw, this.ZoomFactor);
			}
			bw.Flush();
			return true;
		}
		#endregion Internal Methods
	}
}
