namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Constants used in BahnLib.
	/// </summary>
	public static class Constants
	{
		//TODO Write Doc for every constant
		//TODO Constants in english

		//Coordinates
		/// <summary>
		/// Basic element width in pixels for scale 1:1
		/// </summary>
		public const int ELEMENTWIDTH = 32;
		/// <summary>
		/// Basic element height in pixels for scale 1:1
		/// </summary>
		public const int ELEMENTHEIGHT = 16;

		//Variants
		public const int NO_ALTERNATIVE = 0;
		public const int MIN_ALTERNATIVE = 1;
		public const int MAX_ALTERNATIVE = 4;

		//Animations
		public const int MIN_ANIMATIONPHASE = 0;
		public const int MAX_ANIMATIONPHASE = 0;

		//File format versions
		public const ushort GRAPHIC_FILE_FORMAT = 0x8403;

		//Headertext
		public static System.Collections.ObjectModel.ReadOnlyCollection<byte> HEADERTEXT = new System.Collections.ObjectModel.ReadOnlyCollection<byte>(new System.Collections.Generic.List<byte>() { 67, 114, 101, 97, 116, 101, 100, 32, 98, 121, 32, 66, 97, 104, 110, 32, 71, 114, 97, 112, 104, 105, 99, 32, 69, 100, 105, 116, 111, 114, Constants.HEADERTEXT_TERMINATOR });
		public const byte HEADERTEXT_TERMINATOR = 26;

		//Unicode-Constants
		public const ushort UNICODE_NULL = 0x0000;

		//Colors
		public const uint COLOR_LOGIC = 0x80000000;
		public const uint COLOR_TRANSPARENT = (COLOR_LOGIC | 0x00000001);
		public const uint COLOR_LAMP = 0x50000000;
		public const uint COLOR_ADDITIONAL_DATA_MASK = 0xFf000000;
		public const uint COLOR_ALWAYSBRIGHT = 0x40000000;
		public const uint COLOR_WINDOW = 0x60000000;
		public const uint COLOR_WINDOW_0 = COLOR_WINDOW;
		public const uint COLOR_WINDOW_1 = (COLOR_WINDOW | 0x01000000);
		public const uint COLOR_WINDOW_2 = (COLOR_WINDOW | 0x02000000);

		public const uint COLOR_AS_MINIMUM = (COLOR_LOGIC | 0x00000100);
		public const uint COLOR_AS_MAXIMUM = (COLOR_LOGIC | 0x00000116);
		public const uint COLOR_COMPRESSED = (COLOR_LOGIC | 0x40000000);
		public const uint COLORMASK_COMPRESSED_TRANSPARENT = 0x00010000;
		public const uint COLOR_COMPRESSED_TRANSPARENT = (COLOR_COMPRESSED | COLORMASK_COMPRESSED_TRANSPARENT);
		public const uint COLORMASK_COMPRESSED_SYSTEMCOLOR = 0x00040000;
		public const uint COLOR_COMPRESSED_SYSTEMCOLOR = (COLOR_COMPRESSED | COLORMASK_COMPRESSED_SYSTEMCOLOR);
		public const uint COLORMASK_COMPRESSED_SFB = 0x0000ff00;
		public const uint COLORMASK_COMPRESSED_COUNT = 0x000000Ff;
		public const uint MAX_REPEATED_COLORS = 257;
		public const uint COLORMASK_COMPRESSED_LENGTH = 0x0000Ff00;
		public const uint MAX_REPEATED_COLORS_LENGTH = 4;

	}
}
