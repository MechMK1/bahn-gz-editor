﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Represents a graphic used in BAHN
	/// </summary>
	//public class Graphic
	//{
	//	#region Fields and Properties
	//	private Dictionary<LayerId, Layer> layers;
	//	private int layercount;

	//	/// <summary>
	//	/// Gets the zoom factor
	//	/// </summary>
	//	public ZoomFactor ZoomFactor { get; private set; }

	//	/// <summary>
	//	/// Gets or sets the info-text
	//	/// </summary>
	//	public string InfoText { get; set; }

	//	/// <summary>
	//	/// Gets or sets the color in map / schematic view
	//	/// <para>Makes only sense if ColorSchematicMode is set in properties, else data is ignored</para>
	//	/// </summary>
	//	//public Pixel ColorInSchematicMode { get; set; }

	//	/// <summary>
	//	/// Gets or sets the properties of the graphic
	//	/// </summary>
	//	public GraphicProperties Properties { get; set; }

	//	/// <summary>
	//	/// Gets or sets the x position where steam/smoke begins
	//	/// <para>Makes only sense if steam or smoke is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int SteamXPosition { get; set; }

	//	/// <summary>
	//	/// Gets or sets the y position where steam/smoke begins
	//	/// <para>Makes only sense if steam or smoke is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int SteamYPosition { get; set; }

	//	/// <summary>
	//	/// Gets or sets the width (length) of steam/smoke
	//	/// <para>Makes only sense if steam or smoke is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int SteamWidth { get; set; }

	//	/// <summary>
	//	/// Gets or sets the properties of the clock
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public ClockProperties ClockProperties { get; set; }

	//	/// <summary>
	//	/// Gets or sets the x position of the center of the clock
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int ClockXPosition { get; set; }

	//	/// <summary>
	//	/// Gets or sets the y position of the center of the clock
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int ClockYPosition { get; set; }

	//	/// <summary>
	//	/// Gets or sets the z position (layer) of the center of the clock
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public LayerId ClockZPosition { get; set; }

	//	/// <summary>
	//	/// Gets or sets the width of the clock
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int ClockWidth { get; set; }

	//	/// <summary>
	//	/// Gets or sets the height of the clock
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public int ClockHeight { get; set; }

	//	/// <summary>
	//	/// Gets or sets the color of the hours pointer
	//	/// <para>Colors that light at night are possible</para>
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	//public Pixel ClockColorHoursPointer { get; set; }

	//	/// <summary>
	//	/// Gets or sets the color of the minutes pointer
	//	/// <para>Makes only sense if clock is set in properties, else data is ignored</para>
	//	/// </summary>
	//	//public Pixel ClockColorMinutesPointer { get; set; }

	//	/// <summary>
	//	/// Gets or sets the normal cursor direction
	//	/// <para>Makes only sense if cursor is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public Direction CursorNormalDirection { get; set; }

	//	/// <summary>
	//	/// Gets or sets the reverse cursor direction
	//	/// <para>Mostly, this is the opposite of CursorNormalDirection</para>
	//	/// <para>Makes only sense if cursor is set in properties, else data is ignored</para>
	//	/// </summary>
	//	public Direction CursorReverseDirection { get; set; }

	//	/// <summary>
	//	/// Gets or sets the version of the graphic
	//	/// </summary>
	//	public GraphicVersion Version { get; set; }

	//	/// <summary>
	//	/// Gets the list of driving ways
	//	/// </summary>
	//	public List<DrivingWayElement> DrivingWay { get; private set; }
	//	#endregion Fields and Properties

	//	#region Constructors
	//	/// <summary>
	//	/// Initializes a new Instance of BahnEditor.BahnLib.Graphic class
	//	/// </summary>
	//	/// <param name="infoText">InfoText</param>
	//	/// <param name="zoomFactor">Zoom factor</param>
	//	/// <param name="version">Version</param>
	//	public Graphic(string infoText, ZoomFactor zoomFactor = ZoomFactor.Zoom1, GraphicVersion version = GraphicVersion.Version2)
	//		: this()
	//	{
	//		this.Version = version;
	//		this.ZoomFactor = zoomFactor;
	//		this.InfoText = infoText;

	//	}

	//	/// <summary>
	//	/// Initializes a new Instance of BahnEditor.BahnLib.Graphic class, used in Load-Function
	//	/// </summary>
	//	private Graphic()
	//	{
	//		this.layers = new Dictionary<LayerId, Layer>();
	//		this.DrivingWay = new List<DrivingWayElement>();
	//	}
	//	#endregion Constructors

	//	#region Public Methods

	//	/// <summary>
	//	/// Adds a transparent layer (every pixel of layer is transparent) to the graphic
	//	/// </summary>
	//	/// <param name="layerID">LayerId</param>
	//	public void AddTransparentLayer(LayerId layerID)
	//	{
	//		uint[,] element = new uint[Constants.ElementHeight * 8 * (byte)this.ZoomFactor, Constants.ElementWidth * 3 * (byte)this.ZoomFactor];
	//		for (int i = 0; i < element.GetLength(0); i++)
	//		{
	//			for (int j = 0; j < element.GetLength(1); j++)
	//			{
	//				element[i, j] = Constants.ColorTransparent;
	//			}
	//		}
	//		Layer layer = new Layer(layerID, element);
	//		this.AddLayer(layerID, layer);
	//	}

	//	/// <summary>
	//	/// Adds a layer to the graphic
	//	/// </summary>
	//	/// <param name="layer">Layer</param>
	//	/// <exception cref="System.ArgumentNullException"/>
	//	public void AddLayer(LayerId layerId, Layer layer)
	//	{
	//		if (layer == null)
	//		{
	//			throw new ArgumentNullException("layer");
	//		}
	//		this.layers.Add(layerId, layer);
	//	}

	//	/// <summary>
	//	/// Gets a layer of the graphic
	//	/// </summary>
	//	/// <param name="id">LayerId</param>
	//	/// <returns>Layer</returns>
	//	public Layer GetLayer(LayerId id)
	//	{
	//		return layers[id];
	//	}

	//	/// <summary>
	//	/// Saves the graphic into a file
	//	/// </summary>
	//	/// <param name="path">Path for the file</param>
	//	/// <param name="overwrite">If the file should be overwritten when existing</param>
	//	/// <returns>Returns true if succeeded, else false</returns>
	//	/// <exception cref="BahnEditor.BahnLib.ElementIsEmptyException"/>
	//	/// <exception cref="System.ArgumentNullException"/>
	//	public bool Save(string path, bool overwrite)
	//	{
	//		if (path == null)
	//			throw new ArgumentNullException("path");
	//		if (File.Exists(path) && !overwrite)
	//		{
	//			return false;
	//		}
	//		else
	//		{
	//			using (FileStream stream = File.OpenWrite(path))
	//			{
	//				return Save(stream);
	//			}
	//		}
	//	}

	//	/// <summary>
	//	/// Checks if the graphic is empty (every pixel is transparent)
	//	/// </summary>
	//	/// <returns>True if graphic is empty, else false</returns>
	//	public bool IsElementEmpty()
	//	{
	//		foreach (var item in layers)
	//		{
	//			for (int i = 0; i < item.Value.Element.GetLength(0); i++)
	//			{
	//				for (int j = 0; j < item.Value.Element.GetLength(1); j++)
	//				{
	//					if ((item.Value.Element[i, j] & Constants.ColorTransparent) != Constants.ColorTransparent)
	//					{
	//						return false;
	//					}
	//				}
	//			}
	//		}
	//		return true;
	//	}

	//	/// <summary>
	//	/// Gets a preview of the graphic how it would be in the game
	//	/// </summary>
	//	/// <returns>Element</returns>
	//	public uint[,] ElementPreview()
	//	{
	//		uint[,] element = new uint[Constants.ElementHeight, Constants.ElementWidth];
	//		for (int i = 0; i < element.GetLength(0); i++)
	//		{
	//			for (int j = 0; j < element.GetLength(1); j++)
	//			{
	//				// HACK Is transparent
	//				//if (this.GetLayer(LayerId.ForegroundAbove) != null && !this.GetLayer(LayerId.ForegroundAbove).Element[i + Constants.ElementHeight, j + Constants.ElementWidth].IsTransparent)
	//				//	element[i, j] = this.GetLayer(LayerId.ForegroundAbove).Element[i + Constants.ElementHeight, j + Constants.ElementWidth];
	//				//else if (this.GetLayer(LayerId.Foreground) != null && !this.GetLayer(LayerId.Foreground).Element[i + Constants.ElementHeight, j + Constants.ElementWidth].IsTransparent)
	//				//	element[i, j] = this.GetLayer(LayerId.Foreground).Element[i + Constants.ElementHeight, j + Constants.ElementWidth];
	//				//else if (this.GetLayer(LayerId.Front) != null && !this.GetLayer(LayerId.Front).Element[i + Constants.ElementHeight, j + Constants.ElementWidth].IsTransparent)
	//				//	element[i, j] = this.GetLayer(LayerId.Front).Element[i + Constants.ElementHeight, j + Constants.ElementWidth];
	//				//else if (this.GetLayer(LayerId.Background1) != null && !this.GetLayer(LayerId.Background1).Element[i + Constants.ElementHeight, j + Constants.ElementWidth].IsTransparent)
	//				//	element[i, j] = this.GetLayer(LayerId.Background1).Element[i + Constants.ElementHeight, j + Constants.ElementWidth];
	//				//else if (this.GetLayer(LayerId.Background0) != null && !this.GetLayer(LayerId.Background0).Element[i + Constants.ElementHeight, j + Constants.ElementWidth].IsTransparent)
	//				//	element[i, j] = this.GetLayer(LayerId.Background0).Element[i + Constants.ElementHeight, j + Constants.ElementWidth];
	//				//else
	//				//	element[i, j] = new Pixel(0, 0, 0, Pixel.PixelProperty.Transparent);
	//			}
	//		}
	//		return element;
	//	}
	//	#endregion Public Methods

	//	#region Static Methods
	//	/// <summary>
	//	/// Loads a Graphic from a file
	//	/// </summary>
	//	/// <param name="path">Path to the file</param>
	//	/// <returns>Loaded Graphic</returns>
	//	/// <exception cref="System.IO.FileNotFoundException"/>
	//	/// <exception cref="System.IO.InvalidDataException"/>
	//	public static Graphic Load(string path)
	//	{
	//		if (File.Exists(path))
	//		{
	//			using (FileStream stream = File.OpenRead(path))
	//			{
	//				return Load(stream);
	//			}
	//		}
	//		else throw new FileNotFoundException("File not found", path);
	//	}


	//	internal static Graphic Load(Stream path)
	//	{
	//		using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
	//		{
	//			Graphic graphic = LoadHeader(br);
	//			graphic.LoadData(br);
	//			return graphic;
	//		}
	//	}

	//	//TODO Reduce complexity (optional)
	//	//TODO Remove magic numbers
	//	internal static Graphic LoadHeader(BinaryReader br)
	//	{
	//		Graphic graphic = new Graphic();
	//		while (br.ReadByte() != Constants.HeaderTextTerminator) { } //Headertext, can be skipped
	//		byte[] readIdentification = br.ReadBytes(3); //Read indentification string
	//		if (readIdentification[0] != 71 || readIdentification[1] != 90 || readIdentification[2] != 71) //identification string "GZG" ASCII-format
	//		{
	//			throw new InvalidDataException("wrong identification string");
	//		}

	//		byte zoomFactor = (byte)(br.ReadByte() - 48); //Zoomfactor is read as ASCII-character, then "converted" to a byte
	//		//ASCII 0 => 48, ASCII 1 => 49, etc...
	//		switch (zoomFactor)
	//		{
	//			case 1:
	//			case 2:
	//			case 4:
	//				graphic.ZoomFactor = (ZoomFactor)zoomFactor; //HACK Could be shit. Who knows?
	//				break;
	//			default:
	//				throw new InvalidDataException("unknown zoom factor");
	//		}
	//		ushort readVersion = br.ReadUInt16();
	//		byte[] readSubversion = br.ReadBytes(2);
	//		if (readVersion != Constants.GraphicFileFormat || readSubversion[0] != 0x00)
	//		{
	//			throw new InvalidDataException("wrong version");
	//		}
	//		switch (readSubversion[1])
	//		{
	//			case (byte)GraphicVersion.Version0:
	//			case (byte)GraphicVersion.Version1:
	//			case (byte)GraphicVersion.Version2:
	//				graphic.Version = (GraphicVersion)readSubversion[1];
	//				break;
	//			default:
	//				throw new InvalidDataException("wrong version");
	//		}

	//		int p = br.ReadInt32(); //Properties
	//		graphic.Properties = (GraphicProperties)p;
	//		if (((graphic.Properties & GraphicProperties.Smoke) == GraphicProperties.Smoke) || ((graphic.Properties & GraphicProperties.Steam) == GraphicProperties.Steam))
	//		{
	//			graphic.SteamXPosition = br.ReadInt32();
	//			graphic.SteamYPosition = br.ReadInt32();
	//			graphic.SteamWidth = br.ReadInt32();
	//		}
	//		if ((graphic.Properties & GraphicProperties.Clock) == GraphicProperties.Clock)
	//		{
	//			if (graphic.Version == GraphicVersion.Version0)
	//			{
	//				throw new InvalidDataException("Clock is set, but invalid for the version of the graphic");
	//			}
	//			br.ReadInt32(); //skipping unused data (for future use)
	//			int clockProperties = br.ReadInt32();
	//			graphic.ClockProperties = (ClockProperties)clockProperties;
	//			graphic.ClockXPosition = br.ReadInt32();
	//			graphic.ClockYPosition = br.ReadInt32();
	//			int clockZPosition = br.ReadInt32();
	//			graphic.ClockZPosition = (LayerId)clockZPosition;
	//			graphic.ClockWidth = br.ReadInt32();
	//			graphic.ClockHeight = br.ReadInt32();
	//			uint hours = br.ReadUInt32();
	//			uint minutes = br.ReadUInt32();
	//			//HACK Fail
	//			//graphic.ClockColorHoursPointer = Pixel.FromUInt(br.ReadUInt32());
	//			//graphic.ClockColorMinutesPointer = Pixel.FromUInt(br.ReadUInt32());
	//			br.ReadInt32(); //skipping unused data (for future use)
	//		}
	//		if ((graphic.Properties & GraphicProperties.Cursor) == GraphicProperties.Cursor)
	//		{
	//			if (graphic.Version < GraphicVersion.Version2)
	//			{
	//				throw new InvalidDataException("Cursor is set, but invalid for the version of the graphic");
	//			}
	//			int cursorNormalDirection = br.ReadInt32();
	//			int cursorReverseDirection = br.ReadInt32();
	//			graphic.CursorNormalDirection = (Direction)cursorNormalDirection;
	//			graphic.CursorReverseDirection = (Direction)cursorReverseDirection;
	//		}
	//		if ((graphic.Properties & GraphicProperties.ColorSchematicMode) == GraphicProperties.ColorSchematicMode)
	//		{
	//			if (graphic.Version < GraphicVersion.Version2)
	//			{
	//				throw new InvalidDataException("ColorSchematicMode is set, but invalid for the version of the graphic");
	//			}
	//			uint schematicmode = br.ReadUInt32();
	//			// HACK Fail
	//			//graphic.ColorInSchematicMode = Pixel.FromUInt(br.ReadUInt32());
	//			br.ReadUInt32(); //skipping unused data
	//		}
	//		if ((graphic.Properties & GraphicProperties.DrivingWay) == GraphicProperties.DrivingWay)
	//		{
	//			if (graphic.Version < GraphicVersion.Version2)
	//				throw new InvalidDataException("DrivingWay is set, but invalid for the version of the graphic");
	//			graphic.DrivingWay = new List<DrivingWayElement>();
	//			int count = br.ReadInt32();
	//			for (int i = 0; i < count; i++)
	//			{
	//				graphic.DrivingWay.Add(DrivingWayElement.FromBytes(br.ReadBytes(12)));
	//			}
	//		}
	//		graphic.layercount = br.ReadInt16();
	//		br.ReadUInt16(); //skipping unknown data, see more in save-method
	//		char c;
	//		StringBuilder sb = new StringBuilder();
	//		while ((c = br.ReadChar()) != Constants.UnicodeNull)
	//		{
	//			sb.Append(c);
	//		}
	//		graphic.InfoText = sb.ToString();
	//		return graphic;
	//	}
	//	#endregion Static Methods

	//	#region Internal Methods
	//	//TODO Remove magic numbers
	//	internal bool Save(Stream path)
	//	{
	//		if (this.IsElementEmpty())
	//			throw new ElementIsEmptyException("element is empty");

	//		BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode);
	//		int layer = this.layers.Count;
	//		bw.Write(Constants.HeaderText.ToArray()); //Headertext 
	//		bw.Write(new byte[] { 71, 90, 71 }); //identification string "GZG" ASCII-format
	//		bw.Write((byte)(48 + this.ZoomFactor)); //Zoomfactor, as ASCII-format
	//		bw.Write(Constants.GraphicFileFormat); //version
	//		bw.Write((byte)0); //subversion, normally a byte[2] array, but first element of the array is empty
	//		bw.Write((byte)this.Version); //subversion
	//		if (this.Version >= GraphicVersion.Version2 && ((this.Properties & GraphicProperties.ColorFormat24BPP) != GraphicProperties.ColorFormat24BPP))
	//			this.Properties |= GraphicProperties.ColorFormat24BPP;
	//		bw.Write((int)this.Properties); //Properties 
	//		if (((this.Properties & GraphicProperties.Smoke) == GraphicProperties.Smoke) || ((this.Properties & GraphicProperties.Steam) == GraphicProperties.Steam))
	//		{
	//			bw.Write(this.SteamXPosition);
	//			bw.Write(this.SteamYPosition);
	//			bw.Write(this.SteamWidth);
	//		}
	//		if ((this.Properties & GraphicProperties.Clock) == GraphicProperties.Clock)
	//		{
	//			bw.Write(1); //reserved for future use
	//			bw.Write((int)this.ClockProperties);
	//			bw.Write(this.ClockXPosition);
	//			bw.Write(this.ClockYPosition);
	//			bw.Write((int)this.ClockZPosition);
	//			bw.Write(this.ClockWidth);
	//			bw.Write(this.ClockHeight);
	//			// HACK Fail
	//			//bw.Write(this.ClockColorHoursPointer.ToUInt());
	//			//bw.Write(this.ClockColorMinutesPointer.ToUInt());
	//			//bw.Write(this.ClockColorHoursPointer.ToUInt()); //reserved for future use
	//		}
	//		if ((this.Properties & GraphicProperties.Cursor) == GraphicProperties.Cursor)
	//		{
	//			bw.Write((int)this.CursorNormalDirection);
	//			bw.Write((int)this.CursorReverseDirection);
	//		}
	//		if ((this.Properties & GraphicProperties.ColorSchematicMode) == GraphicProperties.ColorSchematicMode)
	//		{
	//			//HACK FAIL
	//			//bw.Write(this.ColorInSchematicMode.ToUInt());
	//			bw.Write(Constants.ColorTransparent); //Not used, reserved for future use
	//		}
	//		if ((this.Properties & GraphicProperties.DrivingWay) == GraphicProperties.DrivingWay)
	//		{
	//			bw.Write(this.DrivingWay.Count);
	//			foreach (var item in this.DrivingWay)
	//			{
	//				bw.Write(item.ToBytes());
	//			}
	//		}
	//		bw.Write((short)layer); //layer
	//		bw.Write((ushort)0xFFFE); //unknown, got data through analysis of existing files, doesn't work without
	//		bw.Write(this.InfoText.ToCharArray());
	//		bw.Write(Constants.UnicodeNull);
	//		foreach (var item in this.layers)
	//		{
	//			item.Value.WriteLayerToStream(bw, this.ZoomFactor);
	//		}
	//		bw.Flush();
	//		return true;
	//	}

	//	internal void LoadData(BinaryReader br)
	//	{
	//		if (br == null)
	//			throw new ArgumentNullException();
	//		bool backgroundLayerExists = false;
	//		for (int i = 0; i < this.layercount; i++)
	//		{
	//			Layer l = Layer.ReadLayerFromStream(br, this.ZoomFactor, this.Version);
	//			if (l.LayerId == LayerId.Background0)
	//			{
	//				if (!backgroundLayerExists)
	//				{
	//					backgroundLayerExists = true;
	//				}
	//				else
	//				{
	//					l.LayerId = LayerId.Background1;
	//				}
	//			}
	//			this.AddLayer(l.LayerId, l);
	//		}
	//	}
	//	#endregion Internal Methods

	//	#region Helper
	//	internal bool IsLayerEmpty
	//	{
	//		get
	//		{
	//			return this.layers.Count == 0;
	//		}
	//	}
	//	#endregion Helper
	//}
}
