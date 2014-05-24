namespace BahnEditor.BahnLib
{
	public static class Constants
	{
		//Coordinates
		public const uint SYMBREITE				= 32;
		public const uint SYMHOEHE				= 16;
		public const uint MAXSYMHOEHE			= (6 * SYMHOEHE);

		//Layers
		public const uint GSYMTEIL_MAXZAHL		= 4; //Maximale Anzahl von Layern pro Element
		public const uint GFX_Z_HI				= 1; //zum Hintergrund (=flach, nicht "aufwärts", z. B. Platformen)
		public const uint GFY_Z_VG				= 2; //Vordergrund (in Front von Fahrwegen und Zügen
		public const uint GFX_Z_HG				= 3; //Hintergrund (auch hinter Zügen auf ny+1)
		public const uint GFX_Z_OG				= 4; //Front
		public const uint GFX_Z_VGO				= 5; //Upper foreground
		public const uint GFX_Z_MIN				= 1; //Minimum Layers
		public const uint GFX_Z_MAX				= 5; //Maximum Layers

		//Variants
		public const uint KEINE_ALTERNATIVE		= 0;
		public const uint MIN_ALTERNATIVE		= 1;
		public const uint MAX_ALTERNATIVE		= 4;
		public const uint MAX_ALTERNATIVZAHL	= (MAX_ALTERNATIVE - MIN_ALTERNATIVE + 1);

		//Animations
		public const uint MIN_ANIPHASE			= 0;
		public const uint MAX_ANIPHASE			= 0;

		//File format versions and sub-versions
		public const uint ZOOMXGZGFORMAT384		= 0x0384;
		public const uint ZOOMXGZGSUBFORMAT5	= 0x05;

		//Unicode-Constants
		public const ushort UNICODE_NULL		= 0x0000;
		public const ushort UNICODE_TAB			= 0x0009;
		public const ushort UNICODE_CR			= 0x000d;
		public const ushort UNICODE_LF			= 0x000a;
		public const ushort UNICODE_SPACE		= 0x0020;
		public const ushort UNICODE_HILO		= 0xFeFf;
		public const ushort UNICODE_LOHI		= 0xFfFe;
	}
}
