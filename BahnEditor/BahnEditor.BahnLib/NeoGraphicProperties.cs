using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class NeoGraphicProperties
	{
		//TODO Move RawData to ctor
		public Flags RawData { get; set; }
		public uint ColorInSchematicMode { get; set; }

		public int ParticleX { get; set; }
		public int ParticleY { get; set; }
		public int ParticleWidth { get; set; }
		public int ClockX { get; set; }
		public int ClockY { get; set; }
		public LayerId ClockZ { get; set; }
		public int ClockWidth { get; set; }
		public int ClockHeight { get; set; }
		
		public ClockProperties ClockProperties { get; set; }
		public uint ClockColorHoursPointer { get; set; }
		public uint ClockColorMinutesPointer { get; set; }
		/// <summary>
		/// Gets or sets the normal cursor direction
		/// <para>Makes only sense if cursor is set in properties, else data is ignored</para>
		/// </summary>
		public Direction CursorNormalDirection { get; set; }

		/// <summary>
		/// Gets or sets the reverse cursor direction
		/// <para>Mostly, this is the opposite of CursorNormalDirection</para>
		/// <para>Makes only sense if cursor is set in properties, else data is ignored</para>
		/// </summary>
		public Direction CursorReverseDirection { get; set; }

		public bool HasParticles { 
			get {
				return this.RawData.HasFlag(Flags.Smoke) || this.RawData.HasFlag(Flags.Steam);
			} 
		}

		[System.Flags]
		public enum Flags : short
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
}
