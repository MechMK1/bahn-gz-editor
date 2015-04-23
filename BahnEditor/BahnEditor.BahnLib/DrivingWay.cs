namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Enum of driving way information
	/// </summary>
	public enum DrivingWay : byte
	{
		/// <summary>
		/// Driving way on rails
		/// </summary>
		Rail = 1,
		/// <summary>
		/// Driving way on road
		/// </summary>
		Road = 2,
		/// <summary>
		/// Driving way on rails and road (e.g. tram tracks on a road)
		/// </summary>
		RailAndRoad = Rail | Road,
		/// <summary>
		/// Driving way on water
		/// </summary>
		Water = 4
	}
}
