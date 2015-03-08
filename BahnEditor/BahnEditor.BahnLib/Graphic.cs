using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BahnEditor.BahnLib
{
	public abstract class Graphic
	{
		private List<Layer> layers;

		public byte ZoomFactor { get; protected set; }
		public string InfoText { get; set; }
		public Pixel ColorInSchematicMode { get; set; }
		public int Bauform { get; set; }
		public int FwSig { get; set; }
		public int Phase { get; set; }
		public int Alt { get; set; }

		protected Graphic(string infoText, byte zoomFactor, Pixel colorInSchematicMode)
		{
			this.ZoomFactor = zoomFactor;
			this.InfoText = infoText;
			this.ColorInSchematicMode = colorInSchematicMode;
			layers = new List<Layer>();
		}

		public void AddTransparentLayer(short layerID)
		{
			Pixel[,] element = new Pixel[Constants.SYMHOEHE * 8 * this.ZoomFactor, Constants.SYMBREITE * 3 * this.ZoomFactor];
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					element[i, j] = Pixel.TransparentPixel();
				}
			}
			Layer layer = new Layer(layerID, element);
			this.AddLayer(layer);
		}

		public void AddLayer(Layer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			this.layers.Add(layer);
		}

		public Layer GetLayer(int index)
		{
			return layers[index];
		}

		public Layer GetLayerByID(short id)
		{
			return layers.Find(x => x.LayerID == id);
		}

		public int GetIndexByID(short id)
		{
			return layers.FindIndex(x => x.LayerID == id);
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

		internal static Graphic Load(Stream path)
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
				if ((settings & 0x0008) != 0)
				{
					br.ReadInt32();
					br.ReadInt32();
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
				Graphic graphic = null;
				if (zoomFactor == 1)
				{
					graphic = new Zoom1Graphic(infoText, colorInSchematicMode);
				}
				else if (zoomFactor == 2)
				{
					graphic = new Zoom2Graphic(infoText, colorInSchematicMode);
				}
				else if (zoomFactor == 4)
				{
					graphic = new Zoom4Graphic(infoText, colorInSchematicMode);
				}
				else
				{
					throw new Exception("unknown zoom factor");
				}
				//List<Layer> layers = new List<Layer>();
				for (int i = 0; i < layer; i++)
				{
					graphic.AddLayer(Layer.ReadLayerFromStream(br));
				}
				return graphic;
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

		internal bool Save(Stream path)
		{
			try
			{
				if (this.ValidateElement())
					throw new ElementIsEmptyException("element is empty");
				BinaryWriter bw = new BinaryWriter(path, Encoding.Unicode);
				int layer = this.layers.Count;
				bw.Write(Constants.HEADERTEXT.ToArray()); //Headertext 
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
					this.layers[i].WriteLayerToStream(bw);
				}
				bw.Flush();
				return true;

			}
			catch (ElementIsEmptyException)
			{
				throw;
			}
			catch (Exception) //TODO Exchange general exceptions with specific ones
			{
				throw;
			}
		}

		public bool ValidateElement()
		{
			foreach (var item in layers)
			{
				for (int i = 0; i < item.Element.GetLength(0); i++)
				{
					for (int j = 0; j < item.Element.GetLength(1); j++)
					{
						if (item.Element[i, j].IsTransparent == false)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
