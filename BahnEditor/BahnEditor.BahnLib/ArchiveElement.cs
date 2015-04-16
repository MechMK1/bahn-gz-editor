using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class ArchiveElement
	{
		#region Properties
		public int ElementNumber { get; set; }

		public int Bauform { get; set; }

		public int DrivingWay_Signal { get; set; }

		public int AnimationPhase { get; set; }

		public int Alternative { get; set; }

		public Graphic Graphic { get; set; }
		#endregion Properties

		#region Constructor
		internal ArchiveElement()
		{

		}

		internal ArchiveElement(int elementNumber, int bauform, int drivingWay_Signal, int animationPhase, int alternative, Graphic graphic)
		{
			this.ElementNumber = elementNumber;
			this.Bauform = bauform;
			this.DrivingWay_Signal = drivingWay_Signal;
			this.AnimationPhase = animationPhase;
			this.Alternative = alternative;
			this.Graphic = graphic;
		}
		#endregion
	}
}
