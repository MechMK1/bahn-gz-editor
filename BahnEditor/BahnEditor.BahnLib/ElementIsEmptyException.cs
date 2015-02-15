using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	[Serializable]
	public class ElementIsEmptyException : Exception
	{
		public ElementIsEmptyException() { }
		public ElementIsEmptyException(string message) : base(message) { }
		public ElementIsEmptyException(string message, Exception inner) : base(message, inner) { }
		protected ElementIsEmptyException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
