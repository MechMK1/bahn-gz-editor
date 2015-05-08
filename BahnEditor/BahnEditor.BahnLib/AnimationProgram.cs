using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class AnimationProgram
	{
		private List<AnimationStep> animationSteps = new List<AnimationStep>();

		public int XDiff { get; set; }

		public int YDiff { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public AnimationProgram(int xDiff, int yDiff, int width, int height)
		{
			this.XDiff = xDiff;
			this.YDiff = yDiff;
			this.Width = width;
			this.Height = height;
		}

		public void AddAnimationStep(AnimationStep animationStep)
		{
			if (animationStep == null)
				throw new ArgumentNullException("animationStep");
			this.animationSteps.Add(animationStep);
		}

		public void InsertAnimationStep(AnimationStep animationStep, int index)
		{
			if (animationStep == null)
				throw new ArgumentNullException("animationStep");
			this.animationSteps.Insert(index, animationStep);
		}

		public AnimationStep this[int index]
		{
			get
			{
				return this.animationSteps[index];
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				this.animationSteps[index] = value;
			}
		}

		public void RemoveAnimationStep(int index)
		{
			this.animationSteps.Remove(this.animationSteps[index]);
		}
	}
}
