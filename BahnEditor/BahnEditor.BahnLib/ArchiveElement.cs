namespace BahnEditor.BahnLib
{
	// TODO Refactor as nested class of GraphicArchive
	/// <summary>
	/// Represents an element in a graphic archive
	/// </summary>
	internal class ArchiveElement
	{
		#region Properties
		/// <summary>
		/// Index in the archive
		/// </summary>
		public int ElementNumber { get; set; }

		/// <summary>
		/// Animationphase
		/// </summary>
		public int AnimationPhase { get; set; }

		/// <summary>
		/// Alternative
		/// </summary>
		public int Alternative { get; set; }

		/// <summary>
		/// Graphic
		/// </summary>
		public Graphic Graphic { get; set; }

		/// <summary>
		/// Seekposition in the archive-file (prepared for later)
		/// </summary>
		public int SeekPosition { get; set; }

		public long SeekPositionGraphicData { get; set; }
		#endregion Properties

		#region Constructor
		internal ArchiveElement(int elementNumber, int animationPhase, int alternative, Graphic graphic)
		{
			this.ElementNumber = elementNumber;
			this.AnimationPhase = animationPhase;
			this.Alternative = alternative;
			this.Graphic = graphic;
		}
		#endregion
	}
}
