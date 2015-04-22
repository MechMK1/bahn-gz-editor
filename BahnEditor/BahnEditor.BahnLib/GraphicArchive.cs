using System;
using System.Collections.Generic;
using System.Globalization;
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

		/// <summary>
		/// Gets the zoom factor
		/// </summary>
		public ZoomFactor ZoomFactor { get; protected set; }

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

		public Graphic this[int index]
		{
			get
			{
				try
				{
					IEnumerable<ArchiveElement> enumerable = graphics.Where(x => x.ElementNumber == index);
					for (int i = 1; i < 4; i++)
					{
						ArchiveElement archiveElement = enumerable.SingleOrDefault(x => x.AnimationPhase == 0 && x.Alternative == i);
						if (archiveElement != null)
						{
							return archiveElement.Graphic;
						}
					}
					ArchiveElement element = enumerable.SingleOrDefault(x => x.AnimationPhase == 0 && x.Alternative == 0);
					if (element != null)
					{
						return element.Graphic;
					}
					return null;
				}
				catch (InvalidOperationException)
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Removes a graphic from the archive
		/// </summary>
		/// <param name="elementNumber">Position in the archive</param>
		/// <exception cref="System.ArgumentException"/>
		public void RemoveGraphic(int elementNumber)
		{
			this.RemoveGraphic(elementNumber, 0, 1);
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
				using (FileStream stream = File.OpenWrite(path))
				{
					return Save(stream);
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
			if (File.Exists(path))
			{
				using (FileStream stream = File.OpenRead(path))
				{
					return Load(stream);
				}
			}
			else throw new FileNotFoundException("File not found", path);
		}
		#endregion Static Methods

		#region Private Methods
		private static GraphicArchive Load(FileStream path)
		{
			// TODO remove magic numbers
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				while (br.ReadByte() != Constants.HeaderTextTerminator) { }
				byte[] read = br.ReadBytes(3);
				if (read[0] != 85 || read[1] != 90 || read[2] != 88)
				{
					throw new InvalidDataException("wrong identification string");
				}
				byte zoomFactor = (byte)(br.ReadByte() - 48);
				int elementNummer;
				List<int> seekList = new List<int>();
				while (true)
				{
					elementNummer = br.ReadInt32();
					if (elementNummer == -1)
					{
						break;
					}
					br.ReadInt32(); //skipping redundant data (bauform)
					br.ReadInt32(); //skipping redundant data (fwSig)
					br.ReadInt32(); //skipping redundant data (phase)
					br.ReadInt32(); //skipping redundant data (alt)
					seekList.Add((int)(br.BaseStream.Position + 4 + br.ReadInt32()));
				}

				List<ArchiveElement> graphics = new List<ArchiveElement>();
				foreach (var item in seekList)
				{
					br.BaseStream.Seek(item, SeekOrigin.Begin);
					ArchiveElement element = new ArchiveElement();
					element.ElementNumber = br.ReadInt32();
					br.ReadInt32(); //skip bauform
					br.ReadInt32(); //skip fwSig
					element.AnimationPhase = br.ReadInt32();
					element.Alternative = br.ReadInt32();
					int length = br.ReadInt32();
					int date = br.ReadInt32();
					using (MemoryStream ms = new MemoryStream())
					{
						br.BaseStream.CopyTo(ms, length);
						ms.Position = 0;
						element.Graphic = Graphic.Load(ms);
						graphics.Add(element);
					}
				}
				GraphicArchive archive;
				switch (zoomFactor)
				{
					case 1:
					case 2:
					case 4:
						ZoomFactor f = (ZoomFactor)Enum.Parse(typeof(ZoomFactor), zoomFactor.ToString(CultureInfo.InvariantCulture));
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
			if (this.graphics.Count <= 0)
			{
				throw new ArchiveIsEmptyException("the archive is empty");
			}
			foreach (var item in this.graphics)
			{
				if (item.Graphic.IsElementEmpty())
				{
					throw new ElementIsEmptyException("a graphic is empty");
				}
			}
			// TODO remove magic numbers
			using (BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode))
			{
				List<Tuple<long, long>> offsetList = new List<Tuple<long, long>>();
				bw.Write(Constants.HeaderText.ToArray()); //Headertext 
				bw.Write(new byte[] { 85, 90, 88 }); //identification string UZX ASCII
				bw.Write((byte)(48 + this.ZoomFactor)); //Zoom faktor ASCII
				foreach (var item in this.graphics)
				{
					bw.Write(item.ElementNumber); //Elementnummer
					bw.Write(0); //Bauform
					bw.Write(0); //FwSig
					bw.Write(item.AnimationPhase); //Phase
					bw.Write(item.Alternative); //Alt
					offsetList.Add(Tuple.Create<long, long>(bw.BaseStream.Position, 0));
					bw.Write(0); //Offset

				}
				bw.Write(0xFFFFFFFF);
				int i = 0;
				foreach (var item in this.graphics)
				{
					bw.Write(item.ElementNumber); //Elementnummer
					offsetList[i] = Tuple.Create(offsetList[i].Item1, bw.BaseStream.Position);
					bw.Write(0); //Bauform
					bw.Write(0); //FwSig
					bw.Write(item.AnimationPhase); //Phase
					bw.Write(item.Alternative); //Alt
					using (MemoryStream ms = new MemoryStream())
					{
						item.Graphic.Save(ms);
						bw.Write((int)ms.Length); //Länge der Einzeldatei in Byte
						bw.Write(0x46646E7C); //Datum
						ms.WriteTo(bw.BaseStream);
					}
					i++;
				}
				foreach (var item in offsetList)
				{
					bw.Seek((int)item.Item1, SeekOrigin.Begin);
					bw.Write((int)(item.Item2 - item.Item1 - 8));
				}
				bw.Flush();
				return true;
			}
		}

		private static int GetTime()
		{
			// TODO do actual stuff
			return 0;
		}
		#endregion Private Methods
	}
}
