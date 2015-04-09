namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Enum of the version of a graphic
	/// </summary>
	public enum GraphicVersion : byte
	{
		/// <summary>
		/// BAHN 3.84, 3.85Beta,r1,r2,r3
		/// </summary>
		Version0 = 0x00,
		/// <summary>
		/// BAHN 3.85r3, used only if GSYMEIG_UHR is set
		/// </summary>
		Version1 = 0x03,
		/// <summary>
		/// BAHN 3.86 and higher
		/// </summary>
		Version2 = 0x05
	}
}
