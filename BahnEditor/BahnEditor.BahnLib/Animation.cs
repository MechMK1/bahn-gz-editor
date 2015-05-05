using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class Animation
	{
		private Graphic[] Graphics;

		public Animation(GraphicArchive archive, int elementID, int alternativeID)
		{
			this.Graphics = archive[(element => element.ElementNumber == elementID && element.Alternative == alternativeID)].Select((s=>s.Graphic)).ToArray<Graphic>();
		}

	}
}
