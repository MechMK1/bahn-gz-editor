using System;
using System.Globalization;

namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Represents a driving way element in BAHN
	/// </summary>
	public class DrivingWayElement
	{
		#region Properties
		/// <summary>
		/// Gets or sets the type of driving way
		/// </summary>
		public DrivingWay DrivingWay { get; set; }

		/// <summary>
		/// Gets or sets the additional functions of the driving way
		/// </summary>
		public DrivingWayFunction Function { get; set; }

		/// <summary>
		/// Gets or sets the incoming direction of the driving way
		/// </summary>
		public Direction Arrival { get; set; }

		/// <summary>
		/// Gets or sets the outgoing direction of the driving way
		/// </summary>
		public Direction Departure { get; set; }

		#endregion Properties

		#region Constructor
		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.DrivingWayElement class
		/// </summary>
		/// <param name="drivingWay">Type of driving way</param>
		/// <param name="function">Additional functions of the driving way</param>
		/// <param name="arrival">Incoming direction of the driving way</param>
		/// <param name="departure">Outgoing direction of the driving way</param>
		public DrivingWayElement(DrivingWay drivingWay, DrivingWayFunction function, Direction arrival, Direction departure)
		{
			this.DrivingWay = drivingWay;
			this.Function = function;
			this.Arrival = arrival;
			this.Departure = departure;
		}
		#endregion Constructor

		#region Internal Methods
		/// <summary>
		/// Gets a driving way from bytes (used to load from file)
		/// </summary>
		/// <param name="data">Data from file</param>
		/// <returns>Driving way element</returns>
		internal static DrivingWayElement FromBytes(byte[] data)
		{
			byte drivingWayByte = data[0];
			byte drivingWayFunctionByte = data[4];
			byte arrivalByte = (byte)(data[8] & 0xF);
			byte departureByte = (byte)((data[8] & 0xF0) >> 4);

			DrivingWay drivingWay = (DrivingWay)Enum.Parse(typeof(DrivingWay), drivingWayByte.ToString(CultureInfo.InvariantCulture));
			DrivingWayFunction function = (DrivingWayFunction)Enum.Parse(typeof(DrivingWayFunction), drivingWayFunctionByte.ToString(CultureInfo.InvariantCulture));
			Direction arrival = (Direction)Enum.Parse(typeof(Direction), arrivalByte.ToString(CultureInfo.InvariantCulture));
			Direction departure = (Direction)Enum.Parse(typeof(Direction), departureByte.ToString(CultureInfo.InvariantCulture));

			return new DrivingWayElement(drivingWay, function, arrival, departure);
		}

		/// <summary>
		/// Converts the driving way to bytes (used to save into a file)
		/// </summary>
		/// <returns>Data for file</returns>
		internal byte[] ToBytes()
		{
			byte[] result = new byte[12];
			result[0] = (byte)this.DrivingWay;
			result[4] = (byte)this.Function;
			result[8] = (byte)((byte)this.Departure << 4 | (byte)this.Arrival);
			return result;
		}
		#endregion Internal Methods
	}
}
