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
		private Pixel[,] actualElement;
		private string lastPath = "";
		private Pixel leftColor = Pixel.RGBPixel(0, 0, 0);
		private Pixel rightColor = Pixel.RGBPixel(255, 255, 255);

		public Editor()
		{
			InitializeComponent();
		}

		private void NewGraphic()
		{
			List<Layer> layers = new List<Layer>();
			this.actualElement = new Pixel[Constants.SYMHOEHE, Constants.SYMBREITE];
			for (int i = 0; i < Constants.SYMHOEHE; i++)
			{
				for (int j = 0; j < Constants.SYMBREITE; j++)
				{
					this.actualElement[i, j] = Pixel.TransparentPixel();
				}
			}
			//Layer l = new Layer((short)Constants.SYMHOEHE, (short)Constants.SYMBREITE, 0, 0, Constants.GFY_Z_VG, element);
			//layers.Add(l);
			this.graphic = new Graphic("Test", 1, Pixel.RGBPixel(100, 100, 100), layers);
			this.drawPanel.Invalidate();
		}

		private void LoadGraphic()
		{
			this.graphic = Graphic.Load(this.loadFileDialog.FileName);
			this.LoadActualElement();
			this.drawPanel.Invalidate();
		}

		private void SaveGraphic()
		{
			this.graphic.Layers.Clear();
			short x0 = 0;
			short y0 = 0;
			Pixel[,] element = this.TrimActualElement(out x0, out y0);
			Layer layer = new Layer((short)element.GetLength(0), (short)element.GetLength(1), x0, y0, Constants.GFY_Z_VG, element);
			this.graphic.Layers.Add(layer);
			this.graphic.Save(lastPath, true);
		}

		private void SaveClick(bool forceSave)
		{
			if (lastPath == "" || forceSave == true)
			{
				this.saveFileDialog.ShowDialog();
			}
			else
			{
				this.SaveGraphic();
			}
		}

		private void PaintGraphic(Graphics g)
		{
			if (this.graphic == null)
			{
				return;
			}//transparent 0, 112, 0
			for (int i = 0; i < this.actualElement.GetLength(0); i++)
			{
				for (int j = 0; j < this.actualElement.GetLength(1); j++)
				{
					SolidBrush sb;
					if (this.actualElement[i, j].IsTransparent == true)
					{
						sb = new SolidBrush(Color.FromArgb(0, 112, 0));
					}
					else
					{
						sb = new SolidBrush(Color.FromArgb(this.actualElement[i, j].Red, this.actualElement[i, j].Green, this.actualElement[i, j].Blue));
					}
					g.FillRectangle(sb, j * 10 + 20, (20 + 10 * this.actualElement.GetLength(0)) - (10 * i), 10, 10);
				}
			}
		}

		private void LoadActualElement()
		{
			this.actualElement = new Pixel[Constants.SYMHOEHE, Constants.SYMBREITE];
			Layer layer = this.graphic.Layers[0];
			for (int i = 0; i < Constants.SYMHOEHE; i++)
			{
				for (int j = 0; j < Constants.SYMBREITE; j++)
				{
					if (i >= layer.Y0 && i < layer.Y0 + layer.Height && j >= layer.X0 && j < layer.X0 + layer.Width)
					{
						this.actualElement[i, j] = layer.Element[i - layer.Y0, j - layer.X0];
					}
					else
					{
						this.actualElement[i, j] = Pixel.TransparentPixel();
					}
				}
			}
		}

		private Pixel[,] TrimActualElement(out short x0, out short y0)
		{
			int minx = this.actualElement.GetLength(1);
			int miny = this.actualElement.GetLength(0);
			int maxx = 0;
			int maxy = 0;
			for (int i = 0; i < this.actualElement.GetLength(0); i++)
			{
				for (int j = 0; j < this.actualElement.GetLength(1); j++)
				{
					if (this.actualElement[i, j].IsTransparent == false)
					{
						if (minx > j)
						{
							minx = j;
						}
						if (maxx < j)
						{
							maxx = j;
						}
						if (miny > i)
						{
							miny = i;
						}
						if (maxy < i)
						{
							maxy = i;
						}
					}
				}
			}
			maxx++;
			maxy++;
			Pixel[,] element = new Pixel[maxy - miny, maxx - minx];
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					element[i, j] = this.actualElement[i + miny, j + minx];
				}
			}
			x0 = (short)minx;
			y0 = (short)miny;
			return element;
		}

		#region Event-Handler
		private void newButton_Click(object sender, EventArgs e)
		{
			this.NewGraphic();
		}

		private void drawPanel_Paint(object sender, PaintEventArgs e)
		{
			this.PaintGraphic(e.Graphics);
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			this.loadFileDialog.ShowDialog();
		}

		private void loadFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.LoadGraphic();
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			this.SaveClick(false);
		}

		private void newMenuItem_Click(object sender, EventArgs e)
		{
			this.NewGraphic();
		}

		private void openMenuItem_Click(object sender, EventArgs e)
		{
			this.loadFileDialog.ShowDialog();
		}

		private void saveMenuItem_Click(object sender, EventArgs e)
		{
			this.SaveClick(false);
		}

		private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.lastPath = this.saveFileDialog.FileName;
			this.SaveGraphic();
		}

		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			this.SaveClick(true);
		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void leftColorButton_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.colorDialog.ShowDialog();
			if(dr == DialogResult.OK)
			{
				Color c = this.colorDialog.Color;
				this.leftColor = Pixel.RGBPixel(c.R, c.G, c.B);
				this.leftColorButton.BackColor = c;
			}
		}

		private void rightColorButton_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.colorDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				Color c = this.colorDialog.Color;
				this.rightColor = Pixel.RGBPixel(c.R, c.G, c.B);
				this.rightColorButton.BackColor = c;
			}
		}
		#endregion
	}
}
