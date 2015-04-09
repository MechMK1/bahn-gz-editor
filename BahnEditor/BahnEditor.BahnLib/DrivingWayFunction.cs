namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Enum of driving way functions
	/// </summary>
	public enum DrivingWayFunction : byte
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// 
		/// </summary>
		TunnelIn = 1,
		/// <summary>
		/// 
		/// </summary>
		TunnelOut = 2,
		/// <summary>
		/// 
		/// </summary>
		Ramp = 3,
		/// <summary>
		/// 
		/// </summary>
		Crossing = 0x80,
		/// <summary>
		/// 
		/// </summary>
		TunnelInAndCrossing = TunnelIn | Crossing,
		/// <summary>
		/// 
		/// </summary>
		TunnelOutAndCrossing = TunnelOut | Crossing,
		/// <summary>
		/// 
		/// </summary>
		RampAndCrossing = Ramp | Crossing
	}
}
