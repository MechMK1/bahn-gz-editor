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
		public byte ZoomFactor { get; set; }
		public string InfoText { get; set; }
		public Pixel ColorInSchematicMode { get; set; }
		public List<Layer> Layers { get; set; }

		public Graphic(string infoText, byte zoomFactor, Pixel colorInSchematicMode, List<Layer> layers)
		{
			this.ZoomFactor = zoomFactor;
			this.InfoText = infoText;
			this.ColorInSchematicMode = colorInSchematicMode;
			Layers = layers;
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
				Pixel colorInSchematicMode = Pixel.RGBPixel(100, 100, 100);
				if ((settings & 0x20) != 0)
				{
					colorInSchematicMode = Pixel.FromUInt(br.ReadUInt32());
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
				List<Layer> layers = new List<Layer>();
				for (int i = 0; i < layer; i++)
				{
					layers.Add(Layer.ReadLayerFromStream(br));
				}
				return new Graphic(infoText, zoomFactor, colorInSchematicMode, layers);
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
					int layer = this.Layers.Count;
					bw.Write(new byte[] { 67, 114, 101, 97, 116, 101, 100, 32, 98, 121, 32, 71, 90, 45, 69, 100, 105, 116 }); //Text "Testdatei"
					bw.Write((byte)26); // text end
					bw.Write(new byte[] { 71, 90, 71 }); //identification string GZG ASCII
					bw.Write((byte)(48 + this.ZoomFactor)); //Zoom faktor ASCII
					bw.Write((byte)0x03); //version
					bw.Write((byte)0x84); //version
					bw.Write((byte)0x00); //subversion
					bw.Write((byte)0x05); //subversion
					bw.Write((int)0x0220); //Gzg_Eig 
					bw.Write(this.ColorInSchematicMode.ConvertToUInt()); //kfarbe
					bw.Write(0x80000001);
					bw.Write((short)layer); //layer
					bw.Write((ushort)0xFFFE);
					bw.Write(this.InfoText.ToCharArray());
					bw.Write(Constants.UNICODE_NULL);
					for (int i = 0; i < layer; i++)
					{
						this.Layers[i].WriteLayerToStream(bw);
					}
					bw.Flush();
					bw.Close();
					return true;
				}

			}
			catch (Exception) //TODO Exchange general exceptions with specific ones
			{
				throw;
			}
		}
	}
}
