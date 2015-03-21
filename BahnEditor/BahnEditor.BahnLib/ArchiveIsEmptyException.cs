using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	/// <summary>
	/// The exception is thrown when the archive is empty
	/// </summary>
	[Serializable]
	public class ArchiveIsEmptyException : Exception
	{
		public ArchiveIsEmptyException() { }
		public ArchiveIsEmptyException(string message) : base(message) { }
		public ArchiveIsEmptyException(string message, Exception inner) : base(message, inner) { }
		protected ArchiveIsEmptyException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
