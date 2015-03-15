using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class GraphicArchive
	{
		/// <summary>
		/// Tuple: Element number, Bauform, FwSig, Phase, Alt, Graphic
		/// </summary>
		private List<Tuple<int, int, int, int, int, Graphic>> graphics;
		public ZoomFactor ZoomFactor { get; protected set; }
		public int GraphicsCount
		{
			get
			{
				return graphics.Count;
			}
		}

		public GraphicArchive(ZoomFactor zoomFactor)
		{
			graphics = new List<Tuple<int, int, int, int, int, Graphic>>();
			this.ZoomFactor = zoomFactor;
		}

		public void AddGraphic(Graphic graphic)
		{
			int elementNumber;
			if (this.graphics.Count == 0)
				elementNumber = 0;
			else
				elementNumber = this.graphics.OrderBy(x => x.Item1).Last().Item1 + 1;
			this.AddGraphic(elementNumber, graphic);
		}

		public void AddGraphic(int elementNumber, Graphic graphic)
		{
			this.AddGraphic(elementNumber, 0, 1, graphic);
		}

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
				throw new Exception("ZoomFactor not matching");
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

		private static GraphicArchive Load(FileStream path)
		{
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				while (br.ReadByte() != 26) { }
				byte[] read = br.ReadBytes(3);
				if (read[0] != 85 || read[1] != 90 || read[2] != 88)
				{
					throw new Exception("wrong identification string");
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
					br.BaseStream.Seek(16, SeekOrigin.Current);
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
					MemoryStream ms = new MemoryStream();
					br.BaseStream.CopyTo(ms, length);
					ms.Position = 0;
					graphics.Add(Tuple.Create(elementNumber, bauform, fwSig, phase, alt, Graphic.Load(ms)));
				}
				GraphicArchive archive;
				switch (zoomFactor)
				{
					case 1:
						archive = new GraphicArchive(ZoomFactor.Zoom1);
						break;
					case 2:
						archive = new GraphicArchive(ZoomFactor.Zoom2);
						break;
					case 4:
						archive = new GraphicArchive(ZoomFactor.Zoom4);
						break;
					default:
						throw new Exception("Invalid ZoomFactor");
				}
				archive.graphics = graphics;
				return archive;
			}
		}

		public bool Save(string path, bool overwrite)
		{
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

		private bool Save(FileStream path)
		{
			foreach (var item in this.graphics)
			{
				if (item.Item6.ValidateElement())
				{
					throw new ElementIsEmptyException("a graphic is empty");
				}
			}
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
					MemoryStream ms = new MemoryStream();
					item.Item6.Save(ms);
					bw.Write((int)ms.Length); //Länge der Einzeldatei in Byte
					bw.Write(0x46646E7C); //Datum
					ms.WriteTo(bw.BaseStream);
					ms.Dispose();
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

		private int GetTime()
		{

			return 0;
		}
	}
}
