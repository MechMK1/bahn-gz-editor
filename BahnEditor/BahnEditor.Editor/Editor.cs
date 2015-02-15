using BahnEditor.BahnLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace BahnEditor.Editor
{
	public partial class Editor : Form
	{
		private Graphic graphic;
		private int actualLayer;
		private string lastPath = "";
		private int zoomLevel = 6;
		private bool userMadeChanges = false;
		private Pixel leftColor = Pixel.RGBPixel(0, 0, 0);
		private Pixel lastLeftColor = null;
		private Pixel rightColor = Pixel.RGBPixel(255, 255, 255);
		private Pixel lastRightColor = null;

		private static SolidBrush transparentBrush = new SolidBrush(Color.FromArgb(0, 112, 0));

		public Editor()
		{
			InitializeComponent();
		}

		private void EditorLoaded()
		{
			this.rightComboBox.SelectedIndex = 0;
			this.leftComboBox.SelectedIndex = 0;
		}

		private void NewGraphic()
		{
			List<Layer> layers = new List<Layer>();
			Pixel[,] element = NewElement();
			Layer l = new Layer(Constants.LAYER_VG, element);
			layers.Add(l);
			this.graphic = new Graphic("Test", 1, Pixel.RGBPixel(100, 100, 100), layers);
			this.drawPanel.Visible = true;
			this.controlPanel.Visible = true;
			this.layerComboBox.SelectedIndex = 0;
			this.ResizeDrawPanel();
			this.drawPanel.AutoScrollPosition = new Point(this.drawPanel.HorizontalScroll.Maximum, this.drawPanel.VerticalScroll.Maximum);
			this.drawPanel.Invalidate();
		}

		private static Pixel[,] NewElement()
		{
			Pixel[,] element = new Pixel[Constants.SYMHOEHE * 8, Constants.SYMBREITE * 3];
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					element[i, j] = Pixel.TransparentPixel();
				}
			}
			return element;
		}

		private void LoadGraphic()
		{
			this.graphic = Graphic.Load(this.loadFileDialog.FileName);
			this.drawPanel.Visible = true;
			this.controlPanel.Visible = true;
			this.ResizeDrawPanel();
			this.drawPanel.AutoScrollPosition = new Point(this.drawPanel.HorizontalScroll.Maximum, this.drawPanel.VerticalScroll.Maximum);
			this.drawPanel.Invalidate();
		}

		private void SaveGraphic()
		{
			try
			{
				this.graphic.Save(lastPath, true);
			}
			catch (ElementIsEmptyException)
			{
				MessageBox.Show("Element ist leer!", "Element ist leer!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			this.userMadeChanges = false;
		}

		private void ClickOnSaveButton(bool forceSave)
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
			this.drawPanel.AutoScrollMinSize = new Size((int)((this.zoomLevel) * Constants.SYMBREITE * 3 + 30), (int)((this.zoomLevel) * Constants.SYMHOEHE * 8 + 30));
		}

		private void PaintGraphic(Graphics g)
		{
			if (this.actualLayer < 0)
			{
				g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);
				g.FillRectangle(transparentBrush, 20, 20, Constants.SYMBREITE * this.zoomLevel * 3, Constants.SYMHOEHE * this.zoomLevel * 8); //transparent 0, 112, 0
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						g.DrawRectangle(Pens.Gray, 20 + (i * Constants.SYMBREITE) * (this.zoomLevel), 20 + (j * Constants.SYMHOEHE) * (this.zoomLevel), Constants.SYMBREITE * (this.zoomLevel), Constants.SYMHOEHE * (this.zoomLevel));
					}
				}
				g.DrawRectangle(Pens.DarkGray, 20 + Constants.SYMBREITE * (this.zoomLevel), 20 + (6 * Constants.SYMHOEHE) * (this.zoomLevel), Constants.SYMBREITE * (this.zoomLevel), Constants.SYMHOEHE * (this.zoomLevel));
				return;
			}

			if (this.graphic == null || this.graphic.Layers == null || this.graphic.Layers.Count == 0 || this.graphic.Layers[this.actualLayer] == null || this.graphic.Layers[this.actualLayer].Element == null)
			{
				return;
			}

			if (g.ClipBounds.Width == this.zoomLevel * 2 && g.ClipBounds.Height == this.zoomLevel * 2)
			{
				int x = (int)g.ClipBounds.X;
				int y = (int)g.ClipBounds.Y;
				int diffX = 0;
				int diffY = 0;
				if (x - drawPanel.AutoScrollPosition.X < 20)
				{
					x = 20;
				}
				else if (x - 20 - drawPanel.AutoScrollPosition.X + this.zoomLevel * 2 >= Constants.SYMBREITE * 3 * this.zoomLevel)
				{
					//x = (int)Constants.SYMBREITE * 3 * this.zoomLevel;
					diffX = (int)((x - 20 - drawPanel.AutoScrollPosition.X + this.zoomLevel * 2) - (Constants.SYMBREITE * 3 * this.zoomLevel));
				}
				if (y - drawPanel.AutoScrollPosition.Y < 20)
				{
					y = 20;
				}
				else if (y - 20 - drawPanel.AutoScrollPosition.Y + this.zoomLevel * 2 >= Constants.SYMHOEHE * 8 * this.zoomLevel)
				{
					diffY = (int)((y - 20 - drawPanel.AutoScrollPosition.Y + this.zoomLevel * 2) - (Constants.SYMHOEHE * 8 * this.zoomLevel));
				}
				g.FillRectangle(transparentBrush, x, y, this.zoomLevel * 2 - diffX, this.zoomLevel * 2 - diffY);
				int xElement = ((x - 20 - drawPanel.AutoScrollPosition.X) / (this.zoomLevel));
				int yElement = (((10 + (this.zoomLevel) * this.graphic.Layers[this.actualLayer].Element.GetLength(0)) - (y - 10 - drawPanel.AutoScrollPosition.Y)) / (this.zoomLevel));
				g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);
				if (yElement >= this.graphic.Layers[this.actualLayer].Element.GetLength(0))
				{
					yElement = this.graphic.Layers[this.actualLayer].Element.GetLength(0) - 1;
				}
				if (xElement >= this.graphic.Layers[this.actualLayer].Element.GetLength(1))
				{
					xElement = this.graphic.Layers[this.actualLayer].Element.GetLength(1) - 1;
				}
				for (int i = yElement; (i >= yElement - 3) && i < this.graphic.Layers[this.actualLayer].Element.GetLength(0) && i >= 0; i--)
				{
					for (int j = xElement; (j < xElement + 3) && j < this.graphic.Layers[this.actualLayer].Element.GetLength(1); j++)
					{
						if (this.graphic.Layers[this.actualLayer].Element[i, j].IsTransparent != true)
						{
							g.FillRectangle(new SolidBrush(Color.FromArgb(this.graphic.Layers[this.actualLayer].Element[i, j].Red, this.graphic.Layers[this.actualLayer].Element[i, j].Green, this.graphic.Layers[this.actualLayer].Element[i, j].Blue)), j * (this.zoomLevel) + 20, (((this.zoomLevel) * this.graphic.Layers[this.actualLayer].Element.GetLength(0)) - ((this.zoomLevel) * (i + 1))) + 20, (this.zoomLevel), (this.zoomLevel));
						}
					}
				}
			}
			else
			{
				g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);
				g.FillRectangle(transparentBrush, 20, 20, Constants.SYMBREITE * this.zoomLevel * 3, Constants.SYMHOEHE * this.zoomLevel * 8); //transparent 0, 112, 0
				for (int i = 0; i < this.graphic.Layers[this.actualLayer].Element.GetLength(0); i++)
				{
					for (int j = 0; j < this.graphic.Layers[this.actualLayer].Element.GetLength(1); j++)
					{
						if (this.graphic.Layers[this.actualLayer].Element[i, j].IsTransparent != true)
						{
							g.FillRectangle(new SolidBrush(Color.FromArgb(this.graphic.Layers[this.actualLayer].Element[i, j].Red, this.graphic.Layers[this.actualLayer].Element[i, j].Green, this.graphic.Layers[this.actualLayer].Element[i, j].Blue)), j * (this.zoomLevel) + 20, (((this.zoomLevel) * this.graphic.Layers[this.actualLayer].Element.GetLength(0)) - ((this.zoomLevel) * (i + 1))) + 20, (this.zoomLevel), (this.zoomLevel));
						}
					}
				}
			}
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					g.DrawRectangle(Pens.Gray, 20 + (i * Constants.SYMBREITE) * (this.zoomLevel), 20 + (j * Constants.SYMHOEHE) * (this.zoomLevel), Constants.SYMBREITE * (this.zoomLevel), Constants.SYMHOEHE * (this.zoomLevel));
				}
			}
			g.DrawRectangle(Pens.DarkGray, 20 + Constants.SYMBREITE * (this.zoomLevel), 20 + (6 * Constants.SYMHOEHE) * (this.zoomLevel), Constants.SYMBREITE * (this.zoomLevel), Constants.SYMHOEHE * (this.zoomLevel));
		}

		private void MouseClickGraphic(MouseEventArgs e)
		{
			if (this.actualLayer < 0)
			{
				Pixel[,] element = NewElement();
				int layerID = GetLayerIDByIndex();
				Layer layer = new Layer((short)layerID, element);
				this.graphic.Layers.Add(layer);
				this.actualLayer = this.graphic.Layers.FindIndex(x => x.LayerID == layerID);
			}
			if (e.Button != MouseButtons.None && this.graphic != null && this.graphic.Layers != null && this.graphic.Layers.Count > 0 && this.graphic.Layers[this.actualLayer] != null && this.graphic.Layers[this.actualLayer].Element != null)
			{
				if (e.X - drawPanel.AutoScrollPosition.X >= 20 && e.Y - drawPanel.AutoScrollPosition.Y >= 20 && (20 + this.zoomLevel * this.graphic.Layers[this.actualLayer].Element.GetLength(0) + drawPanel.AutoScrollPosition.Y - e.Y) > 0)
				{
					int xElement = (e.X - 20 - drawPanel.AutoScrollPosition.X) / (this.zoomLevel);
					int yElement = (20 + this.zoomLevel * this.graphic.Layers[this.actualLayer].Element.GetLength(0) + drawPanel.AutoScrollPosition.Y - e.Y) / this.zoomLevel;
					if (xElement >= 0 && yElement >= 0 && xElement < this.graphic.Layers[this.actualLayer].Element.GetLength(1) && yElement < this.graphic.Layers[this.actualLayer].Element.GetLength(0))
					{
						if (e.Button == MouseButtons.Left && this.graphic.Layers[this.actualLayer].Element[yElement, xElement] != leftColor)
						{
							this.graphic.Layers[this.actualLayer].Element[yElement, xElement] = leftColor;

						}
						else if (e.Button == MouseButtons.Right && this.graphic.Layers[this.actualLayer].Element[yElement, xElement] != rightColor)
						{
							this.graphic.Layers[this.actualLayer].Element[yElement, xElement] = rightColor;
						}
						else
						{
							return;
						}
						this.userMadeChanges = true;
						drawPanel.Invalidate(new Rectangle(e.X - this.zoomLevel, e.Y - this.zoomLevel, this.zoomLevel * 2, this.zoomLevel * 2));
					}
				}
			}
		}

		private void SelectLayer()
		{
			int index = this.GetLayerIDByIndex();
			this.actualLayer = this.graphic.Layers.FindIndex(x => x.LayerID == index);
			this.drawPanel.Invalidate();
		}

		private int GetLayerIDByIndex()
		{
			switch (this.layerComboBox.SelectedIndex)
			{
				case 0:
					return Constants.LAYER_VG;
				case 1:
					return Constants.LAYER_HG;
				case 2:
					return Constants.LAYER_FL;
				case 3:
					return Constants.LAYER_VO;
				case 4:
					return Constants.LAYER_VB;
				default:
					return -1;
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
					this.ClickOnSaveButton(false);
				}
			}
			return false;
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
			this.ClickOnSaveButton(false);
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
			this.ClickOnSaveButton(false);
		}

		private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.lastPath = this.saveFileDialog.FileName;
			this.SaveGraphic();
		}

		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(true);
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
				this.leftColor = Pixel.FromColor(c);
				this.leftColorButton.BackColor = c;
			}
		}

		private void rightColorButton_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.colorDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				Color c = this.colorDialog.Color;
				this.rightColor = Pixel.FromColor(c);
				this.rightColorButton.BackColor = c;
			}
		}

		private void Editor_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = this.ExitEditor();
		}

		private void zoomTrackBar_Scroll(object sender, EventArgs e)
		{
			this.zoomLevel = this.zoomTrackBar.Value * 2;
			this.ResizeDrawPanel();
			this.drawPanel.Invalidate();
		}

		private void drawPanel_MouseMove(object sender, MouseEventArgs e)
		{
			this.MouseClickGraphic(e);
		}

		private void rightComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.rightComboBox.SelectedIndex == 0)
			{
				if (this.lastRightColor != null)
				{
					this.rightColor = this.lastRightColor;

				}
			}
			else if (this.rightComboBox.SelectedIndex == 1)
			{
				if (this.rightColor.IsTransparent != true)
				{
					this.lastRightColor = this.rightColor;
					this.rightColor = Pixel.TransparentPixel();
				}
			}
			this.rightColorButton.BackColor = this.rightColor.ConvertToColor();
		}

		private void leftComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.leftComboBox.SelectedIndex == 0)
			{
				if (this.lastLeftColor != null)
				{
					this.leftColor = this.lastLeftColor;
				}
			}
			else if (this.leftComboBox.SelectedIndex == 1)
			{
				if (this.leftColor.IsTransparent != true)
				{
					this.lastLeftColor = this.leftColor;
					this.leftColor = Pixel.TransparentPixel();
				}
			}
			this.leftColorButton.BackColor = this.leftColor.ConvertToColor();
		}

		private void Editor_Load(object sender, EventArgs e)
		{
			this.EditorLoaded();
		}

		private void layerComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		#endregion
	}
}
