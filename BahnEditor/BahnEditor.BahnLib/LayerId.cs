namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Enum of layer ids 
	/// </summary>
	public enum LayerId : int
	{
		/// <summary>
		/// To background (=flat, not "upwards", i.e. platforms)
		/// </summary>
		ToBackground = 1,
		/// <summary>
		/// Foreground (in front of driving way and train)
		/// </summary>
		Foreground = 2,
		/// <summary>
		/// background (behind train)
		/// </summary>
		Background = 3,
		/// <summary>
		/// front (used especially for bridges 45 degrees)
		/// </summary>
		Front = 4,
		/// <summary>
		/// Foreground above (use e.g. for overhead wires)
		/// </summary>
		ForegroundAbove = 5
	}
}
