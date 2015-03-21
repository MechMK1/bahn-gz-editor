using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class GraphicArchive
	{
		#region Fields and Properties
		/// <summary>
		/// Tuple: Element number, Bauform, FwSig, Phase, Alt, Graphic
		/// </summary>
		private List<Tuple<int, int, int, int, int, Graphic>> graphics; // TODO Create class from tuple
		// TODO Use Dictionary<int, Graphic>

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
		/// Initializes a new Instance of BahnEditor.GraphicArchive class
		/// </summary>
		/// <param name="zoomFactor">Zoom factor of the graphics</param>
		public GraphicArchive(ZoomFactor zoomFactor)
		{
			graphics = new List<Tuple<int, int, int, int, int, Graphic>>();
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
				elementNumber = this.graphics.OrderBy(x => x.Item1).Last().Item1 + 1;
			this.AddGraphic(elementNumber, graphic);
		}

		/// <summary>
		/// Adds a graphic to the archive at a defined position
		/// </summary>
		/// <param name="elementNumber">Position in the archive</param>
		/// <param name="graphic">Graphic</param>
		/// <exception cref="System.ArgumentNullException"/>
		/// <exception cref="System.ArgumentOutOfRangeException"/>
		public void AddGraphic(int elementNumber, Graphic graphic)
		{
			this.AddGraphic(elementNumber, 0, 1, graphic);
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
			if (this.graphics.Count(x => x.Item1 == elementNumber && x.Item4 == phase && x.Item5 == alternative) > 0)
				throw new ArgumentException("Graphic is already existing at this position");
			this.graphics.Add(Tuple.Create(elementNumber, 0, 0, phase, alternative, graphic));
		}

		public Graphic this[int index]
		{
			get
			{
				try
				{
					IEnumerable<Tuple<int, int, int, int, int, Graphic>> e = graphics.Where(x => x.Item1 == index);
					return e.Single(x => x.Item4 == 0 && x.Item5 == 1).Item6;
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
		/// <param name="phase">Phase</param>
		/// <param name="alternative">Alternative</param>
		/// <exception cref="System.ArgumentException"/>
		public void RemoveGraphic(int elementNumber, int phase, int alternative)
		{
			int result = this.graphics.Count(x => x.Item1 == elementNumber && x.Item4 == phase && x.Item5 == alternative);
			if (result > 1)
				throw new ArgumentException("Too many graphics found");
			if (result == 0)
				throw new ArgumentException("Graphic not found");
			this.graphics.RemoveAll(x => x.Item1 == elementNumber && x.Item4 == phase && x.Item5 == alternative);
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
#		endregion Static Methods

		#region Private Methods
		private static GraphicArchive Load(FileStream path)
		{
			// TODO remove magic numbers
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				while (br.ReadByte() != 26) { }
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

				List<Tuple<int, int, int, int, int, Graphic>> graphics = new List<Tuple<int, int, int, int, int, Graphic>>();
				foreach (var item in seekList)
				{
					br.BaseStream.Seek(item, SeekOrigin.Begin);
					int elementNumber = br.ReadInt32();
					int bauform = br.ReadInt32();
					int fwSig = br.ReadInt32();
					int phase = br.ReadInt32();
					int alt = br.ReadInt32();
					int length = br.ReadInt32();
					int date = br.ReadInt32();
					using (MemoryStream ms = new MemoryStream())
					{
						br.BaseStream.CopyTo(ms, length);
						ms.Position = 0;
						graphics.Add(Tuple.Create(elementNumber, bauform, fwSig, phase, alt, Graphic.Load(ms)));
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
				if (item.Item6.IsElementEmpty())
				{
					throw new ElementIsEmptyException("a graphic is empty");
				}
			}
			// TODO remove magic numbers
			using (BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode))
			{
				List<Tuple<long, long>> offsetList = new List<Tuple<long, long>>();
				bw.Write(Constants.HEADERTEXT.ToArray()); //Headertext 
				bw.Write((byte)26); // text end
				bw.Write(new byte[] { 85, 90, 88 }); //identification string UZX ASCII
				bw.Write((byte)(48 + this.ZoomFactor)); //Zoom faktor ASCII
				foreach (var item in this.graphics)
				{
					bw.Write(item.Item1); //Elementnummer
					bw.Write(item.Item2); //Bauform
					bw.Write(item.Item3); //FwSig
					bw.Write(item.Item4); //Phase
					bw.Write(item.Item5); //Alt
					offsetList.Add(Tuple.Create<long, long>(bw.BaseStream.Position, 0));
					bw.Write(0); //Offset

				}
				bw.Write(0xFFFFFFFF);
				int i = 0;
				foreach (var item in this.graphics)
				{
					bw.Write(item.Item1); //Elementnummer
					offsetList[i] = Tuple.Create(offsetList[i].Item1, bw.BaseStream.Position);
					bw.Write(item.Item2); //Bauform
					bw.Write(item.Item3); //FwSig
					bw.Write(item.Item4); //Phase
					bw.Write(item.Item5); //Alt
					using (MemoryStream ms = new MemoryStream())
					{
						item.Item6.Save(ms);
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
