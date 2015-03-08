using BahnEditor.BahnLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;

namespace BahnEditor.Editor
{
	public partial class Editor : Form
	{
		private Graphic graphic;
		private int actualLayer;
		private string lastPath = "";
		private int zoomLevel = 6;
		private bool userMadeChanges = false;
		private bool hasLoadedGraphic = false;
		private Pixel leftPixel = Pixel.RGBPixel(0, 0, 0);
		private Pixel lastLeftPixel = null;
		private Pixel rightPixel = Pixel.RGBPixel(255, 255, 255);
		private Pixel lastRightPixel = null;

		private static Brush transparentBrush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), Color.FromArgb(0, 112, 0)); //transparent 0, 112, 0


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
			this.graphic = new Zoom1Graphic("Test", Pixel.RGBPixel(100, 100, 100));
			this.graphic.AddTransparentLayer(Constants.LAYER_VG);
			this.drawPanel.Visible = true;
			this.controlPanel.Visible = true;
			this.layerComboBox.SelectedIndex = 0;
			this.ResizeDrawPanel();
			this.drawPanel.AutoScrollPosition = new Point(this.drawPanel.HorizontalScroll.Maximum, this.drawPanel.VerticalScroll.Maximum);
			this.drawPanel.Invalidate();
		}

		private void LoadGraphic()
		{
			try
			{
				this.graphic = Graphic.Load(this.loadFileDialog.FileName);
				this.hasLoadedGraphic = true;
				this.drawPanel.Visible = true;
				this.controlPanel.Visible = true;
				this.ResizeDrawPanel();
				this.drawPanel.AutoScrollPosition = new Point(this.drawPanel.HorizontalScroll.Maximum, this.drawPanel.VerticalScroll.Maximum / 2);
				this.drawPanel.Invalidate();
			}
			catch(Exception ex)
			{
				MessageBox.Show(String.Format("Fehler: {0}!", ex.Message), "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SaveGraphic()
		{
			try
			{
				if(!this.graphic.Save(lastPath, true))
				{
					MessageBox.Show("Fehler beim Speichern!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
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
			try
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

				if (this.graphic == null || this.graphic.GetLayer(this.actualLayer) == null || this.graphic.GetLayer(this.actualLayer).Element == null)
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
					int xElement = ((x - 20 - drawPanel.AutoScrollPosition.X) / (this.zoomLevel));
					int yElement = (((10 + (this.zoomLevel) * this.graphic.GetLayer(this.actualLayer).Element.GetLength(0)) - (y - 10 - drawPanel.AutoScrollPosition.Y)) / (this.zoomLevel));

					g.FillRectangle(transparentBrush, x, y, this.zoomLevel * 2 - diffX, this.zoomLevel * 2 - diffY);
					g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);

					if (yElement >= this.graphic.GetLayer(this.actualLayer).Element.GetLength(0))
					{
						yElement = this.graphic.GetLayer(this.actualLayer).Element.GetLength(0) - 1;
					}
					if (xElement >= this.graphic.GetLayer(this.actualLayer).Element.GetLength(1))
					{
						xElement = this.graphic.GetLayer(this.actualLayer).Element.GetLength(1) - 1;
					}

					for (int i = yElement; (i >= yElement - 3) && i < this.graphic.GetLayer(this.actualLayer).Element.GetLength(0) && i >= 0; i--)
					{
						for (int j = xElement; (j < xElement + 3) && j < this.graphic.GetLayer(this.actualLayer).Element.GetLength(1); j++)
						{
							if (this.graphic.GetLayer(this.actualLayer).Element[i, j].IsTransparent != true)
							{
								Brush brush;
								if (this.graphic.GetLayer(this.actualLayer).Element[i, j].IsSpecialColorWithoutRGB)
									brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), this.graphic.GetLayer(this.actualLayer).Element[i, j].ConvertToColor());
								else if(this.graphic.GetLayer(this.actualLayer).Element[i, j].IsSpecialColorWithRGB)
									brush = new HatchBrush(HatchStyle.Percent10, Color.FromArgb(140, 140, 140), this.graphic.GetLayer(this.actualLayer).Element[i, j].ConvertToColor());
								else
									brush = new SolidBrush(this.graphic.GetLayer(this.actualLayer).Element[i, j].ConvertToColor());
								g.FillRectangle(brush, j * (this.zoomLevel) + 20, (((this.zoomLevel) * this.graphic.GetLayer(this.actualLayer).Element.GetLength(0)) - ((this.zoomLevel) * (i + 1))) + 20, (this.zoomLevel), (this.zoomLevel));
							}
						}
					}
				}
				else
				{
					g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);
					g.FillRectangle(transparentBrush, 20, 20, Constants.SYMBREITE * this.zoomLevel * 3, Constants.SYMHOEHE * this.zoomLevel * 8);
					for (int i = 0; i < this.graphic.GetLayer(this.actualLayer).Element.GetLength(0); i++)
					{
						for (int j = 0; j < this.graphic.GetLayer(this.actualLayer).Element.GetLength(1); j++)
						{
							if (this.graphic.GetLayer(this.actualLayer).Element[i, j].IsTransparent != true)
							{
								Brush brush;
								if (this.graphic.GetLayer(this.actualLayer).Element[i, j].IsSpecialColorWithoutRGB)
									brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), this.graphic.GetLayer(this.actualLayer).Element[i, j].ConvertToColor());
								else if (this.graphic.GetLayer(this.actualLayer).Element[i, j].IsSpecialColorWithRGB)
									brush = new HatchBrush(HatchStyle.Percent10, Color.FromArgb(140, 140, 140), this.graphic.GetLayer(this.actualLayer).Element[i, j].ConvertToColor());
								else
									brush = new SolidBrush(this.graphic.GetLayer(this.actualLayer).Element[i, j].ConvertToColor());
								g.FillRectangle(brush, j * (this.zoomLevel) + 20, (((this.zoomLevel) * this.graphic.GetLayer(this.actualLayer).Element.GetLength(0)) - ((this.zoomLevel) * (i + 1))) + 20, (this.zoomLevel), (this.zoomLevel));
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
			catch (IndexOutOfRangeException)
			{

			}
		}

		private void MouseClickGraphic(MouseEventArgs e)
		{
			if (this.hasLoadedGraphic == true)
			{
				this.hasLoadedGraphic = false;
				return;
			}
			if (this.actualLayer < 0)
			{
				//Pixel[,] element = NewElement();
				short layerID = GetLayerIDBySelectedIndex();
				//Layer layer = new Layer((short)layerID, element);
				this.graphic.AddTransparentLayer(layerID);
				this.actualLayer = this.graphic.GetIndexByID(layerID);
			}
			try
			{
				if (e.Button != MouseButtons.None && this.graphic != null && this.graphic.GetLayer(this.actualLayer) != null && this.graphic.GetLayer(this.actualLayer).Element != null)
				{
					if (e.X - drawPanel.AutoScrollPosition.X >= 20 && e.Y - drawPanel.AutoScrollPosition.Y >= 20 && (20 + this.zoomLevel * this.graphic.GetLayer(this.actualLayer).Element.GetLength(0) + drawPanel.AutoScrollPosition.Y - e.Y) > 0)
					{
						int xElement = (e.X - 20 - drawPanel.AutoScrollPosition.X) / (this.zoomLevel);
						int yElement = (20 + this.zoomLevel * this.graphic.GetLayer(this.actualLayer).Element.GetLength(0) + drawPanel.AutoScrollPosition.Y - e.Y) / this.zoomLevel;
						if (xElement >= 0 && yElement >= 0 && xElement < this.graphic.GetLayer(this.actualLayer).Element.GetLength(1) && yElement < this.graphic.GetLayer(this.actualLayer).Element.GetLength(0))
						{
							if (e.Button == MouseButtons.Left && this.graphic.GetLayer(this.actualLayer).Element[yElement, xElement] != leftPixel)
							{
								this.graphic.GetLayer(this.actualLayer).Element[yElement, xElement] = leftPixel;

							}
							else if (e.Button == MouseButtons.Right && this.graphic.GetLayer(this.actualLayer).Element[yElement, xElement] != rightPixel)
							{
								this.graphic.GetLayer(this.actualLayer).Element[yElement, xElement] = rightPixel;
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
			catch(IndexOutOfRangeException)
			{

			}
		}

		private void SelectLayer()
		{
			short index = (short)this.GetLayerIDBySelectedIndex();
			this.actualLayer = this.graphic.GetIndexByID(index);
			this.drawPanel.Invalidate();
		}

		private short GetLayerIDBySelectedIndex()
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

		private Pixel GetPixelFromComboBox(int index, Pixel lastPixel)
		{
			byte r = 0;
			byte g = 0;
			byte b = 0;
			if(lastPixel != null && !lastPixel.IsTransparent && !lastPixel.IsSpecialColorWithoutRGB)
			{
				r = lastPixel.Red;
				g = lastPixel.Green;
				b = lastPixel.Blue;
			}
			switch (index)
			{
				case 2:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.BehindGlass);
				case 3:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Always_Bright, r, g, b);
				case 4:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Lamp_Yellow, r, g, b);
				case 5:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Lamp_ColdWhite, r, g, b);
				case 6:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Lamp_Red, r, g, b);
				case 7:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Lamp_YellowWhite, r, g, b);
				case 8:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Lamp_Gas_Yellow, r, g, b);
				case 9:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Window_Yellow, r, g, b);
				case 10:
					return Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Window_Neon, r, g, b);
				case 11:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_BG);
				case 12:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Sleepers0);
				case 13:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Sleepers1);
				case 14:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Sleepers3);
				case 15:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Road0);
				case 16:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Road1);
				case 17:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Road2);
				case 18:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Road3);
				case 19:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Trackbed0);
				case 20:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Trackbed1);
				case 21:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Trackbed2);
				case 22:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Rails_Trackbed3);
				case 23:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Marking_Point_Bus0);
				case 24:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Marking_Point_Bus1);
				case 25:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Marking_Point_Bus2);
				case 26:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Marking_Point_Bus3);
				case 27:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Marking_Point_Water);
				case 28:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Gravel);
				case 29:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Small_Gravel);
				case 30:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Grassy);
				case 31:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Path_BG);
				case 32:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Path_FG);
				case 33:
					return Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Text);
				default:
					return null;
			}
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
			if (ExitEditor())
				return;
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
			if (ExitEditor())
				return;
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
				if (this.leftPixel.IsSpecialColorWithRGB == true)
				{
					this.leftPixel = Pixel.SpecialPixelWithRGB(this.leftPixel.SpecialWithRGB, c.R, c.G, c.B);
				}
				else
				{
					this.leftPixel = Pixel.FromColor(c);
					this.leftComboBox.SelectedIndex = 0;
				}
				this.leftColorButton.BackColor = c;
			}
		}

		private void rightColorButton_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.colorDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				Color c = this.colorDialog.Color;
				if (this.rightPixel.IsSpecialColorWithRGB == true)
				{
					this.rightPixel = Pixel.SpecialPixelWithRGB(this.rightPixel.SpecialWithRGB, c.R, c.G, c.B);
				}
				else
				{
					this.rightPixel = Pixel.FromColor(c);
					this.rightComboBox.SelectedIndex = 0;
				}
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
				if (this.lastRightPixel != null && !this.lastRightPixel.IsTransparent && !this.lastRightPixel.IsSpecialColorWithoutRGB)
				{
					if (this.lastRightPixel.IsSpecialColorWithRGB)
					{
						this.rightPixel = Pixel.RGBPixel(this.lastRightPixel.Red, this.lastRightPixel.Green, this.lastRightPixel.Blue);
					}
					else
					{
						this.rightPixel = this.lastRightPixel;
					}
				}
				else
				{
					this.rightPixel = Pixel.RGBPixel(0, 0, 0);
				}
			}
			else if (this.rightComboBox.SelectedIndex == 1)
			{
				if (!this.rightPixel.IsTransparent)
				{
					this.lastRightPixel = this.rightPixel;
					this.rightPixel = Pixel.TransparentPixel();
				}
			}
			else
			{
				Pixel lastPixel = null;
				if(this.rightPixel != null && !this.rightPixel.IsTransparent && !this.rightPixel.IsSpecialColorWithoutRGB)
				{
					lastPixel = this.rightPixel;
				}
				else if(this.lastRightPixel != null && !this.lastRightPixel.IsTransparent && !this.lastRightPixel.IsSpecialColorWithoutRGB)
				{
					lastPixel = this.lastRightPixel;
				}
				Pixel pixel = this.GetPixelFromComboBox(this.rightComboBox.SelectedIndex, lastPixel);
				if(pixel != null)
				{
					if(!this.rightPixel.IsTransparent && !this.rightPixel.IsSpecialColorWithoutRGB)
					{
						this.lastRightPixel = this.rightPixel;
					}
					this.rightPixel = pixel;
				}
				else
				{
					MessageBox.Show("WTF");
				}
			}
			this.rightColorButton.BackColor = this.rightPixel.ConvertToColor();
		}

		private void leftComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.leftComboBox.SelectedIndex == 0)
			{
				if (this.lastLeftPixel != null && !this.lastLeftPixel.IsTransparent && !this.lastLeftPixel.IsSpecialColorWithoutRGB)
				{
					if (this.lastLeftPixel.IsSpecialColorWithRGB)
					{
						this.leftPixel = Pixel.RGBPixel(this.lastLeftPixel.Red, this.lastLeftPixel.Green, this.lastLeftPixel.Blue);
					}
					else
					{
						this.leftPixel = this.lastLeftPixel;
					}
				}
				else
				{
					this.leftPixel = Pixel.RGBPixel(0, 0, 0);
				}
			}
			else if (this.leftComboBox.SelectedIndex == 1)
			{
				if (this.leftPixel.IsTransparent != true)
				{
					this.lastLeftPixel = this.leftPixel;
					this.leftPixel = Pixel.TransparentPixel();
				}
			}
			else
			{
				Pixel lastPixel = null;
				if (this.leftPixel != null && !this.leftPixel.IsTransparent && !this.leftPixel.IsSpecialColorWithoutRGB)
				{
					lastPixel = this.leftPixel;
				}
				else if (this.lastLeftPixel != null && !this.lastLeftPixel.IsTransparent && !this.lastLeftPixel.IsSpecialColorWithoutRGB)
				{
					lastPixel = this.lastLeftPixel;
				}
				Pixel pixel = this.GetPixelFromComboBox(this.leftComboBox.SelectedIndex, lastPixel);
				if (pixel != null)
				{
					if (!this.leftPixel.IsTransparent && !this.leftPixel.IsSpecialColorWithoutRGB)
					{
						this.lastLeftPixel = this.leftPixel;
					}
					this.leftPixel = pixel;
				}
				else
				{
					MessageBox.Show("WTF");
				}
			}
			this.leftColorButton.BackColor = this.leftPixel.ConvertToColor();
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
