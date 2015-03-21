namespace BahnEditor.BahnLib
{
	[System.Flags]
	public enum ClockProperties : int
	{
		/// <summary>
		/// Display clock in 24h-format (else: 12h-format)
		/// </summary>
		Display24h = 0x0002,
		/// <summary>
		/// minute pointer available
		/// </summary>
		MinutePointer = 0x0004,
		/// <summary>
		/// clock rotated some degrees to Northwest
		/// <para>RotatedNorthWest and RotatedNorthEast are exclusive</para>
		/// </summary>
		RotatedNorthWest = 0x0010,
		/// <summary>
		/// clock rotated some degrees to Northeast
		/// <para>RotatedNorthEast and RotatedNorthWest are exclusive</para>
		/// </summary>
		RotatedNorthEast = 0x0020
	}
}