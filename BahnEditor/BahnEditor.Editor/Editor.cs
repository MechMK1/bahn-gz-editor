using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BahnEditor.BahnLib;

namespace BahnEditor.Editor
{
	public partial class Editor : Form
	{
		private Graphic graphic;

		public Editor()
		{
			InitializeComponent();
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			List<Layer> layers = new List<Layer>();
			Pixel[,] element = new Pixel[Constants.SYMHOEHE, Constants.SYMBREITE];
			for (int i = 0; i < Constants.SYMHOEHE; i++)
			{
				for (int j = 0; j < Constants.SYMBREITE; j++)
				{
					//element[i, j] = Pixel.TransparentPixel();
					if (i == 2)
					{
						element[i, j] = Pixel.TransparentPixel();
					}
					else if (j == 3)
					{
						element[i, j] = Pixel.RGBPixel(200, 200, 200);
					}
					else
					{
						element[i, j] = Pixel.RGBPixel(80, 90, 100);
					}
				}
			}
			Layer l = new Layer((short)Constants.SYMHOEHE, (short)Constants.SYMBREITE, 0, 0, Constants.GFY_Z_VG, element);
			layers.Add(l);
			this.graphic = new Graphic("Test", 1, Pixel.RGBPixel(100, 100, 100), layers);
			this.drawPanel.Invalidate();
		}

		private void drawPanel_Paint(object sender, PaintEventArgs e)
		{
			if(this.graphic == null)
			{
				return;
			}//transparent 0, 112, 0
			//e.Graphics.DrawLine(new Pen(Brushes.Red), new Point(2, 2), new Point(5, 5));
			//e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), 30, 30, 20, 20);
			for (int i = 0; i < this.graphic.Layers[0].Element.GetLength(0); i++)
			{
				for (int j = 0; j < this.graphic.Layers[0].Element.GetLength(1); j++)
				{
					SolidBrush sb;
					if(this.graphic.Layers[0].Element[i,j].IsTransparent == true)
					{
						sb = new SolidBrush(Color.FromArgb(0, 112, 0));
					}
					else
					{
						sb = new SolidBrush(Color.FromArgb(this.graphic.Layers[0].Element[i, j].Red, this.graphic.Layers[0].Element[i, j].Green, this.graphic.Layers[0].Element[i, j].Blue));
					}
					e.Graphics.FillRectangle(sb, j * 10 + 20, (20 + 10 * this.graphic.Layers[0].Element.GetLength(0)) - (10 * i), 10, 10);
				}
			}
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			this.loadFileDialog.ShowDialog();
		}

		private void loadFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.graphic = Graphic.Load(this.loadFileDialog.FileName);
			this.drawPanel.Invalidate();
		}
	}
}
