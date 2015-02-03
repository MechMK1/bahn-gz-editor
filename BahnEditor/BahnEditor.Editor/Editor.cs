﻿using System;
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
		private int zoomLevel = 3;
		private bool userMadeChanges = false;
		private Pixel leftColor = Pixel.RGBPixel(0, 0, 0);
		private Pixel rightColor = Pixel.RGBPixel(255, 255, 255);


		public Editor()
		{
			InitializeComponent();
		}

		private void NewGraphic()
		{
			List<Layer> layers = new List<Layer>();
			this.actualElement = new Pixel[Constants.SYMHOEHE * 8, Constants.SYMBREITE * 3];
			for (int i = 0; i < this.actualElement.GetLength(0); i++)
			{
				for (int j = 0; j < this.actualElement.GetLength(1); j++)
				{
					this.actualElement[i, j] = Pixel.TransparentPixel();
				}
			}
			//Layer l = new Layer((short)Constants.SYMHOEHE, (short)Constants.SYMBREITE, 0, 0, Constants.GFY_Z_VG, element);
			//layers.Add(l);
			this.graphic = new Graphic("Test", 1, Pixel.RGBPixel(100, 100, 100), layers);
			this.ResizeDrawPanel();
			this.drawPanel.Invalidate();
		}

		private void LoadGraphic()
		{
			this.graphic = Graphic.Load(this.loadFileDialog.FileName);
			this.LoadActualElement();
			this.ResizeDrawPanel();
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
			this.userMadeChanges = false;
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

		private void ResizeDrawPanel()
		{
			this.drawPanel.AutoScrollMinSize = new Size((int)((2 * this.zoomLevel) * Constants.SYMBREITE * 3 + 30), (int)((2 * this.zoomLevel) * Constants.SYMHOEHE * 8 + 30));
		}

		private void PaintGraphic(Graphics g)
		{
			if (this.graphic == null || this.actualElement == null)
			{
				return;
			}
			g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);
			for (int i = 0; i < this.actualElement.GetLength(0); i++)
			{
				for (int j = 0; j < this.actualElement.GetLength(1); j++)
				{
					SolidBrush sb;
					if (this.actualElement[i, j].IsTransparent == true)
					{
						sb = new SolidBrush(Color.FromArgb(0, 112, 0)); //transparent 0, 112, 0
					}
					else
					{
						sb = new SolidBrush(Color.FromArgb(this.actualElement[i, j].Red, this.actualElement[i, j].Green, this.actualElement[i, j].Blue));
					}
					g.FillRectangle(sb, j * (2 * this.zoomLevel) + 20, (((2 * this.zoomLevel) * this.actualElement.GetLength(0)) - ((2 * this.zoomLevel) * (i + 1))) + 20, (2 * this.zoomLevel), (2 * this.zoomLevel));

				}
			}
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					g.DrawRectangle(Pens.Gray, 20 + (i * Constants.SYMBREITE) * (2 * this.zoomLevel), 20 + (j * Constants.SYMHOEHE) * (2 * this.zoomLevel), Constants.SYMBREITE * (2 * this.zoomLevel), Constants.SYMHOEHE * (2 * this.zoomLevel));
				}
			}
			g.DrawRectangle(Pens.DarkGray, 20 + Constants.SYMBREITE * (2 * this.zoomLevel), 20 + (6 * Constants.SYMHOEHE) * (2 * this.zoomLevel), Constants.SYMBREITE * (2 * this.zoomLevel), Constants.SYMHOEHE * (2 * this.zoomLevel));
		}

		private void MouseClickGraphic(MouseEventArgs e)
		{
			int xElement = (e.X - 20 - drawPanel.AutoScrollPosition.X) / (2 * this.zoomLevel);
			int yElement = ((10 + (2 * this.zoomLevel) * this.actualElement.GetLength(0)) - (e.Y - 10 - drawPanel.AutoScrollPosition.Y)) / (2 * this.zoomLevel);
			if (xElement >= 0 && yElement >= 0 && xElement < this.actualElement.GetLength(1) && yElement < this.actualElement.GetLength(0))
			{
				if (e.Button == MouseButtons.Left)
				{
					this.actualElement[yElement, xElement] = leftColor;
				}
				else if (e.Button == MouseButtons.Right)
				{
					this.actualElement[yElement, xElement] = rightColor;
				}
				this.userMadeChanges = true;
				drawPanel.Invalidate();
			}
		}

		private bool ExitEditor()
		{
			if (this.userMadeChanges == true)
			{
				DialogResult dr = MessageBox.Show("Soll die Grafik gespeichert werden?", "Speichern", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dr == DialogResult.Cancel)
				{
					return true;
				}
				else if (dr == DialogResult.Yes)
				{
					this.SaveClick(false);
				}
			}
			return false;
		}

		private void LoadActualElement()
		{
			this.actualElement = new Pixel[Constants.SYMHOEHE * 8, Constants.SYMBREITE * 3];
			Layer layer = this.graphic.Layers[0];
			int x0 = (int)(layer.X0 + Constants.SYMBREITE);
			int y0 = (int)(layer.Y0 + Constants.SYMHOEHE);
			for (int i = 0; i < this.actualElement.GetLength(0); i++)
			{
				for (int j = 0; j < this.actualElement.GetLength(1); j++)
				{
					if (i >= y0 && i < y0 + layer.Height && j >= x0 && j < x0 + layer.Width)
					{
						this.actualElement[i, j] = layer.Element[i - y0, j - x0];
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
			x0 = (short)(minx - Constants.SYMBREITE);
			y0 = (short)(miny - Constants.SYMHOEHE);
			return element;
		}


		#region Event-Handler
		private void drawPanel_Paint(object sender, PaintEventArgs e)
		{
			this.PaintGraphic(e.Graphics);
		}

		private void drawPanel_MouseClick(object sender, MouseEventArgs e)
		{
			this.MouseClickGraphic(e);
		}

		private void newButton_Click(object sender, EventArgs e)
		{
			this.NewGraphic();
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
			if (dr == DialogResult.OK)
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

		private void Editor_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = this.ExitEditor();
		}

		private void zoomTrackBar_Scroll(object sender, EventArgs e)
		{
			this.zoomLevel = this.zoomTrackBar.Value;
			this.ResizeDrawPanel();
			this.drawPanel.Invalidate();
		}
		#endregion

	}
}
