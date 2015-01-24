using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public class Graphic
	{
		public uint[,] GraphicArray { get; set; }
		public byte ZoomFactor { get; set; }
		public string InfoText { get; set; }
		public short Height { get; set; }
		public short Width { get; set; }
		public short StartHeight { get; set; }
		public short StartWidth { get; set; }
		public uint ColorSchematic { get; set; }

		public Graphic(string infoText, byte zoomFactor, short height, short width, short startHeight, short startWidth)
		{
			if (height + startHeight > Constants.SYMHOEHE || width + startWidth > Constants.SYMBREITE)
			{
				throw new Exception("height or width out of range");
			}
			this.ZoomFactor = zoomFactor;
			this.InfoText = infoText;
			this.Height = height;
			this.Width = width;
			this.StartHeight = startHeight;
			this.StartWidth = startWidth;
			this.GraphicArray = new uint[height, width];
			for (int i = 0; i < this.GraphicArray.GetLength(0); i++)
			{
				for (int j = 0; j < this.GraphicArray.GetLength(1); j++)
				{
					this.GraphicArray[i, j] = Color.FromRGB(100, 100, 100);
				}
			}
		}

		public static Graphic Load(string path)
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

		private static Graphic Load(FileStream path)
		{
			using (BinaryReader br = new BinaryReader(path, Encoding.Unicode))
			{
				while (br.ReadByte() != 26) { }
				byte[] read = br.ReadBytes(3);
				if (read[0] != 71 || read[1] != 90 || read[2] != 71)
				{
					throw new Exception("wrong identification string");
				}
				byte zoomFactor = (byte)(br.ReadByte() - 48);
				read = null;
				read = br.ReadBytes(4);
				if (read[0] != 0x03 || read[1] != 0x84 || read[2] != 0x00 || read[3] != 0x05)
				{
					throw new Exception("wrong version");
				}
				int settings = br.ReadInt32();
				uint colorSchematic = 0;
				if ((settings & 0x20) != 0)
				{
					colorSchematic = br.ReadUInt32();
					br.ReadUInt32();
				}
				short layer = br.ReadInt16();
				br.ReadUInt16();
				char c;
				StringBuilder sb = new StringBuilder();
				while ((c = br.ReadChar()) != Constants.UNICODE_NULL)
				{
					sb.Append(c);
				}
				string infoText = sb.ToString();
				short startWidth = 0;
				short startHeight = 0;
				short width = 0;
				short height = 0;
				List<uint> data = new List<uint>();
				for (int i = 0; i < layer; i++)
				{
					br.ReadInt16();
					startWidth = br.ReadInt16();
					startHeight = br.ReadInt16();
					width = br.ReadInt16();
					height = br.ReadInt16();
					for (int j = 0; j < width-1 * height-1; j++)
					{
						data.Add(br.ReadUInt32());
					}
				}
				return new Graphic(infoText, zoomFactor, height, width, startHeight, startWidth);
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
			try
			{
				using (BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode))
				{
					int layer = 1;
					bw.Write(new byte[] { 67, 114, 101, 97, 116, 101, 100, 32, 98, 121, 32, 71, 90, 45, 69, 100, 105, 116 }); //Text "Testdatei"
					bw.Write((byte)26); // text end
					bw.Write(new byte[] { 71, 90, 71 }); //identification string GZG ASCII
					bw.Write((byte)(48 + this.ZoomFactor)); //Zoom faktor ASCII
					bw.Write((byte)0x03); //version
					bw.Write((byte)0x84); //version
					bw.Write((byte)0x00); //subversion
					bw.Write((byte)0x05); //subversion
					bw.Write((int)0x0220); //Gzg_Eig 
					bw.Write(Color.FromRGB(100, 100, 100)); //kfarbe
					bw.Write(0x80000001);
					bw.Write((short)layer); //layer
					bw.Write((ushort)0xFFFE);
					bw.Write(this.InfoText.ToCharArray());
					bw.Write(Constants.UNICODE_NULL);
					for (int i = 1; i <= layer; i++)
					{

						bw.Write((short)(i + 1)); //layer
						bw.Write((short)this.StartWidth); //x0
						bw.Write((short)this.StartHeight); //y0
						bw.Write((short)this.Width); //width
						bw.Write((short)this.Height); //height
						uint[] lines = new uint[this.Width * this.Height];
						int count = 0;
						for (int j = 0; j <= this.Height - 1; j++)
						{
							for (int k = 0; k <= this.Width - (short)1; k++)
							{
								lines[count] = this.GraphicArray[j, k];
								count++;
							}
						}

						uint[] compressed = Color.Compress(lines);
						for (int j = 0; j < compressed.Length; j++)
						{
							bw.Write(compressed[j]);
						}
					}
					bw.Flush();
					bw.Close();
					return true;
				}

			}
			catch (Exception) //TODO Exchange general exceptions with specific ones
			{
				return false;
			}
		}
	}
}
