using BahnEditor.BahnLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;
using System.IO;

namespace BahnEditor.Editor
{
	public partial class Editor : Form
	{
		private GraphicArchive zoom1Archive;
		private GraphicArchive zoom2Archive;
		private GraphicArchive zoom4Archive;
		private int actualGraphic;
		private LayerId actualLayer;// = LayerId.Foreground;
		private ZoomFactor actualZoomFactor = ZoomFactor.Zoom1;
		private string lastPath = "";
		private int zoomLevel = 5;
		private bool userMadeChanges = false;
		private bool hasLoadedGraphic = false;
		private int overviewLine = 0;
		private int overviewAlternative = 0;
		private bool layerSelectBoxCodeChanged = false;
		private bool zoom2CheckBoxCodeChanged = false;
		private bool zoom4CheckBoxCodeChanged = false;
		private Pixel leftPixel = new Pixel(0, 0, 0);
		private Pixel lastLeftPixel = null;
		private Pixel rightPixel = new Pixel(255, 255, 255);
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
			this.tabControl.TabPages.Remove(this.zoom2Tab);
			this.tabControl.TabPages.Remove(this.zoom4Tab);
		}

		private void NewGraphicArchive()
		{
			this.zoom1Archive = new GraphicArchive(ZoomFactor.Zoom1);
			this.zoom2Archive = new GraphicArchive(ZoomFactor.Zoom2);
			this.zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
			Graphic graphic = new Graphic("Kein Text");
			graphic.AddTransparentLayer(LayerId.Foreground);
			this.zoom1Archive.AddGraphic(graphic);
			this.actualGraphic = 0;
			this.drawPanel.Visible = true;
			this.overviewPanel.Visible = true;
			this.tabControl.Visible = true;
			this.settingsPanel.Visible = true;
			this.ChangeLayer(LayerId.Foreground);
			if (this.tabControl.TabPages.Contains(this.zoom2Tab))
			{
				this.tabControl.TabPages.Remove(this.zoom2Tab);
				this.zoom2CheckBoxCodeChanged = true;
				this.zoom2CheckBox.Checked = false;
			}
			if (this.tabControl.TabPages.Contains(this.zoom4Tab))
			{
				this.tabControl.TabPages.Remove(this.zoom4Tab);
				this.zoom4CheckBoxCodeChanged = true;
				this.zoom4CheckBox.Checked = false;
			}
			this.ResizeDrawPanel();
			this.drawPanel.AutoScrollPosition = new Point(this.drawPanel.HorizontalScroll.Maximum, this.drawPanel.VerticalScroll.Maximum);
			this.drawPanel.Invalidate();
			this.overviewPanel.Invalidate();
		}

		private void LoadGraphicArchive()
		{
			try
			{
				this.zoom1Archive = GraphicArchive.Load(this.loadFileDialog.FileName);
				if (this.zoom1Archive.ZoomFactor != ZoomFactor.Zoom1)
				{
					throw new Exception("Zoomfactor not 1");
				}
				try
				{
					this.zoom2Archive = GraphicArchive.Load(this.loadFileDialog.FileName.Remove(this.loadFileDialog.FileName.Length - 3) + "uz2");
					if (this.zoom2Archive.ZoomFactor != ZoomFactor.Zoom2)
					{
						throw new Exception("Zoom2-file found, internal zoomfactor not 2");
					}
				}
				catch (FileNotFoundException)
				{
					this.zoom2Archive = new GraphicArchive(ZoomFactor.Zoom2);
				}
				try
				{
					this.zoom4Archive = GraphicArchive.Load(this.loadFileDialog.FileName.Remove(this.loadFileDialog.FileName.Length - 3) + "uz4");
					if (this.zoom4Archive.ZoomFactor != ZoomFactor.Zoom4)
					{
						throw new Exception("Zoom4-file found, internal zoomfactor not 4");
					}
				}
				catch (FileNotFoundException)
				{
					this.zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
				}
				this.actualGraphic = 0;
				if (this.zoom2Archive[this.actualGraphic] != null && !this.tabControl.TabPages.Contains(this.zoom2Tab))
				{
					this.tabControl.TabPages.Add(this.zoom2Tab);
					this.zoom2CheckBoxCodeChanged = true;
					this.zoom2CheckBox.Checked = true;
				}
				if (this.zoom4Archive[this.actualGraphic] != null && !this.tabControl.TabPages.Contains(this.zoom4Tab))
				{
					this.tabControl.TabPages.Add(this.zoom4Tab);
					this.zoom4CheckBoxCodeChanged = true;
					this.zoom4CheckBox.Checked = true;
				}
				this.hasLoadedGraphic = true;
				this.drawPanel.Visible = true;
				this.overviewPanel.Visible = true;
				this.tabControl.Visible = true;
				this.settingsPanel.Visible = true;
				this.userMadeChanges = false;
				this.ChangeLayer(LayerId.Foreground);
				this.ResizeDrawPanel();
				this.drawPanel.AutoScrollPosition = new Point(this.drawPanel.HorizontalScroll.Maximum, this.drawPanel.VerticalScroll.Maximum / 2);
				this.drawPanel.Invalidate();
				this.overviewPanel.Invalidate();
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("Fehler beim Laden: {0}!", ex.ToString()), "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SaveGraphicArchive()
		{
			try
			{
				if (!this.zoom1Archive.Save(lastPath, true))
				{
					MessageBox.Show("Fehler beim Speichern des Zoom1-Archivs!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				try
				{
					if (!this.zoom2Archive.Save(lastPath.Remove(lastPath.Length - 3) + "uz2", true))
					{
						MessageBox.Show("Fehler beim Speichern des Zoom2-Archivs!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				catch (ArchiveIsEmptyException) { }
				try
				{
					if (!this.zoom4Archive.Save(lastPath.Remove(lastPath.Length - 3) + "uz4", true))
					{
						MessageBox.Show("Fehler beim Speichern des Zoom4-Archivs!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				catch (ArchiveIsEmptyException) { }
				this.userMadeChanges = false;
			}
			catch (ElementIsEmptyException)
			{
				MessageBox.Show("Element ist leer!", "Element ist leer!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void ClickOnSaveButton(bool forceSave)
		{
			if (lastPath == "" || forceSave == true)
			{
				this.saveFileDialog.ShowDialog();
			}
			else
			{
				this.SaveGraphicArchive();
			}
		}

		private void ResizeDrawPanel()
		{
			this.drawPanel.AutoScrollMinSize = new Size((int)((this.zoomLevel) * Constants.ELEMENTWIDTH * 3 + 30), (int)((this.zoomLevel) * Constants.ELEMENTHEIGHT * 8 + 30));
		}

		private void PaintGraphic(Graphics g)
		{
			try
			{
				if ((this.actualZoomFactor == ZoomFactor.Zoom1 && (this.zoom1Archive[this.actualGraphic] == null || this.zoom1Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)) ||
					(this.actualZoomFactor == ZoomFactor.Zoom2 && (this.zoom2Archive[this.actualGraphic] == null || this.zoom2Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)) ||
					(this.actualZoomFactor == ZoomFactor.Zoom4 && (this.zoom4Archive[this.actualGraphic] == null || this.zoom4Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)))
				{
					g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);
					//g.FillRectangle(transparentBrush, 20, 20, Constants.SYMBREITE * this.zoomLevel * 3, Constants.SYMHOEHE * this.zoomLevel * 8); //transparent 0, 112, 0
					g.DrawImage(global::BahnEditor.Editor.Properties.Resources.background, 20.0f, 20.0f, Constants.ELEMENTWIDTH * this.zoomLevel * 3, Constants.ELEMENTHEIGHT * this.zoomLevel * 8);
				}
				else
				{
					Pixel[,] element = this.GetElement();
					if (element != null)
					{

						if (g.ClipBounds.Width == (this.zoomLevel * 2) / (int)this.actualZoomFactor && g.ClipBounds.Height == (this.zoomLevel * 2) / (int)this.actualZoomFactor)
						{

							int x = (int)g.ClipBounds.X;
							int y = (int)g.ClipBounds.Y;
							int diffX = 0;
							int diffY = 0;
							if (x - drawPanel.AutoScrollPosition.X < 20)
							{
								x = 20;
							}
							else if (x - 20 - drawPanel.AutoScrollPosition.X + this.zoomLevel * 2 >= Constants.ELEMENTWIDTH * 3 * this.zoomLevel)
							{
								diffX = (int)((x - 20 - drawPanel.AutoScrollPosition.X + this.zoomLevel * 2) - (Constants.ELEMENTWIDTH * 3 * this.zoomLevel));
							}
							if (y - drawPanel.AutoScrollPosition.Y < 20)
							{
								y = 20;
							}
							else if (y - 20 - drawPanel.AutoScrollPosition.Y + this.zoomLevel * 2 >= Constants.ELEMENTHEIGHT * 8 * this.zoomLevel)
							{
								diffY = (int)((y - 20 - drawPanel.AutoScrollPosition.Y + this.zoomLevel * 2) - (Constants.ELEMENTHEIGHT * 8 * this.zoomLevel));
							}
							int xElement = (int)(((x - 20 - drawPanel.AutoScrollPosition.X) / (float)this.zoomLevel) * (int)this.actualZoomFactor);
							int yElement = (int)(((20 + this.zoomLevel * element.GetLength(0) / (int)this.actualZoomFactor + drawPanel.AutoScrollPosition.Y - y) / (float)this.zoomLevel) * (int)this.actualZoomFactor);

							//g.FillRectangle(transparentBrush, x, y, this.zoomLevel * 2 - diffX, this.zoomLevel * 2 - diffY);
							g.DrawImage(global::BahnEditor.Editor.Properties.Resources.background, 20.0f, 20.0f, Constants.ELEMENTWIDTH * this.zoomLevel * 3, Constants.ELEMENTHEIGHT * this.zoomLevel * 8);
							g.TranslateTransform(drawPanel.AutoScrollPosition.X, drawPanel.AutoScrollPosition.Y);

							if (yElement >= element.GetLength(0))
							{
								yElement = element.GetLength(0) - 1;
							}
							if (xElement >= element.GetLength(1))
							{
								xElement = element.GetLength(1) - 1;
							}

							for (int i = yElement; (i > yElement - 3) && i < element.GetLength(0) && i >= 0; i--)
							{
								for (int j = xElement; (j < xElement + 3) && j < element.GetLength(1); j++)
								{
									if (element[i, j].IsTransparent != true)
									{
										Brush brush;
										if (element[i, j].IsSpecialColorWithoutRGB)
											brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), element[i, j].ToColor());
										else if (element[i, j].IsSpecialColorWithRGB)
											brush = new HatchBrush(HatchStyle.Percent10, Color.FromArgb(140, 140, 140), element[i, j].ToColor());
										else
											brush = new SolidBrush(element[i, j].ToColor());
										g.FillRectangle(brush, j * (this.zoomLevel) / (int)this.actualZoomFactor + 20, (int)((((this.zoomLevel) * element.GetLength(0) / (float)this.actualZoomFactor) - ((this.zoomLevel) / (float)this.actualZoomFactor * (i + 1))) + 20), this.zoomLevel / (float)this.actualZoomFactor, this.zoomLevel / (float)this.actualZoomFactor);
									}
								}
							}
						}
						else
						{
							g.TranslateTransform(drawPanel.AutoScrollPosition.X + 20, drawPanel.AutoScrollPosition.Y + 20);
							PaintElement(g, element, this.zoomLevel, true, this.actualZoomFactor);
							g.TranslateTransform(-20, -20);
						}
					}
				}
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						g.DrawRectangle(Pens.Gray, 20 + (i * Constants.ELEMENTWIDTH) * (this.zoomLevel), 20 + (j * Constants.ELEMENTHEIGHT) * (this.zoomLevel), Constants.ELEMENTWIDTH * (this.zoomLevel), Constants.ELEMENTHEIGHT * (this.zoomLevel));
					}
				}
				g.DrawRectangle(Pens.DarkGray, 20 + Constants.ELEMENTWIDTH * (this.zoomLevel), 20 + (6 * Constants.ELEMENTHEIGHT) * (this.zoomLevel), Constants.ELEMENTWIDTH * (this.zoomLevel), Constants.ELEMENTHEIGHT * (this.zoomLevel));
			}
			catch (IndexOutOfRangeException)
			{
				MessageBox.Show("Internal Error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void PaintOverview(Graphics g)
		{
			if (this.zoom1Archive != null)
			{
				g.TranslateTransform(50, 20);
				for (int i = this.overviewLine * 18 + this.overviewAlternative, j = 1; j <= 9; i += 2, j++)
				{
					if (this.zoom1Archive[i] != null)
					{
						PaintElement(g, this.zoom1Archive[i].ElementPreview(), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ELEMENTWIDTH, Constants.ELEMENTHEIGHT);
					}
					g.DrawString(String.Format("{0} - {1}", j, i), DefaultFont, Brushes.Black, 1, 20);
					g.TranslateTransform(Constants.ELEMENTWIDTH + 10, 0);
				}
			}
		}

		private static void PaintElement(Graphics g, Pixel[,] element, int zoomLevel, bool withHatchBrush, ZoomFactor zoomfactor)
		{
			if (withHatchBrush)
				//g.FillRectangle(transparentBrush, 0, 0, element.GetLength(1) * zoomLevel / (int)zoomfactor, element.GetLength(0) * zoomLevel / (int)zoomfactor);
				g.DrawImage(global::BahnEditor.Editor.Properties.Resources.background, 0, 0, element.GetLength(1) * zoomLevel / (int)zoomfactor, element.GetLength(0) * zoomLevel / (int)zoomfactor);
			else
				g.FillRectangle(new SolidBrush(Color.FromArgb(0, 112, 0)), 0, 0, element.GetLength(1) * zoomLevel / (int)zoomfactor, element.GetLength(0) * zoomLevel / (int)zoomfactor);
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					if (element[i, j].IsTransparent != true)
					{
						Brush brush;
						if (!withHatchBrush)
							brush = new SolidBrush(element[i, j].ToColor());
						else if (element[i, j].IsSpecialColorWithoutRGB)
							brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), element[i, j].ToColor());
						else if (element[i, j].IsSpecialColorWithRGB)
							brush = new HatchBrush(HatchStyle.Percent10, Color.FromArgb(140, 140, 140), element[i, j].ToColor());
						else
							brush = new SolidBrush(element[i, j].ToColor());
						g.FillRectangle(brush, (j * zoomLevel) / (int)zoomfactor, ((zoomLevel * element.GetLength(0)) - (zoomLevel * (i + 1))) / (int)zoomfactor, zoomLevel / (float)zoomfactor, zoomLevel / (float)zoomfactor);
					}
				}
			}
		}

		private void MouseClickGraphic(MouseEventArgs e)
		{
			if (this.hasLoadedGraphic == true)
			{
				this.hasLoadedGraphic = false;
				return;
			}
			if (this.actualZoomFactor == ZoomFactor.Zoom1 && e.Button != MouseButtons.None)
			{
				if (this.zoom1Archive[this.actualGraphic] == null)
				{
					Graphic graphic = new Graphic("Kein Text");
					graphic.AddTransparentLayer(LayerId.Foreground);
					this.zoom1Archive.AddGraphic(this.actualGraphic, graphic);
					this.ChangeLayer(LayerId.Foreground);
					this.overviewPanel.Invalidate();
				}
				else if (this.zoom1Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)
				{
					LayerId layerID = GetLayerIDBySelectedIndex();
					this.zoom1Archive[this.actualGraphic].AddTransparentLayer(layerID);
					this.ChangeLayer(layerID);
				}
			}
			else if (this.actualZoomFactor == ZoomFactor.Zoom2 && this.zoom2Archive[this.actualGraphic] != null && this.zoom1Archive[this.actualGraphic] != null && this.zoom2Archive[this.actualGraphic].GetLayer(this.actualLayer) == null && e.Button != MouseButtons.None)
			{
				LayerId layerID = GetLayerIDBySelectedIndex();
				this.zoom2Archive[this.actualGraphic].AddTransparentLayer(layerID);
				this.ChangeLayer(layerID);
			}
			else if (this.actualZoomFactor == ZoomFactor.Zoom4 && this.zoom4Archive[this.actualGraphic] != null && this.zoom1Archive[this.actualGraphic] != null && this.zoom4Archive[this.actualGraphic].GetLayer(this.actualLayer) == null && e.Button != MouseButtons.None)
			{
				LayerId layerID = GetLayerIDBySelectedIndex();
				this.zoom4Archive[this.actualGraphic].AddTransparentLayer(layerID);
				this.ChangeLayer(layerID);
			}
			try
			{
				Pixel[,] element = this.GetElement();

				if (e.X - drawPanel.AutoScrollPosition.X >= 20 && e.Y - drawPanel.AutoScrollPosition.Y >= 20 && (20 + this.zoomLevel * element.GetLength(0) + drawPanel.AutoScrollPosition.Y - e.Y) > 0)
				{
					int xElement = (int)(((e.X - 20 - drawPanel.AutoScrollPosition.X) / (float)this.zoomLevel) * (int)this.actualZoomFactor);
					int yElement = (int)(((20 + this.zoomLevel * element.GetLength(0) / (int)this.actualZoomFactor + drawPanel.AutoScrollPosition.Y - e.Y) / (float)this.zoomLevel) * (int)this.actualZoomFactor);
					if (xElement >= 0 && yElement >= 0 && xElement < element.GetLength(1) && yElement < element.GetLength(0))
					{
						if (e.Button == MouseButtons.Left && element[yElement, xElement] != leftPixel)
						{
							element[yElement, xElement] = leftPixel;
						}
						else if (e.Button == MouseButtons.Right && element[yElement, xElement] != rightPixel)
						{
							element[yElement, xElement] = rightPixel;
						}
						else
						{
							return;
						}
						this.userMadeChanges = true;
						drawPanel.Invalidate(new Rectangle(e.X - this.zoomLevel / ((int)this.actualZoomFactor), e.Y - this.zoomLevel / ((int)this.actualZoomFactor), (this.zoomLevel * 2) / (int)this.actualZoomFactor, (this.zoomLevel * 2) / (int)this.actualZoomFactor));
					}
				}
			}
			catch (Exception)
			{ }
		}

		private void ClickOverview(MouseEventArgs e)
		{
			int element = -1;
			int x = e.X - 50;
			if (x >= 0 && 31 >= x)
				element = 0;
			else if (x >= 42 && 73 >= x)
				element = 1;
			else if (x >= 84 && 115 >= x)
				element = 2;
			else if (x >= 126 && 157 >= x)
				element = 3;
			else if (x >= 168 && 199 >= x)
				element = 4;
			else if (x >= 210 && 241 >= x)
				element = 5;
			else if (x >= 252 && 283 >= x)
				element = 6;
			else if (x >= 294 && 325 >= x)
				element = 7;
			else if (x >= 336 && 367 >= x)
				element = 8;
			if (element != -1)
			{
				this.actualGraphic = (element * 2 + overviewAlternative) + ((overviewLine) * 18);
				if (this.zoom2Archive[this.actualGraphic] != null && !this.tabControl.TabPages.Contains(this.zoom2Tab))
				{
					this.tabControl.TabPages.Insert(1, this.zoom2Tab);
					this.zoom2CheckBoxCodeChanged = true;
					this.zoom2CheckBox.Checked = true;
				}
				else if (this.zoom2Archive[this.actualGraphic] == null && this.tabControl.TabPages.Contains(this.zoom2Tab))
				{
					this.tabControl.TabPages.Remove(this.zoom2Tab);
					this.zoom2CheckBoxCodeChanged = true;
					this.zoom2CheckBox.Checked = false;
				}
				if (this.zoom4Archive[this.actualGraphic] != null && !this.tabControl.TabPages.Contains(this.zoom4Tab))
				{
					this.tabControl.TabPages.Add(this.zoom4Tab);
					this.zoom4CheckBoxCodeChanged = true;
					this.zoom4CheckBox.Checked = true;
				}
				else if (this.zoom4Archive[this.actualGraphic] == null && this.tabControl.TabPages.Contains(this.zoom4Tab))
				{
					this.tabControl.TabPages.Remove(this.zoom4Tab);
					this.zoom4CheckBoxCodeChanged = true;
					this.zoom4CheckBox.Checked = false;
				}
				this.ChangeLayer(LayerId.Foreground);
				this.drawPanel.Invalidate();
			}
		}

		private void ChangeLayer(LayerId id)
		{
			if (this.actualLayer != id)
			{
				this.actualLayer = id;
				int index;
				switch (id)
				{
					case LayerId.ToBackground:
						index = 2;
						break;
					case LayerId.Foreground:
						index = 0;
						break;
					case LayerId.Background:
						index = 1;
						break;
					case LayerId.Front:
						index = 4;
						break;
					case LayerId.ForegroundAbove:
						index = 3;
						break;
					default:
						throw new Exception("Internal Error");
				}
				if (this.layerComboBox.SelectedIndex != index)
				{
					this.layerSelectBoxCodeChanged = true;
					this.layerComboBox.SelectedIndex = index;
				}
			}
		}

		private void SelectLayer()
		{
			if (this.layerSelectBoxCodeChanged)
			{
				this.layerSelectBoxCodeChanged = false;
			}
			else
			{
				this.actualLayer = this.GetLayerIDBySelectedIndex();
				this.drawPanel.Invalidate();
			}
		}

		private LayerId GetLayerIDBySelectedIndex()
		{
			switch (this.layerComboBox.SelectedIndex)
			{
				case 0:
					return LayerId.Foreground;
				case 1:
					return LayerId.Background;
				case 2:
					return LayerId.ToBackground;
				case 3:
					return LayerId.ForegroundAbove;
				case 4:
					return LayerId.Front;
				default:
					throw new Exception("Internal Error");
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
			if (lastPixel != null && !lastPixel.IsTransparent && !lastPixel.IsSpecialColorWithoutRGB)
			{
				r = lastPixel.Red;
				g = lastPixel.Green;
				b = lastPixel.Blue;
			}
			switch (index)
			{
				case 2:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.BehindGlass);
				case 3:
					return new Pixel(Pixel.SpecialPixelWithRGB.Always_Bright, r, g, b);
				case 4:
					return new Pixel(Pixel.SpecialPixelWithRGB.Lamp_Yellow, r, g, b);
				case 5:
					return new Pixel(Pixel.SpecialPixelWithRGB.Lamp_ColdWhite, r, g, b);
				case 6:
					return new Pixel(Pixel.SpecialPixelWithRGB.Lamp_Red, r, g, b);
				case 7:
					return new Pixel(Pixel.SpecialPixelWithRGB.Lamp_YellowWhite, r, g, b);
				case 8:
					return new Pixel(Pixel.SpecialPixelWithRGB.Lamp_Gas_Yellow, r, g, b);
				case 9:
					return new Pixel(Pixel.SpecialPixelWithRGB.Window_Yellow_0, r, g, b);
				case 10:
					return new Pixel(Pixel.SpecialPixelWithRGB.Window_Yellow_1, r, g, b);
				case 11:
					return new Pixel(Pixel.SpecialPixelWithRGB.Window_Yellow_2, r, g, b);
				case 12:
					return new Pixel(Pixel.SpecialPixelWithRGB.Window_Neon_0, r, g, b);
				case 13:
					return new Pixel(Pixel.SpecialPixelWithRGB.Window_Neon_1, r, g, b);
				case 14:
					return new Pixel(Pixel.SpecialPixelWithRGB.Window_Neon_2, r, g, b);
				case 15:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_BG);
				case 16:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Sleepers0);
				case 17:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Sleepers1);
				case 18:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Sleepers3);
				case 19:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Road0);
				case 20:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Road1);
				case 21:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Road2);
				case 22:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Road3);
				case 23:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Trackbed0);
				case 24:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Trackbed1);
				case 25:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Trackbed2);
				case 26:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Trackbed3);
				case 27:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Marking_Point_Bus0);
				case 28:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Marking_Point_Bus1);
				case 29:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Marking_Point_Bus2);
				case 30:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Marking_Point_Bus3);
				case 31:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Marking_Point_Water);
				case 32:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Gravel);
				case 33:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Small_Gravel);
				case 34:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Grassy);
				case 35:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Path_BG);
				case 36:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Path_FG);
				case 37:
					return new Pixel(Pixel.SpecialPixelWithoutRGB.As_Text);
				default:
					return null;
			}
		}

		private Pixel[,] GetElement()
		{
			switch (this.actualZoomFactor)
			{
				case ZoomFactor.Zoom1:
					if (this.zoom1Archive != null && this.zoom1Archive[this.actualGraphic] != null && this.zoom1Archive[this.actualGraphic].GetLayer(this.actualLayer) != null && this.zoom1Archive[this.actualGraphic].GetLayer(this.actualLayer).Element != null)
					{
						return this.zoom1Archive[this.actualGraphic].GetLayer(this.actualLayer).Element;
					}
					break;
				case ZoomFactor.Zoom2:
					if (this.zoom2Archive != null && this.zoom2Archive[this.actualGraphic] != null && this.zoom2Archive[this.actualGraphic].GetLayer(this.actualLayer) != null && this.zoom2Archive[this.actualGraphic].GetLayer(this.actualLayer).Element != null)
					{
						return this.zoom2Archive[this.actualGraphic].GetLayer(this.actualLayer).Element;
					}
					break;
				case ZoomFactor.Zoom4:
					if (this.zoom4Archive != null && this.zoom4Archive[this.actualGraphic] != null && this.zoom4Archive[this.actualGraphic].GetLayer(this.actualLayer) != null && this.zoom4Archive[this.actualGraphic].GetLayer(this.actualLayer).Element != null)
					{
						return this.zoom4Archive[this.actualGraphic].GetLayer(this.actualLayer).Element;
					}
					break;
				default:
					break;
			}
			return null;
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
			this.NewGraphicArchive();
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			if (ExitEditor())
				return;
			this.loadFileDialog.Filter = "uz1-files|*.uz1";
			this.loadFileDialog.ShowDialog();
		}

		private void loadFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.LoadGraphicArchive();
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(false);
		}

		private void newMenuItem_Click(object sender, EventArgs e)
		{
			this.NewGraphicArchive();
		}

		private void openMenuItem_Click(object sender, EventArgs e)
		{
			if (ExitEditor())
				return;
			this.loadFileDialog.Filter = "uz1-files|*.uz1";
			this.loadFileDialog.ShowDialog();
		}

		private void saveMenuItem_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(false);
		}

		private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.lastPath = this.saveFileDialog.FileName;
			this.SaveGraphicArchive();
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
					this.leftPixel = new Pixel(this.leftPixel.SpecialColorWithRGB, c.R, c.G, c.B);
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
					this.rightPixel = new Pixel(this.rightPixel.SpecialColorWithRGB, c.R, c.G, c.B);
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
						this.rightPixel = new Pixel(this.lastRightPixel.Red, this.lastRightPixel.Green, this.lastRightPixel.Blue);
					}
					else
					{
						this.rightPixel = this.lastRightPixel;
					}
				}
				else
				{
					this.rightPixel = new Pixel(0, 0, 0);
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
				if (this.rightPixel != null && !this.rightPixel.IsTransparent && !this.rightPixel.IsSpecialColorWithoutRGB)
				{
					lastPixel = this.rightPixel;
				}
				else if (this.lastRightPixel != null && !this.lastRightPixel.IsTransparent && !this.lastRightPixel.IsSpecialColorWithoutRGB)
				{
					lastPixel = this.lastRightPixel;
				}
				Pixel pixel = this.GetPixelFromComboBox(this.rightComboBox.SelectedIndex, lastPixel);
				if (pixel != null)
				{
					if (!this.rightPixel.IsTransparent && !this.rightPixel.IsSpecialColorWithoutRGB)
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
			this.rightColorButton.BackColor = this.rightPixel.ToColor();
		}

		private void leftComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.leftComboBox.SelectedIndex == 0)
			{
				if (this.lastLeftPixel != null && !this.lastLeftPixel.IsTransparent && !this.lastLeftPixel.IsSpecialColorWithoutRGB)
				{
					if (this.lastLeftPixel.IsSpecialColorWithRGB)
					{
						this.leftPixel = new Pixel(this.lastLeftPixel.Red, this.lastLeftPixel.Green, this.lastLeftPixel.Blue);
					}
					else
					{
						this.leftPixel = this.lastLeftPixel;
					}
				}
				else
				{
					this.leftPixel = new Pixel(0, 0, 0);
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
			this.leftColorButton.BackColor = this.leftPixel.ToColor();
		}

		private void Editor_Load(object sender, EventArgs e)
		{
			this.EditorLoaded();
		}

		private void layerComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void overviewPanel_Paint(object sender, PaintEventArgs e)
		{
			this.PaintOverview(e.Graphics);
		}

		private void overviewLeftRightButton_Click(object sender, EventArgs e)
		{
			if (this.overviewAlternative == 1)
				this.overviewAlternative = 0;
			else
				this.overviewAlternative = 1;
			this.overviewPanel.Invalidate();
			drawPanel.Focus();
		}

		private void overviewDownButton_Click(object sender, EventArgs e)
		{
			if (this.overviewLine > 0)
			{
				this.overviewLine--;
				this.overviewPanel.Invalidate();
			}
			drawPanel.Focus();
		}

		private void overviewUpButton_Click(object sender, EventArgs e)
		{
			if (this.overviewLine < 4)
			{
				this.overviewLine++;
				this.overviewPanel.Invalidate();
			}
			drawPanel.Focus();
		}

		private void overviewPanel_Click(object sender, EventArgs e)
		{
			MouseEventArgs me = e as MouseEventArgs;
			if (me != null)
				ClickOverview(me);
			else
				MessageBox.Show("Interner Fehler", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void drawPanel_MouseUp(object sender, MouseEventArgs e)
		{
			this.overviewPanel.Invalidate();
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			switch (e.TabPage.Name)
			{
				case "zoom1Tab":
					this.actualZoomFactor = ZoomFactor.Zoom1;
					break;
				case "zoom2Tab":
					this.actualZoomFactor = ZoomFactor.Zoom2;
					break;
				case "zoom4Tab":
					this.actualZoomFactor = ZoomFactor.Zoom4;
					break;
				default:
					throw new Exception("Internal Error!");
			}
			this.drawPanel.Invalidate();
		}

		private void zoomInButton_Click(object sender, EventArgs e)
		{
			this.zoomLevel++;
			if (!this.zoomOutButton.Enabled)
			{
				this.zoomOutButton.Enabled = true;
			}
			if (this.zoomLevel > 10 + (int)this.actualZoomFactor)
			{
				this.zoomInButton.Enabled = false;
			}
			this.ResizeDrawPanel();
			this.drawPanel.Invalidate();
		}

		private void zoomOutButton_Click(object sender, EventArgs e)
		{
			this.zoomLevel--;
			if (!this.zoomInButton.Enabled)
			{
				this.zoomInButton.Enabled = true;
			}

			if (this.zoomLevel < 4 + (int)this.actualZoomFactor)
			{
				this.zoomOutButton.Enabled = false;
			}
			this.ResizeDrawPanel();
			this.drawPanel.Invalidate();
		}

		private void zoom2CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.zoom2CheckBoxCodeChanged)
			{
				if (zoom2CheckBox.Checked)
				{
					DialogResult result = MessageBox.Show("Soll wirklich eine Zoom2-Grafik erstellt werden?", "Zoom2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (result == DialogResult.Yes)
					{
						Graphic graphic = new Graphic("Kein Text", ZoomFactor.Zoom2);
						graphic.AddTransparentLayer(LayerId.Foreground);
						this.zoom2Archive.AddGraphic(this.actualGraphic, graphic);
						this.ChangeLayer(LayerId.Foreground);
						this.tabControl.TabPages.Insert(1, this.zoom2Tab);
						this.tabControl.SelectedTab = this.zoom2Tab;
						this.actualZoomFactor = ZoomFactor.Zoom2;
						this.drawPanel.Invalidate();
					}
					else
					{
						this.zoom2CheckBoxCodeChanged = true;
						this.zoom2CheckBox.Checked = false;
					}
				}
				else
				{
					DialogResult result = MessageBox.Show("Soll wirklich die Zoom2-Grafik gelöscht werden?", "Zoom2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (result == DialogResult.Yes)
					{
						this.zoom2Archive.RemoveGraphic(this.actualGraphic);
						this.tabControl.TabPages.Remove(this.zoom2Tab);
						this.tabControl.SelectedTab = this.zoom1Tab;
						this.actualZoomFactor = ZoomFactor.Zoom1;
						this.drawPanel.Invalidate();
					}
					else
					{
						this.zoom2CheckBoxCodeChanged = true;
						this.zoom2CheckBox.Checked = true;
					}
				}
			}
			else
			{
				this.zoom2CheckBoxCodeChanged = false;
			}
		}

		private void zoom4CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.zoom4CheckBoxCodeChanged)
			{
				if (zoom4CheckBox.Checked)
				{
					DialogResult result = MessageBox.Show("Soll wirklich eine Zoom4-Grafik erstellt werden?", "Zoom4", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (result == DialogResult.Yes)
					{
						Graphic graphic = new Graphic("Kein Text", ZoomFactor.Zoom4);
						graphic.AddTransparentLayer(LayerId.Foreground);
						this.zoom4Archive.AddGraphic(this.actualGraphic, graphic);
						this.ChangeLayer(LayerId.Foreground);
						this.tabControl.TabPages.Add(this.zoom4Tab);
						this.tabControl.SelectedTab = this.zoom4Tab;
						this.actualZoomFactor = ZoomFactor.Zoom4;
						this.drawPanel.Invalidate();
					}
					else
					{
						this.zoom4CheckBoxCodeChanged = true;
						this.zoom4CheckBox.Checked = false;
					}
				}
				else
				{
					DialogResult result = MessageBox.Show("Soll wirklich die Zoom4-Grafik gelöscht werden?", "Zoom4", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (result == DialogResult.Yes)
					{
						this.zoom4Archive.RemoveGraphic(this.actualGraphic);
						this.tabControl.TabPages.Remove(this.zoom4Tab);
						this.tabControl.SelectedTab = this.zoom1Tab;
						this.actualZoomFactor = ZoomFactor.Zoom1;
						this.drawPanel.Invalidate();
					}
					else
					{
						this.zoom4CheckBoxCodeChanged = true;
						this.zoom4CheckBox.Checked = true;
					}
				}
			}
			else
			{
				this.zoom4CheckBoxCodeChanged = false;
			}
		}

		#endregion

	}
}
