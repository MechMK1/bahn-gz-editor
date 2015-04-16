using System;
using System.Drawing;

namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Represents a pixel of a graphic used in BAHN
	/// </summary>
	public class Pixel
	{
		#region Fields and Properties
		private static Pixel transparentPixel = new Pixel(SpecialPixelWithoutRGB.Transparent);

		/// <summary>
		/// Gets or sets the red part of the pixel
		/// </summary>
		public byte Red { get; set; }

		/// <summary>
		/// Gets or sets the green part of the pixel
		/// </summary>
		public byte Green { get; set; }

		/// <summary>
		/// Gets or sets the blue part of the pixel
		/// </summary>
		public byte Blue { get; set; }

		/// <summary>
		/// Gets or sets if the pixel is transparent
		/// </summary>
		public bool IsTransparent
		{
			get
			{
				return this.SpecialColorWithoutRGB == SpecialPixelWithoutRGB.Transparent;
			}
			set
			{
				if (value)
					this.SpecialColorWithoutRGB = SpecialPixelWithoutRGB.Transparent;
				else
					this.SpecialColorWithoutRGB = SpecialPixelWithoutRGB.None;
			}
		}

		/// <summary>
		/// Gets if the pixel is a special color without own RGB
		/// </summary>
		public bool IsSpecialColorWithoutRGB
		{
			get
			{
				return SpecialColorWithoutRGB != SpecialPixelWithoutRGB.None;
			}
		}

		/// <summary>
		/// Gets if the pixel is a special color with own RGB
		/// </summary>
		public bool IsSpecialColorWithRGB
		{
			get
			{
				return SpecialColorWithRGB != SpecialPixelWithRGB.None;
			}
		}

		/// <summary>
		/// Gets the SpecialColorWithoutRGB of the pixel
		/// </summary>
		public SpecialPixelWithoutRGB SpecialColorWithoutRGB { get; private set; }

		/// <summary>
		/// Gets the SpecialColorWithRGB of the pixel
		/// </summary>
		public SpecialPixelWithRGB SpecialColorWithRGB { get; private set; }
		#endregion Fields and Properties

		#region Constructor
		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.Pixel class
		/// </summary>
		/// <param name="red">Red part of the pixel</param>
		/// <param name="green">Green part of the pixel</param>
		/// <param name="blue">Blue part of the pixel</param>
		public Pixel(byte red, byte green, byte blue)
		{
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
			this.SpecialColorWithRGB = SpecialPixelWithRGB.None;
			this.SpecialColorWithoutRGB = SpecialPixelWithoutRGB.None;
		}

		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.Pixel class
		/// </summary>
		/// <param name="specialColor">Enum of different configurable colors</param>
		public Pixel(SpecialPixelWithoutRGB specialColor)
		{
			this.SpecialColorWithoutRGB = specialColor;
			this.SpecialColorWithRGB = SpecialPixelWithRGB.None;
		}

		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.Pixel class
		/// </summary>
		/// <param name="specialColor">Enum of different special colors</param>
		/// <param name="red">Red part of the pixel</param>
		/// <param name="green">Green part of the pixel</param>
		/// <param name="blue">Blue part of the pixel</param>
		public Pixel(SpecialPixelWithRGB specialColor, byte red, byte green, byte blue)
			: this(red, green, blue)
		{
			this.SpecialColorWithRGB = specialColor;
		}
		#endregion Constructor

		#region Public Methods
		//HACK Convert what to uint
		/// <summary>
		/// Converts the pixel to uint
		/// </summary>
		/// <returns>Returns the data in format RGB with special data in the last two bits.</returns>
		public uint ToUInt()
		{
			if (this.IsTransparent)
			{
				return Constants.COLOR_TRANSPARENT;
			}
			else if (this.IsSpecialColorWithoutRGB)
			{
				return (uint)this.SpecialColorWithoutRGB;
			}
			else
			{
				uint result = 0;
				result =
					(
						(
							(uint)
							(
								(Blue)
								|
								(
									(Green) << 8
								)
							)
							|
							(
								(
									(uint)(Red)
								) << 16
							)
						)
					);
				if (this.IsSpecialColorWithRGB)
				{
					result = result | (uint)this.SpecialColorWithRGB;
				}
				return result;
			}
		}

		/// <summary>
		/// Converts the pixel to System.Drawing.Color
		/// </summary>
		/// <returns>Color</returns>
		public Color ToColor()
		{
			switch (this.SpecialColorWithoutRGB)
			{
				case SpecialPixelWithoutRGB.None:
					return Color.FromArgb(this.Red, this.Green, this.Blue);
				case SpecialPixelWithoutRGB.Transparent:
					return Color.FromArgb(0, 112, 0);
				case SpecialPixelWithoutRGB.BehindGlass:
					return Color.FromArgb(0, 50, 100);
				case SpecialPixelWithoutRGB.As_BG:
					return Color.FromArgb(0, 112, 0);
				case SpecialPixelWithoutRGB.As_Sleepers0:
					return Color.FromArgb(188, 188, 188);
				case SpecialPixelWithoutRGB.As_Sleepers1:
					return Color.FromArgb(84, 40, 0);
				case SpecialPixelWithoutRGB.As_Sleepers3:
					return Color.FromArgb(84, 40, 0);
				case SpecialPixelWithoutRGB.As_Rails_Road0:
					return Color.FromArgb(168, 168, 168);
				case SpecialPixelWithoutRGB.As_Rails_Road1:
					return Color.FromArgb(60, 60, 60);
				case SpecialPixelWithoutRGB.As_Rails_Road2:
					return Color.FromArgb(168, 168, 168);
				case SpecialPixelWithoutRGB.As_Rails_Road3:
					return Color.FromArgb(104, 104, 104);
				case SpecialPixelWithoutRGB.As_Rails_Trackbed0:
					return Color.FromArgb(104, 104, 104);
				case SpecialPixelWithoutRGB.As_Rails_Trackbed1:
					return Color.FromArgb(148, 148, 148);
				case SpecialPixelWithoutRGB.As_Rails_Trackbed2:
					return Color.FromArgb(148, 148, 148);
				case SpecialPixelWithoutRGB.As_Rails_Trackbed3:
					return Color.FromArgb(104, 104, 104);
				case SpecialPixelWithoutRGB.As_Marking_Point_Bus0:
					return Color.FromArgb(252, 252, 252);
				case SpecialPixelWithoutRGB.As_Marking_Point_Bus1:
					return Color.FromArgb(252, 252, 252);
				case SpecialPixelWithoutRGB.As_Marking_Point_Bus2:
					return Color.FromArgb(252, 252, 252);
				case SpecialPixelWithoutRGB.As_Marking_Point_Bus3:
					return Color.FromArgb(252, 252, 252);
				case SpecialPixelWithoutRGB.As_Marking_Point_Water:
					return Color.FromArgb(84, 252, 252);
				case SpecialPixelWithoutRGB.As_Gravel:
					return Color.FromArgb(60, 60, 60);
				case SpecialPixelWithoutRGB.As_Small_Gravel:
					return Color.FromArgb(168, 136, 0);
				case SpecialPixelWithoutRGB.As_Grassy:
					return Color.FromArgb(0, 168, 0);
				case SpecialPixelWithoutRGB.As_Path_BG:
					return Color.FromArgb(30, 180, 20);
				case SpecialPixelWithoutRGB.As_Path_FG:
					return Color.FromArgb(168, 140, 0);
				case SpecialPixelWithoutRGB.As_Text:
					return Color.FromArgb(252, 252, 252);
				default:
					throw new Exception("WTF");
			}
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current pixel.
		/// </summary>
		/// <param name="obj">The object to compare with the current pixel.</param>
		/// <returns>
		/// Returns true if the specified object is equal to the current pixel.
		/// <para>Returns false if the specified object is not a pixel or is null.</para>
		/// </returns>
		public override bool Equals(object obj)
		{
			Pixel p = obj as Pixel;
			if (p != null)
			{
				if
				(
					(
						this.SpecialColorWithoutRGB != SpecialPixelWithoutRGB.None
						&&
						p.SpecialColorWithoutRGB != SpecialPixelWithoutRGB.None
						&&
						this.SpecialColorWithoutRGB == p.SpecialColorWithoutRGB
					)
					||
					(
						this.Red == p.Red
						&&
						this.Green == p.Green
						&&
						this.Blue == p.Blue
						&&
						this.SpecialColorWithoutRGB == SpecialPixelWithoutRGB.None
						&&
						p.SpecialColorWithoutRGB == SpecialPixelWithoutRGB.None
					)
					||
					(
						this.IsSpecialColorWithRGB
						&&
						p.IsSpecialColorWithRGB
						&&
						this.Red == p.Red
						&&
						this.Green == p.Green
						&&
						this.Blue == p.Blue
						&&
						this.SpecialColorWithoutRGB == SpecialPixelWithoutRGB.None
						&&
						p.SpecialColorWithoutRGB == SpecialPixelWithoutRGB.None
					)
				)
				{
					return true;
				}
			}
			return false;
		}

		#endregion Public Methods

		#region Static Methods
		/// <summary>
		/// Gets a transparent pixel
		/// </summary>
		/// <returns>transparent pixel</returns>
		public static Pixel TransparentPixel()
		{
			return transparentPixel;
		}

		/// <summary>
		/// Gets a pixel from uint
		/// </summary>
		/// <param name="data">Data in format RGB with special data in last two bits</param>
		/// <returns>Pixel</returns>
		public static Pixel FromUInt(uint data) //TODO Remove magic numbers
		{
			if ((data & Constants.COLOR_LOGIC) != 0)
			{
				return new Pixel((SpecialPixelWithoutRGB)data);
			}
			else if (((data & Constants.COLOR_LAMP) != 0) || ((data & Constants.COLOR_ALWAYSBRIGHT) != 0) || ((data & Constants.COLOR_WINDOW) != 0))
			{
				return new Pixel((SpecialPixelWithRGB)(data & 0xFF000000), blue:(byte)data, green:(byte)(data >> 8), red:(byte)(data >> 16));
			}
			else
			{
				return new Pixel(blue: (byte)data,green:(byte)(data >> 8),red:(byte)(data >> 16));
			}
		}

		/// <summary>
		/// Gets a pixel from System.Drawing.Color
		/// </summary>
		/// <param name="color">Color</param>
		/// <returns>Pixel</returns>
		public static Pixel FromColor(Color color)
		{
			return new Pixel(color.R, color.G, color.B);
		}
		#endregion Static Methods

		#region Enums
		/// <summary>
		/// Enum of Special Pixel Without RGB (Configurable colors)
		/// </summary>
		public enum SpecialPixelWithoutRGB : uint
		{ //Magic numbers copied from doc: http://www.jbss.de/exe/gfx_386.zip
			None = 0,
			Transparent = (Constants.COLOR_LOGIC | 0x00000001),
			BehindGlass = (Constants.COLOR_LOGIC),
			As_BG = (Constants.COLOR_LOGIC | 0x00000100),
			As_Sleepers0 = (Constants.COLOR_LOGIC | 0x00000101),
			As_Sleepers1 = (Constants.COLOR_LOGIC | 0x00000102),
			As_Sleepers3 = (Constants.COLOR_LOGIC | 0x00000103),
			As_Rails_Road0 = (Constants.COLOR_LOGIC | 0x00000104),
			As_Rails_Road1 = (Constants.COLOR_LOGIC | 0x00000105),
			As_Rails_Road2 = (Constants.COLOR_LOGIC | 0x00000106),
			As_Rails_Road3 = (Constants.COLOR_LOGIC | 0x00000107),
			As_Rails_Trackbed0 = (Constants.COLOR_LOGIC | 0x00000108),
			As_Rails_Trackbed1 = (Constants.COLOR_LOGIC | 0x00000109),
			As_Rails_Trackbed2 = (Constants.COLOR_LOGIC | 0x0000010A),
			As_Rails_Trackbed3 = (Constants.COLOR_LOGIC | 0x0000010B),
			As_Marking_Point_Bus0 = (Constants.COLOR_LOGIC | 0x0000010C),
			As_Marking_Point_Bus1 = (Constants.COLOR_LOGIC | 0x0000010D),
			As_Marking_Point_Bus2 = (Constants.COLOR_LOGIC | 0x0000010E),
			As_Marking_Point_Bus3 = (Constants.COLOR_LOGIC | 0x0000010F),
			As_Marking_Point_Water = (Constants.COLOR_LOGIC | 0x00000110),
			As_Gravel = (Constants.COLOR_LOGIC | 0x00000111),
			As_Small_Gravel = (Constants.COLOR_LOGIC | 0x00000112),
			As_Grassy = (Constants.COLOR_LOGIC | 0x00000113),
			As_Path_BG = (Constants.COLOR_LOGIC | 0x00000114),
			As_Path_FG = (Constants.COLOR_LOGIC | 0x00000115),
			As_Text = (Constants.COLOR_LOGIC | 0x00000116)
		}

		/// <summary>
		/// Enum of Special Pixel With RGB (i.e. lamps or pixel with different color at night)
		/// </summary>
		public enum SpecialPixelWithRGB : uint
		{ //Magic numbers copied from doc: http://www.jbss.de/exe/gfx_386.zip
			None = 0,
			Always_Bright = Constants.COLOR_ALWAYSBRIGHT,
			Lamp_Yellow = (Constants.COLOR_LAMP | 0x00000000),
			Lamp_Red = (Constants.COLOR_LAMP | 0x01000000),
			Lamp_ColdWhite = (Constants.COLOR_LAMP | 0x02000000),
			Lamp_YellowWhite = (Constants.COLOR_LAMP | 0x03000000),
			Lamp_Gas_Yellow = (Constants.COLOR_LAMP | 0x04000000),
			Window_Yellow_0 = Constants.COLOR_WINDOW_0,
			Window_Yellow_1 = Constants.COLOR_WINDOW_1,
			Window_Yellow_2 = Constants.COLOR_WINDOW_2,
			Window_Neon_0 = (Constants.COLOR_WINDOW_0 | 0x04000000),
			Window_Neon_1 = (Constants.COLOR_WINDOW_1 | 0x04000000),
			Window_Neon_2 = (Constants.COLOR_WINDOW_2 | 0x04000000)
		}
		#endregion Enums
	}
}
