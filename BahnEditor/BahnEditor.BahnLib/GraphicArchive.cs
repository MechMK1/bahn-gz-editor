using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BahnEditor.BahnLib
{
	/// <summary>
	/// Represents an archive that contains one or more graphics
	/// </summary>
	public class GraphicArchive
	{
		#region Fields and Properties

		/// <summary>
		/// List of graphics in the archive
		/// </summary>
		private List<ArchiveElement> graphics;

		public string FileName { get; private set; }

		/// <summary>
		/// Gets the count of graphics in the archive
		/// </summary>
		public int GraphicsCount
		{
			get
			{
				return graphics.Count;
			}
		}

		/// <summary>
		/// Gets the zoom factor
		/// </summary>
		public ZoomFactor ZoomFactor { get; protected set; }

		public Animation Animation { get; private set; }

		#endregion Fields and Properties

		#region Constructor

		/// <summary>
		/// Initializes a new Instance of BahnEditor.BahnLib.GraphicArchive class
		/// </summary>
		/// <param name="zoomFactor">Zoom factor of the graphics</param>
		public GraphicArchive(ZoomFactor zoomFactor)
		{
			graphics = new List<ArchiveElement>();
			this.ZoomFactor = zoomFactor;
		}

		#endregion Constructor

		#region Public Methods

		public Graphic this[int index]
		{
			get
			{
				try
				{
					Graphic graphic;
					// Try if alternatives exist...
					for (int i = 1; i < 4; i++)
					{
						graphic = this[index, 0, i];
						if (graphic != null)
							return graphic;
					}

					// If not, return the first
					return this[index, 0, 0];
				}
				catch (InvalidOperationException)
				{
					return null;
				}
			}
		}

		public Graphic this[int elementNumber, int animationPhase, int alternative]
		{
			get
			{
				ArchiveElement element = this.graphics.SingleOrDefault(x => x.ElementNumber == elementNumber && x.AnimationPhase == animationPhase && x.Alternative == alternative);
				if (element != null)
				{
					if (element.Graphic.IsEmpty() && this.FileName != null)
					{
						using (FileStream stream = File.Open(this.FileName, FileMode.Open))
						{
							stream.Seek(element.SeekPositionGraphicData, SeekOrigin.Begin);
							using (BinaryReader br = new BinaryReader(stream))
							{
								element.Graphic.LoadData(br);
							}
						}
					}
					return element.Graphic;
				}
				return null;
			}
		}

		/// <summary>
		/// Adds a graphic to the archive at the last position
		/// </summary>
		/// <param name="graphic">Graphic</param>
		/// <exception cref="System.ArgumentNullException"/>
		public void AddGraphic(Graphic graphic)
		{
			if (graphic == null)
				throw new ArgumentNullException("graphic");
			int elementNumber;
			if (this.graphics.Count == 0)
				elementNumber = 0;
			else
				elementNumber = this.graphics.OrderBy(x => x.ElementNumber).Last().ElementNumber + 1;
			this.AddGraphic(elementNumber, graphic);
		}

		/// <summary>
		/// Adds a graphic to the archive at a defined position
		/// </summary>
		/// <param name="elementNumber">Position in the archive</param>
		/// <param name="graphic">Graphic</param>
		/// <exception cref="System.ArgumentNullException"/>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		/// <exception cref="System.ArgumentException"/>
		public void AddGraphic(int elementNumber, Graphic graphic)
		{
			this.AddGraphic(elementNumber, Constants.MinAnimationPhase, Constants.NoAlternative, graphic);
		}

		/// <summary>
		/// Adds a graphic to the archive at a defined position with animationphase and alternative
		/// </summary>
		/// <param name="elementNumber">Position in the archive</param>
		/// <param name="phase">Animationphase</param>
		/// <param name="alternative">Alternative</param>
		/// <param name="graphic">Graphic</param>
		/// <exception cref="System.ArgumentNullException"/>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		/// <exception cref="System.ArgumentException"/>
		public void AddGraphic(int elementNumber, int phase, int alternative, Graphic graphic)
		{
			if (graphic == null)
				throw new ArgumentNullException("graphic");
			if (elementNumber < 0 || elementNumber > 89)
				throw new ArgumentOutOfRangeException("elementNumber");
			if (phase < 0 || phase > 99)
				throw new ArgumentOutOfRangeException("phase");
			if (alternative < 0 || alternative > 4)
				throw new ArgumentOutOfRangeException("alternative");
			if (graphic.ZoomFactor != this.ZoomFactor)
				throw new ArgumentException("Zoomfactor not matching");
			if (this.graphics.Count(x => x.ElementNumber == elementNumber && x.AnimationPhase == phase && x.Alternative == alternative) > 0)
				throw new ArgumentException("Graphic is already existing at this position");
			this.graphics.Add(new ArchiveElement(elementNumber, phase, alternative, graphic));
		}

		public void AddAnimation()
		{
			if (this.ZoomFactor != BahnLib.ZoomFactor.Zoom1)
				throw new ArgumentException("Animations only available in Zoom1-Archives");
			if (this.Animation != null)
				throw new ArgumentException("There is already an animation");
			this.Animation = new Animation();
		}

		/// <summary>
		/// Removes a graphic from the archive
		/// </summary>
		/// <param name="elementNumber">Position in the archive</param>
		/// <exception cref="System.ArgumentException"/>
		public void RemoveGraphic(int elementNumber)
		{
			this.RemoveGraphic(elementNumber, Constants.MinAnimationPhase, Constants.NoAlternative);
		}

		/// <summary>
		/// Removes a graphic from the archive
		/// </summary>
		/// <param name="elementNumber">Position in the archive</param>
		/// <param name="phase">Animationphase</param>
		/// <param name="alternative">Alternative</param>
		/// <exception cref="System.ArgumentException"/>
		public void RemoveGraphic(int elementNumber, int phase, int alternative)
		{
			int result = this.graphics.Count(x => x.ElementNumber == elementNumber && x.AnimationPhase == phase && x.Alternative == alternative);
			if (result > 1)
				throw new ArgumentException("Too many graphics found");
			if (result == 0)
				throw new ArgumentException("Graphic not found");
			this.graphics.RemoveAll(x => x.ElementNumber == elementNumber && x.AnimationPhase == phase && x.Alternative == alternative);
		}

		public void RemoveAnimation()
		{
			if (this.ZoomFactor != BahnLib.ZoomFactor.Zoom1)
				throw new ArgumentException("Animations only available in Zoom1-Archives");
			if (this.Animation == null)
				throw new ArgumentException("There is no animation to remove");
			this.Animation = null;
		}

		/// <summary>
		/// Saves the archive into a file
		/// </summary>
		/// <param name="path">Path for the file</param>
		/// <param name="overwrite">If the file should be overwritten when existing</param>
		/// <returns>Returns true if succeeded, else false</returns>
		/// <exception cref="BahnEditor.BahnLib.ArchiveIsEmptyException"/>
		/// <exception cref="BahnEditor.BahnLib.ElementIsEmptyException"/>
		/// <exception cref="System.ArgumentNullException"/>
		public bool Save(string path, bool overwrite)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (File.Exists(path) && !overwrite)
			{
				return false;
			}
			else
			{
				if (this.graphics.Count <= 0)
				{
					throw new ArchiveIsEmptyException("the archive is empty");
				}
				foreach (var item in this.graphics)
				{
					if (item.Graphic.IsTransparent())
					{
						throw new ElementIsEmptyException("a graphic is empty");
					}
				}
				using (FileStream stream = File.OpenWrite(path))
				{
					bool result = Save(stream);
					if (this.Animation != null && result)
					{
						this.Animation.Save(path.Remove(path.Length - 3) + "bnm", true);
					}
					return result;
				}
			}
		}

		#endregion Public Methods

		#region Static Methods

		/// <summary>
		/// Loads an archive from a file
		/// </summary>
		/// <param name="path">Path to the file</param>
		/// <returns>Loaded Archive</returns>
		/// <exception cref="System.IO.FileNotFoundException"/>
		/// <exception cref="System.IO.InvalidDataException"/>
		public static GraphicArchive Load(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (File.Exists(path))
			{
				GraphicArchive archive = null;
				using (FileStream stream = File.OpenRead(path))
				{
					archive = Load(stream);
					archive.FileName = path;
				}
				if (archive != null && archive.ZoomFactor == ZoomFactor.Zoom1)
				{
					string animationPath = path.Remove(path.Length - 3) + "bnm";
					if (File.Exists(animationPath))
					{
						archive.Animation = Animation.Load(animationPath);
					}
				}
				return archive;
			}
			else throw new FileNotFoundException("File not found", path);
		}

		#endregion Static Methods

		#region Private Methods

		private static GraphicArchive Load(FileStream path)
		{
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				while (br.ReadByte() != Constants.HeaderTextTerminator) { }
				byte[] read = br.ReadBytes(3);
				if (read[0] != 85 || read[1] != 90 || read[2] != 88) // 'UZX' as ASCII characters
				{
					throw new InvalidDataException("wrong identification string");
				}
				byte zoomFactor = (byte)(br.ReadByte() - 48); // zoomFactor number is stored as ASCII. -48 will 'convert' this to int
				int elementNumber;
				List<ArchiveElement> graphics = new List<ArchiveElement>();
				while (true)
				{
					elementNumber = br.ReadInt32();
					if (elementNumber == -1)
					{
						break;
					}
					br.ReadInt32();                          //skipping data (bauform)
					br.ReadInt32();                          //skipping data (fwSig)
					int animationPhase = br.ReadInt32(); //AnimationPhase
					int alternative = br.ReadInt32();    //Alternative
					ArchiveElement archiveElement = new ArchiveElement(elementNumber, animationPhase, alternative, null); //Graphic loaded later
					archiveElement.SeekPosition = ((int)(br.BaseStream.Position + 4 + br.ReadInt32())); // +4 Used for unknown reasons. Possibly because of C/C# incompatibility
					graphics.Add(archiveElement);
				}
				foreach (var item in graphics)
				{
					br.BaseStream.Seek(item.SeekPosition + sizeof(int) * 5, SeekOrigin.Begin); // sizeof(int) * 5 => Skip 5 integer-sized fields
					br.ReadInt32(); //Length
					br.ReadInt32(); //Date
					item.Graphic = Graphic.LoadHeader(br);
					item.SeekPositionGraphicData = br.BaseStream.Position;
				}
				GraphicArchive archive;
				switch (zoomFactor)
				{
					case 1:
					case 2:
					case 4:
						ZoomFactor f = (ZoomFactor)zoomFactor;
						archive = new GraphicArchive(f);
						break;

					default:
						throw new InvalidDataException("Invalid ZoomFactor");
				}
				archive.graphics = graphics;

				return archive;
			}
		}

		private bool Save(FileStream path)
		{
			using (BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode))
			{
				List<Tuple<long, long>> offsetList = new List<Tuple<long, long>>();
				bw.Write(Constants.HeaderText.ToArray()); //Headertext
				bw.Write(new byte[] { 85, 90, 88 });      //identification string UZX ASCII
				bw.Write((byte)(48 + this.ZoomFactor));   //Zoom factor to ASCII
				foreach (var item in this.graphics)
				{
					bw.Write(item.ElementNumber);  //Elementnummer
					bw.Write(0);                   //Bauform
					bw.Write(0);                   //FwSig
					bw.Write(item.AnimationPhase); //Phase
					bw.Write(item.Alternative);    //Alternative
					offsetList.Add(Tuple.Create<long, long>(bw.BaseStream.Position, 0));
					bw.Write(0); //Offset
				}
				bw.Write(0xFFFFFFFF); //Terminator
				int i = 0;
				foreach (var item in this.graphics)
				{
					bw.Write(item.ElementNumber); //Elementnummer
					offsetList[i] = Tuple.Create(offsetList[i].Item1, bw.BaseStream.Position);
					bw.Write(0);                   //Bauform
					bw.Write(0);                   //FwSig
					bw.Write(item.AnimationPhase); //Phase
					bw.Write(item.Alternative);    //Alt
					using (MemoryStream ms = new MemoryStream())
					{
						item.Graphic.Save(ms);
						bw.Write((int)ms.Length); //Länge der Einzeldatei in Byte
						bw.Write(0x46646E7C);     //Dummy data
						ms.WriteTo(bw.BaseStream);
					}
					i++;
				}
				foreach (var item in offsetList)
				{
					bw.Seek((int)item.Item1, SeekOrigin.Begin);
					bw.Write((int)(item.Item2 - item.Item1 - 8)); // -8 Used for unknown reasons. Possibly because of C/C# incompatibility
				}
				bw.Flush();
				return true;
			}
		}

		#endregion Private Methods

		#region Nested Classes

		/// <summary>
		/// Represents an element in a graphic archive
		/// </summary>
		internal class ArchiveElement
		{
			#region Properties

			/// <summary>
			/// Alternative
			/// </summary>
			public int Alternative { get; set; }

			/// <summary>
			/// Animationphase
			/// </summary>
			public int AnimationPhase { get; set; }

			/// <summary>
			/// Index in the archive
			/// </summary>
			public int ElementNumber { get; set; }

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

			#endregion Constructor
		}

		#endregion Nested Classes
	}
}