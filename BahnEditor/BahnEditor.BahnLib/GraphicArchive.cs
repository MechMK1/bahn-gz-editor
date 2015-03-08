using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public abstract class GraphicArchive
	{
		private List<Graphic> graphics;
		public byte ZoomFactor { get; protected set; }
		public int GraphicsCount
		{
			get
			{
				return graphics.Count;
			}
		}

		protected GraphicArchive(byte zoomFactor)
		{
			graphics = new List<Graphic>();
			this.ZoomFactor = zoomFactor;
		}

		public void AddGraphic(Graphic graphic)
		{
			if (graphic == null)
				throw new ArgumentNullException("graphic");
			if (graphic.ZoomFactor != this.ZoomFactor)
				throw new Exception("ZoomFactor not matching");
			this.graphics.Add(graphic);
		}

		public Graphic this[int index]
		{
			get { return graphics[index]; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				graphics[index] = value;
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
				while(true)
				{
					elementNummer = br.ReadInt32();
					if(elementNummer == -1)
					{
						break;
					}
					br.BaseStream.Seek(16, SeekOrigin.Current);
					seekList.Add((int)(br.BaseStream.Position + 8 + br.ReadInt32()));
				}
				
				List<Graphic> graphics = new List<Graphic>();
				foreach (var item in seekList)
				{
					br.BaseStream.Seek(item, SeekOrigin.Begin);
					int bauform = br.ReadInt32();
					int fwSig = br.ReadInt32();
					int phase = br.ReadInt32();
					int alt = br.ReadInt32();
					int length = br.ReadInt32();
					int date = br.ReadInt32();
					MemoryStream ms = new MemoryStream();
					br.BaseStream.CopyTo(ms, length);
					ms.Position = 0;
					graphics.Add(Graphic.Load(ms));
				}
				GraphicArchive archive;
				switch (zoomFactor)
				{
					case 1:
						archive = new Zoom1GraphicArchive();
						break;
					case 2:
						archive = new Zoom2GraphicArchive();
						break;
					case 4:
						archive = new Zoom2GraphicArchive();
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
				if (item.ValidateElement())
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
				for (int i = 0; i < this.graphics.Count; i++)
				{
					bw.Write(i); //Elementnummer
					bw.Write(0); //Bauform
					bw.Write(0); //FwSig
					bw.Write(0); //Phase
					bw.Write(1); //Alt
					offsetList.Add(Tuple.Create<long, long>(bw.BaseStream.Position, 0));
					bw.Write(0); //Offset

				}
				bw.Write(0xFFFFFFFF);
				for (int i = 0; i < this.graphics.Count; i++)
				{
					bw.Write(i); //Elementnummer
					offsetList[i] = Tuple.Create(offsetList[i].Item1, bw.BaseStream.Position);
					bw.Write(0); //Bauform
					bw.Write(0); //FwSig
					bw.Write(0); //Phase
					bw.Write(1); //Alt
					MemoryStream ms = new MemoryStream();
					this.graphics[i].Save(ms);
					bw.Write((int)ms.Length); //Länge der Einzeldatei in Byte
					bw.Write(0x46646E7C); //Datum
					ms.WriteTo(bw.BaseStream);
					ms.Dispose();
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
