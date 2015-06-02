using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class AnimationStep
	{
		public int AnimationPhase { get; set; }

		public int MinimumTime { get; set; }

		public int MaximumTime { get; set; }

		public int Sound { get; set; }

		public AnimationStep(int animationPhase, int minimumTime, int maximumTime, int sound)
		{
			this.AnimationPhase = animationPhase;
			this.MinimumTime = minimumTime;
			this.MaximumTime = maximumTime;
			this.Sound = sound;
		}
	}
}
