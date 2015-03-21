namespace BahnEditor.BahnLib
{
	[System.Flags]
	public enum GraphicProperties : int
	{
		/// <summary>
		/// Graphic emits smoke (industry), more dark gray
		/// <para>Steam- and Smoke-Attributes are exclusive</para>
		/// </summary>
		Smoke = 0x0001,
		/// <summary>
		/// Graphic emits steam (as smoke, but more light colors) 
		/// <para>Steam- and Smoke-Attributes are exclusive</para>
		/// </summary>
		Steam = 0x0002,
		/// <summary>
		/// Graphic displays an animated clock, since BAHN 3.85r3
		/// </summary>
		Clock = 0x0004,
		/// <summary>
		/// Direction info for cursor available, since BAHN 3.86b0
		/// </summary>
		Cursor = 0x0008,
		/// <summary>
		/// Driving way info available, since BAHN 3.86b0Graphic 
		/// </summary>
		DrivingWay = 0x0010,
		/// <summary>
		/// Color for map / schematic view available, since BAHN 3.86b0
		/// </summary>
		ColorSchematicMode = 0x0020,
		/// <summary>
		/// Color format of BAHN 3.83, since BAHN 3.86b0: must be set in any case
		/// </summary>
		ColorFormat24BPP = 0x0200
	}
}
