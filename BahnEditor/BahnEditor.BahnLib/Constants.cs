namespace BahnEditor.BahnLib
{
	public static class Constants
	{
		//TODO Write Doc for every constant
		//TODO Constants in english

		//Coordinates
		public const uint SYMBREITE = 32;
		public const uint SYMHOEHE = 16;
		public const uint MAXSYMHOEHE = (6 * SYMHOEHE);

		//Layers
		public const uint GSYMTEIL_MAXZAHL = 4;	//Maximale Anzahl von Layern pro Element
		/// <summary>
		/// Flach nach hinten (z. B. Bahnsteige)
		/// </summary>
		public const short LAYER_FL = 1;
		/// <summary>
		/// Vordergrund (vor Zug)
		/// </summary>
		public const short LAYER_VG = 2;
		/// <summary>
		/// Hintergrund und flach nach vorn (hinter oder unter Zug)
		/// </summary>
		public const short LAYER_HG = 3;
		/// <summary>
		/// Vorn, auf Brücken
		/// </summary>
		public const short LAYER_VB = 4;
		/// <summary>
		/// Vordergrund, oben (z. B. Oberleitung)
		/// </summary>
		public const short LAYER_VO = 5;
		public const short LAYER_MIN = 1; //Minimum Layers
		public const short LAYER_MAX = 5; //Maximum Layers

		//Variants
		public const uint KEINE_ALTERNATIVE = 0;
		public const uint MIN_ALTERNATIVE = 1;
		public const uint MAX_ALTERNATIVE = 4;
		public const uint MAX_ALTERNATIVZAHL = (MAX_ALTERNATIVE - MIN_ALTERNATIVE + 1);

		//Animations
		public const uint MIN_ANIPHASE = 0;
		public const uint MAX_ANIPHASE = 0;

		//File format versions and sub-versions
		public const uint ZOOMXGZGFORMAT384 = 0x0384;
		public const uint ZOOMXGZGSUBFORMAT5 = 0x05;

		//Unicode-Constants
		public const ushort UNICODE_NULL = 0x0000;
		public const ushort UNICODE_TAB = 0x0009;
		public const ushort UNICODE_CR = 0x000d;
		public const ushort UNICODE_LF = 0x000a;
		public const ushort UNICODE_SPACE = 0x0020;
		public const ushort UNICODE_HILO = 0xFeFf;
		public const ushort UNICODE_LOHI = 0xFfFe;

		//Colors
		public const uint FARBE_LOGISCH = 0x80000000;
		public const uint FARBE_TRANSPARENT = (FARBE_LOGISCH | 0x00000001);
		public const uint FARBE_LAMPE = 0x50000000;
		public const uint FARBE_ZUSATZ = 0xFf000000;
		public const uint FARBE_IMMERHELL = 0x40000000;
		public const uint FARBE_FENSTER = 0x60000000;
		public const uint FARBE_FENSTER_0 = FARBE_FENSTER;
		public const uint FARBE_FENSTER_1 = (FARBE_FENSTER | 0x01000000);
		public const uint FARBE_FENSTER_2 = (FARBE_FENSTER | 0x02000000);

		public const uint FARBE_WIE_MIN = (FARBE_LOGISCH | 0x00000100);
		public const uint FARBE_KOMPRIMIERT = (FARBE_LOGISCH | 0x40000000);
		public const uint FARBMASK_KOMPR_TR = 0x00010000;
		public const uint FARBE_KOMPR_TR = (FARBE_KOMPRIMIERT | FARBMASK_KOMPR_TR);
		public const uint FARBMASK_KOMPR_SYS = 0x00040000;
		public const uint FARBE_KOMPR_SYS = (FARBE_KOMPRIMIERT | FARBMASK_KOMPR_SYS);
		public const uint FARBMASK_KOMPR_SFB = 0x0000ff00;
		public const uint FARBMASK_KOMPR_ZAHL = 0x000000Ff;
		public const uint MAX_FARB_WDH = 257;
		public const uint FARBMASK_KOMPR_LEN = 0x0000Ff00;
		public const uint MAX_FARBFOLGE_LEN = 4;

		public static System.Collections.ObjectModel.ReadOnlyCollection<byte> HEADERTEXT = new System.Collections.ObjectModel.ReadOnlyCollection<byte> ( new System.Collections.Generic.List<byte>() { 67, 114, 101, 97, 116, 101, 100, 32, 98, 121, 32, 66, 97, 104, 110, 32, 71, 114, 97, 112, 104, 105, 99, 32, 69, 100, 105, 116, 111, 114 });
	}
}
