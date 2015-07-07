namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Constants used in BahnLib.
	/// </summary>
	public static class Constants
	{
		//TODO Write Doc for every constant

		//Coordinates
		/// <summary>
		/// Basic element width in pixels for scale 1:1
		/// </summary>
		public const int ElementWidth = 32;
		/// <summary>
		/// Basic element height in pixels for scale 1:1
		/// </summary>
		public const int ElementHeight = 16;

		//Elements in Archive
		public const int MaxElementsInArchive = 89;

		//Variants
		public const int NoAlternative = 0;
		public const int MinAlternative = 1;
		public const int MaxAlternative = 4;

		//Animations
		public const int MinAnimationPhase = 0;
		public const int MaxAnimationPhase = 99;

		//File format versions
		public const ushort GraphicFileFormat = 0x8403;

		//Headertext
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")] //Variable is actually read-only
		public static System.Collections.ObjectModel.ReadOnlyCollection<byte> HeaderText = new System.Collections.ObjectModel.ReadOnlyCollection<byte>(new System.Collections.Generic.List<byte>() { 67, 114, 101, 97, 116, 101, 100, 32, 98, 121, 32, 66, 97, 104, 110, 32, 71, 114, 97, 112, 104, 105, 99, 32, 69, 100, 105, 116, 111, 114, Constants.HeaderTextTerminator });
		public const byte HeaderTextTerminator = 26;

		//Unicode-Constants
		public const ushort UnicodeNull = 0x0000;

		//Colors
		public const uint ColorLogic = 0x80000000;
		public const uint ColorTransparent = (ColorLogic | 0x00000001);
		public const uint ColorLamp = 0x50000000;
		public const uint ColorAdditionalDataMask = 0xFf000000;
		public const uint ColorAlwaysBright = 0x40000000;
		public const uint ColorWindow = 0x60000000;
		public const uint ColorWindow0 = ColorWindow;
		public const uint ColorWindow1 = (ColorWindow | 0x01000000);
		public const uint ColorWindow2 = (ColorWindow | 0x02000000);

		public const uint ColorAsMin = (ColorLogic | 0x00000100);
		public const uint ColorAsMax = (ColorLogic | 0x00000116);
		public const uint ColorCompressed = (ColorLogic | 0x40000000);
		public const uint ColorMaskCompressedTransparent = 0x00010000;
		public const uint ColorCompressedTransparent = (ColorCompressed | ColorMaskCompressedTransparent);
		public const uint ColorMaskCompressedSystemcolor = 0x00040000;
		public const uint ColorCompressedSystemcolor = (ColorCompressed | ColorMaskCompressedSystemcolor);
		public const uint ColorMaskCompressedSFB = 0x0000ff00;
		public const uint ColorMaskCompressedCount = 0x000000Ff;
		public const uint MaxRepeatedColors = 257;
		public const uint ColorMaskCompressedLength = 0x0000Ff00;
		public const uint MaxRepeatedColorsLength = 4;

	}
}
