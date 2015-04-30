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
		private bool hasLoadedGraphic = false;
		private bool layerSelectBoxCodeChanged = false;
		private bool userMadeChanges = false;
		private bool zoom2CheckBoxCodeChanged = false;
		private bool zoom4CheckBoxCodeChanged = false;
		private GraphicArchive zoom1Archive;
		private GraphicArchive zoom2Archive;
		private GraphicArchive zoom4Archive;
		private int actualGraphic;
		private int overviewAlternative = 0;
		private int overviewLine = 0;
		private int zoomLevel = 5;
		private LayerID actualLayer;// = LayerID.Foreground;
		private string lastPath = "";
		private uint lastLeftPixel = 0;
		private uint lastRightPixel = 0;
		private uint leftPixel = 0;
		private uint rightPixel = 255 << 16 | 255 << 8 | 255;
		private ZoomFactor actualZoomFactor = ZoomFactor.Zoom1;

		private static Brush transparentBrush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), Color.FromArgb(0, 112, 0)); //transparent 0, 112, 0


		public Editor()
		{
			InitializeComponent();
		}

		private void EditorLoaded()
		{
			this.rightComboBox.SelectedIndex = 0;
			this.leftComboBox.SelectedIndex = 0;
			this.particleComboBox.SelectedIndex = 0;
			this.tabControl.TabPages.Remove(this.zoom2Tab);
			this.tabControl.TabPages.Remove(this.zoom4Tab);
			this.NewGraphicArchive();
		}

		private void NewGraphicArchive()
		{
			this.zoom1Archive = new GraphicArchive(ZoomFactor.Zoom1);
			this.zoom2Archive = new GraphicArchive(ZoomFactor.Zoom2);
			this.zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
			Graphic graphic = new Graphic("Kein Text");
			graphic.AddTransparentLayer(LayerID.Foreground);
			this.zoom1Archive.AddGraphic(graphic);
			this.actualGraphic = 0;
			//this.drawPanel.Visible = true;
			//this.overviewPanel.Visible = true;
			//this.tabControl.Visible = true;
			//this.settingsPanel.Visible = true;
			this.ChangeLayer(LayerID.Foreground);
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
					throw new InvalidDataException("Zoomfactor not 1");
				}
				try
				{
					this.zoom2Archive = GraphicArchive.Load(this.loadFileDialog.FileName.Remove(this.loadFileDialog.FileName.Length - 3) + "uz2");
					if (this.zoom2Archive.ZoomFactor != ZoomFactor.Zoom2)
					{
						throw new InvalidDataException("Zoom2-file found, internal zoomfactor not 2");
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
						throw new InvalidDataException("Zoom4-file found, internal zoomfactor not 4");
					}
				}
				catch (FileNotFoundException)
				{
					this.zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
				}
				this.actualGraphic = 0;
				this.actualZoomFactor = ZoomFactor.Zoom1;
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
				if (this.zoom1Archive[this.actualGraphic].Properties.HasParticles)
				{
					if (this.zoom1Archive[this.actualGraphic].Properties.RawData.HasFlag(GraphicProperties.Properties.Steam))
						this.particleComboBox.SelectedIndex = 1;
					else
						this.particleComboBox.SelectedIndex = 2;
					this.particleWidthNumericUpDown.Enabled = true;
					this.particleXNumericUpDown.Enabled = true;
					this.particleYNumericUpDown.Enabled = true;
					this.particleWidthNumericUpDown.Value = this.zoom1Archive[this.actualGraphic].Properties.ParticleWidth;
					this.particleXNumericUpDown.Value = this.zoom1Archive[this.actualGraphic].Properties.ParticleX;
					this.particleYNumericUpDown.Value = this.zoom1Archive[this.actualGraphic].Properties.ParticleY;
				}
				else if(this.particleComboBox.SelectedIndex != 0)
				{
					this.particleComboBox.SelectedIndex = 0;
					this.particleWidthNumericUpDown.Enabled = false;
					this.particleXNumericUpDown.Enabled = false;
					this.particleYNumericUpDown.Enabled = false;
				}
				this.lastPath = this.zoom1Archive.FileName;
				this.hasLoadedGraphic = true;
				this.drawPanel.Visible = true;
				this.overviewPanel.Visible = true;
				this.tabControl.Visible = true;
				this.propertiesPanel.Visible = true;
				this.userMadeChanges = false;
				this.ChangeLayer(LayerID.Foreground);
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
			this.drawPanel.AutoScrollMinSize = new Size((int)((this.zoomLevel) * Constants.ElementWidth * 3 + 30), (int)((this.zoomLevel) * Constants.ElementHeight * 8 + 30));
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
					g.DrawImage(global::BahnEditor.Editor.Properties.Resources.background, 20.0f, 20.0f, Constants.ElementWidth * this.zoomLevel * 3, Constants.ElementHeight * this.zoomLevel * 8);
				}
				else
				{
					uint[,] element = this.GetElement();
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
							else if (x - 20 - drawPanel.AutoScrollPosition.X + this.zoomLevel * 2 >= Constants.ElementWidth * 3 * this.zoomLevel)
							{
								diffX = (int)((x - 20 - drawPanel.AutoScrollPosition.X + this.zoomLevel * 2) - (Constants.ElementWidth * 3 * this.zoomLevel));
							}
							if (y - drawPanel.AutoScrollPosition.Y < 20)
							{
								y = 20;
							}
							else if (y - 20 - drawPanel.AutoScrollPosition.Y + this.zoomLevel * 2 >= Constants.ElementHeight * 8 * this.zoomLevel)
							{
								diffY = (int)((y - 20 - drawPanel.AutoScrollPosition.Y + this.zoomLevel * 2) - (Constants.ElementHeight * 8 * this.zoomLevel));
							}
							int xElement = (int)(((x - 20 - drawPanel.AutoScrollPosition.X) / (float)this.zoomLevel) * (int)this.actualZoomFactor);
							int yElement = (int)(((20 + this.zoomLevel * element.GetLength(0) / (int)this.actualZoomFactor + drawPanel.AutoScrollPosition.Y - y) / (float)this.zoomLevel) * (int)this.actualZoomFactor);

							//g.FillRectangle(transparentBrush, x, y, this.zoomLevel * 2 - diffX, this.zoomLevel * 2 - diffY);
							g.DrawImage(global::BahnEditor.Editor.Properties.Resources.background, 20.0f, 20.0f, Constants.ElementWidth * this.zoomLevel * 3, Constants.ElementHeight * this.zoomLevel * 8);
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
									if (Pixel.IsTransparent(element[i, j]) != true)
									{
										Brush brush;
										if (Pixel.IsSpecial(element[i, j]))
											brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), PixelToColor(element[i, j]));
										else
											brush = new SolidBrush(PixelToColor(element[i, j]));
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
						g.DrawRectangle(Pens.Gray, 20 + (i * Constants.ElementWidth) * (this.zoomLevel), 20 + (j * Constants.ElementHeight) * (this.zoomLevel), Constants.ElementWidth * (this.zoomLevel), Constants.ElementHeight * (this.zoomLevel));
					}
				}
				g.DrawRectangle(Pens.DarkGray, 20 + Constants.ElementWidth * (this.zoomLevel), 20 + (6 * Constants.ElementHeight) * (this.zoomLevel), Constants.ElementWidth * (this.zoomLevel), Constants.ElementHeight * (this.zoomLevel));
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
						PaintElement(g, GraphicPreview(this.zoom1Archive[i]), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ElementWidth, Constants.ElementHeight);
					}
					g.DrawString(String.Format("{0} - {1}", j, i), DefaultFont, Brushes.Black, 1, 20);
					g.TranslateTransform(Constants.ElementWidth + 10, 0);
				}
			}
		}

		private static void PaintElement(Graphics g, uint[,] element, int zoomLevel, bool withHatchBrush, ZoomFactor zoomfactor)
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
					if (Pixel.IsTransparent(element[i, j]) != true)
					{
						Brush brush;
						if (!withHatchBrush)
							brush = new SolidBrush(PixelToColor(element[i, j]));
						else if (Pixel.IsSpecial(element[i, j]))
							brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), PixelToColor(element[i, j]));
						else
							brush = new SolidBrush(PixelToColor(element[i, j]));
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
			if (e.Button == MouseButtons.None)
				return;
			if (this.actualZoomFactor == ZoomFactor.Zoom1)
			{
				if (this.zoom1Archive[this.actualGraphic] == null)
				{
					Graphic graphic = new Graphic("Kein Text");
					graphic.AddTransparentLayer(LayerID.Foreground);
					this.zoom1Archive.AddGraphic(this.actualGraphic, graphic);
					this.ChangeLayer(LayerID.Foreground);
					this.overviewPanel.Invalidate();
				}
				else if (this.zoom1Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)
				{
					LayerID LayerID = GetLayerIDBySelectedIndex();
					this.zoom1Archive[this.actualGraphic].AddTransparentLayer(LayerID);
					this.ChangeLayer(LayerID);
				}
			}
			else if (this.actualZoomFactor == ZoomFactor.Zoom2 && this.zoom2Archive[this.actualGraphic] != null && this.zoom1Archive[this.actualGraphic] != null && this.zoom2Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)
			{
				LayerID LayerID = GetLayerIDBySelectedIndex();
				this.zoom2Archive[this.actualGraphic].AddTransparentLayer(LayerID);
				this.ChangeLayer(LayerID);
			}
			else if (this.actualZoomFactor == ZoomFactor.Zoom4 && this.zoom4Archive[this.actualGraphic] != null && this.zoom1Archive[this.actualGraphic] != null && this.zoom4Archive[this.actualGraphic].GetLayer(this.actualLayer) == null)
			{
				LayerID LayerID = GetLayerIDBySelectedIndex();
				this.zoom4Archive[this.actualGraphic].AddTransparentLayer(LayerID);
				this.ChangeLayer(LayerID);
			}
			try
			{
				uint[,] element = this.GetElement();

				if (e.X - drawPanel.AutoScrollPosition.X >= 20 && e.Y - drawPanel.AutoScrollPosition.Y >= 20 && (20 + this.zoomLevel * element.GetLength(0) + drawPanel.AutoScrollPosition.Y - e.Y) > 0)
				{
					int xElement = (int)(((e.X - 20 - drawPanel.AutoScrollPosition.X) / (float)this.zoomLevel) * (int)this.actualZoomFactor);
					int yElement = (int)(((20 + this.zoomLevel * element.GetLength(0) / (int)this.actualZoomFactor + drawPanel.AutoScrollPosition.Y - e.Y) / (float)this.zoomLevel) * (int)this.actualZoomFactor);
					if (xElement >= 0 && yElement >= 0 && xElement < element.GetLength(1) && yElement < element.GetLength(0))
					{
						if (e.Button == MouseButtons.Left && !element[yElement, xElement].Equals(leftPixel))
						{
							element[yElement, xElement] = leftPixel;
						}
						else if (e.Button == MouseButtons.Right && !element[yElement, xElement].Equals(rightPixel))
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
				this.ChangeLayer(LayerID.Foreground);
				Graphic graphic = this.GetActualGraphic();
				if(graphic.Properties.HasParticles)
				{
					if (graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Steam))
						this.particleComboBox.SelectedIndex = 1;
					else
						this.particleComboBox.SelectedIndex = 2;
					this.particleWidthNumericUpDown.Enabled = true;
					this.particleXNumericUpDown.Enabled = true;
					this.particleYNumericUpDown.Enabled = true;
					this.particleWidthNumericUpDown.Value = graphic.Properties.ParticleWidth;
					this.particleXNumericUpDown.Value = graphic.Properties.ParticleX;
					this.particleYNumericUpDown.Value = graphic.Properties.ParticleY;
				}
				else
				{
					this.particleComboBox.SelectedIndex = 0;
					this.particleWidthNumericUpDown.Enabled = false;
					this.particleXNumericUpDown.Enabled = false;
					this.particleYNumericUpDown.Enabled = false;
				}
				this.drawPanel.Invalidate();
			}
		}

		private void ChangeLayer(LayerID id)
		{
			if (this.actualLayer != id)
			{
				this.actualLayer = id;
				//int index;
				switch (id)
				{
					case LayerID.ToBackground:
						this.toBackgroundRadioButton.Checked = true;
						break;
					case LayerID.Foreground:
						this.foregroundRadioButton.Checked = true;
						break;
					case LayerID.Background0:
						this.backgroundRadioButton.Checked = true;
						break;
					case LayerID.Front:
						this.frontRadioButton.Checked = true;
						break;
					case LayerID.ForegroundAbove:
						this.foregroundAboveRadioButton.Checked = true;
						break;
					default:
						throw new Exception("Internal Error");
				}
				/*if (this.layerComboBox2.SelectedIndex != index)
				{
					this.layerSelectBoxCodeChanged = true;
					this.layerComboBox2.SelectedIndex = index;
				}*/
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

		private LayerID GetLayerIDBySelectedIndex()
		{
			if (this.foregroundRadioButton.Checked)
				return LayerID.Foreground;
			else if (this.backgroundRadioButton.Checked)
				return LayerID.Background0;
			else if (this.backgroundRadioButton2.Checked)
				return LayerID.Background1;
			else if (this.toBackgroundRadioButton.Checked)
				return LayerID.ToBackground;
			else if (this.foregroundAboveRadioButton.Checked)
				return LayerID.ForegroundAbove;
			else if (this.frontRadioButton.Checked)
				return LayerID.Front;
			else
				throw new Exception("Internal Error");
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

		private uint GetPixelFromComboBox(int index, uint lastPixel)
		{
			byte r = 0;
			byte g = 0;
			byte b = 0;
			if (lastPixel != 0 && !Pixel.IsTransparent(lastPixel) && !Pixel.IsSpecial(lastPixel) && Pixel.UsesRgb(lastPixel))
			{
				r = Pixel.GetRed(lastPixel);
				g = Pixel.GetGreen(lastPixel);
				b = Pixel.GetBlue(lastPixel);
			}
			switch (index)
			{
				case 2:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.BehindGlass);
				case 3:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.AlwaysBright);
				case 4:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.LampYellow);
				case 5:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.LampColdWhite);
				case 6:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.LampRed);
				case 7:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.LampYellowWhite);
				case 8:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.LampGasYellow);
				case 9:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.WindowYellow0);
				case 10:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.WindowYellow1);
				case 11:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.WindowYellow2);
				case 12:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.WindowNeon0);
				case 13:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.WindowNeon1);
				case 14:
					return Pixel.Create(r, g, b, Pixel.PixelProperty.WindowNeon2);
				case 15:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsBG);
				case 16:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsSleepers0);
				case 17:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsSleepers1);
				case 18:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsSleepers3);
				case 19:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsRoad0);
				case 20:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsRoad1);
				case 21:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsRoad2);
				case 22:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsRoad3);
				case 23:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsTrackbed0);
				case 24:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsTrackbed1);
				case 25:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsTrackbed2);
				case 26:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsRailsTrackbed3);
				case 27:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsMarkingPointBus0);
				case 28:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsMarkingPointBus1);
				case 29:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsMarkingPointBus2);
				case 30:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsMarkingPointBus3);
				case 31:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsMarkingPointWater);
				case 32:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsGravel);
				case 33:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsSmallGravel);
				case 34:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsGrassy);
				case 35:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsPathBG);
				case 36:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsPathFG);
				case 37:
					return Pixel.Create(0, 0, 0, Pixel.PixelProperty.AsText);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private uint[,] GetElement()
		{
			Graphic graphic = this.GetActualGraphic();
			if (graphic != null && graphic.GetLayer(this.actualLayer) != null)
			{
				return graphic.GetLayer(this.actualLayer);
			}
			return null;
		}

		private Graphic GetActualGraphic()
		{
			switch (this.actualZoomFactor)
			{
				case ZoomFactor.Zoom1:
					if (this.zoom1Archive != null)
					{
						return this.zoom1Archive[this.actualGraphic];
					}
					break;
				case ZoomFactor.Zoom2:
					if (this.zoom2Archive != null)
					{
						return this.zoom2Archive[this.actualGraphic];
					}
					break;
				case ZoomFactor.Zoom4:
					if (this.zoom4Archive != null)
					{
						return this.zoom4Archive[this.actualGraphic];
					}
					break;
				default:
					break;
			}
			return null;
		}

		private static uint PixelFromColor(Color color)
		{
			return Pixel.Create(color.R, color.G, color.B);
		}

		private static Color PixelToColor(uint pixel)
		{
			Pixel.PixelProperty property = Pixel.PixelProperty.Transparent;
			if (Pixel.UsesRgb(pixel))
			{
				property = Pixel.GetProperty(pixel);
			}
			else
			{
				property = (Pixel.PixelProperty)pixel;
			}
			switch (property)
			{
				//With RGB
				case Pixel.PixelProperty.None:
				case Pixel.PixelProperty.AlwaysBright:
				case Pixel.PixelProperty.LampYellow:
				case Pixel.PixelProperty.LampRed:
				case Pixel.PixelProperty.LampColdWhite:
				case Pixel.PixelProperty.LampYellowWhite:
				case Pixel.PixelProperty.LampGasYellow:
				case Pixel.PixelProperty.WindowYellow0:
				case Pixel.PixelProperty.WindowYellow1:
				case Pixel.PixelProperty.WindowYellow2:
				case Pixel.PixelProperty.WindowNeon0:
				case Pixel.PixelProperty.WindowNeon1:
				case Pixel.PixelProperty.WindowNeon2:
					return Color.FromArgb(Pixel.GetRed(pixel), Pixel.GetGreen(pixel), Pixel.GetBlue(pixel));

				//Without RGB
				case Pixel.PixelProperty.Transparent:
					return Color.FromArgb(0, 112, 0);
				case Pixel.PixelProperty.BehindGlass:
					return Color.FromArgb(0, 50, 100);
				case Pixel.PixelProperty.AsBG:
					return Color.FromArgb(0, 112, 0);
				case Pixel.PixelProperty.AsSleepers0:
					return Color.FromArgb(188, 188, 188);
				case Pixel.PixelProperty.AsSleepers1:
					return Color.FromArgb(84, 40, 0);
				case Pixel.PixelProperty.AsSleepers3:
					return Color.FromArgb(84, 40, 0);
				case Pixel.PixelProperty.AsRailsRoad0:
					return Color.FromArgb(168, 168, 168);
				case Pixel.PixelProperty.AsRailsRoad1:
					return Color.FromArgb(60, 60, 60);
				case Pixel.PixelProperty.AsRailsRoad2:
					return Color.FromArgb(168, 168, 168);
				case Pixel.PixelProperty.AsRailsRoad3:
					return Color.FromArgb(104, 104, 104);
				case Pixel.PixelProperty.AsRailsTrackbed0:
					return Color.FromArgb(104, 104, 104);
				case Pixel.PixelProperty.AsRailsTrackbed1:
					return Color.FromArgb(148, 148, 148);
				case Pixel.PixelProperty.AsRailsTrackbed2:
					return Color.FromArgb(148, 148, 148);
				case Pixel.PixelProperty.AsRailsTrackbed3:
					return Color.FromArgb(104, 104, 104);
				case Pixel.PixelProperty.AsMarkingPointBus0:
					return Color.FromArgb(252, 252, 252);
				case Pixel.PixelProperty.AsMarkingPointBus1:
					return Color.FromArgb(252, 252, 252);
				case Pixel.PixelProperty.AsMarkingPointBus2:
					return Color.FromArgb(252, 252, 252);
				case Pixel.PixelProperty.AsMarkingPointBus3:
					return Color.FromArgb(252, 252, 252);
				case Pixel.PixelProperty.AsMarkingPointWater:
					return Color.FromArgb(84, 252, 252);
				case Pixel.PixelProperty.AsGravel:
					return Color.FromArgb(60, 60, 60);
				case Pixel.PixelProperty.AsSmallGravel:
					return Color.FromArgb(168, 136, 0);
				case Pixel.PixelProperty.AsGrassy:
					return Color.FromArgb(0, 168, 0);
				case Pixel.PixelProperty.AsPathBG:
					return Color.FromArgb(30, 180, 20);
				case Pixel.PixelProperty.AsPathFG:
					return Color.FromArgb(168, 140, 0);
				case Pixel.PixelProperty.AsText:
					return Color.FromArgb(252, 252, 252);

				default:
					throw new ArgumentOutOfRangeException("property", "A property which was not defined here was encountered");
			}
		}

		private static uint[,] GraphicPreview(Graphic graphic)
		{
			uint[,] element = new uint[Constants.ElementHeight, Constants.ElementWidth];
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					if (graphic.GetLayer(LayerID.ForegroundAbove) != null && !Pixel.IsTransparent(graphic.GetLayer(LayerID.ForegroundAbove)[i + Constants.ElementHeight, j + Constants.ElementWidth]))
						element[i, j] = graphic.GetLayer(LayerID.ForegroundAbove)[i + Constants.ElementHeight, j + Constants.ElementWidth];
					else if (graphic.GetLayer(LayerID.Foreground) != null && !Pixel.IsTransparent(graphic.GetLayer(LayerID.Foreground)[i + Constants.ElementHeight, j + Constants.ElementWidth]))
						element[i, j] = graphic.GetLayer(LayerID.Foreground)[i + Constants.ElementHeight, j + Constants.ElementWidth];
					else if (graphic.GetLayer(LayerID.Front) != null && !Pixel.IsTransparent(graphic.GetLayer(LayerID.Front)[i + Constants.ElementHeight, j + Constants.ElementWidth]))
						element[i, j] = graphic.GetLayer(LayerID.Front)[i + Constants.ElementHeight, j + Constants.ElementWidth];
					else if (graphic.GetLayer(LayerID.Background1) != null && !Pixel.IsTransparent(graphic.GetLayer(LayerID.Background1)[i + Constants.ElementHeight, j + Constants.ElementWidth]))
						element[i, j] = graphic.GetLayer(LayerID.Background1)[i + Constants.ElementHeight, j + Constants.ElementWidth];
					else if (graphic.GetLayer(LayerID.Background0) != null && !Pixel.IsTransparent(graphic.GetLayer(LayerID.Background0)[i + Constants.ElementHeight, j + Constants.ElementWidth]))
						element[i, j] = graphic.GetLayer(LayerID.Background0)[i + Constants.ElementHeight, j + Constants.ElementWidth];
					else
						element[i, j] = Pixel.Create(0, 0, 0, Pixel.PixelProperty.Transparent);
				}
			}
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

		private void loadFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.LoadGraphicArchive();
		}

		private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.lastPath = this.saveFileDialog.FileName;
			this.SaveGraphicArchive();
		}

		private void leftColorButton_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.colorDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				Color c = this.colorDialog.Color;
				if (Pixel.IsSpecial(this.leftPixel) && Pixel.UsesRgb(this.leftPixel))
				{
					this.leftPixel = Pixel.Create(c.R, c.G, c.B, Pixel.GetProperty(this.leftPixel));
				}
				else
				{
					this.leftPixel = PixelFromColor(c);
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
				if (Pixel.IsSpecial(this.rightPixel) && Pixel.UsesRgb(this.rightPixel))
				{
					this.rightPixel = Pixel.Create(c.R, c.G, c.B, Pixel.GetProperty(this.rightPixel));
				}
				else
				{
					this.rightPixel = PixelFromColor(c);
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
				if (this.lastRightPixel != 0 && !Pixel.IsTransparent(this.lastRightPixel) && Pixel.UsesRgb(this.lastRightPixel))
				{
					if (Pixel.IsSpecial(this.lastRightPixel))
					{
						this.rightPixel = Pixel.Create(Pixel.GetRed(this.lastRightPixel), Pixel.GetGreen(this.lastRightPixel), Pixel.GetBlue(this.lastRightPixel));
					}
					else
					{
						this.rightPixel = this.lastRightPixel;
					}
				}
				else
				{
					this.rightPixel = Pixel.Create(0, 0, 0);
				}
			}
			else if (this.rightComboBox.SelectedIndex == 1)
			{
				if (!Pixel.IsTransparent(this.rightPixel))
				{
					this.lastRightPixel = this.rightPixel;
					this.rightPixel = Pixel.Create(0, 0, 0, Pixel.PixelProperty.Transparent);
				}
			}
			else
			{
				uint lastPixel = 0;
				if (this.rightPixel != 0 && !Pixel.IsTransparent(this.rightPixel) && Pixel.UsesRgb(this.rightPixel))
				{
					lastPixel = this.rightPixel;
				}
				else if (this.lastRightPixel != 0 && !Pixel.IsTransparent(this.lastRightPixel) && Pixel.UsesRgb(this.lastRightPixel))
				{
					lastPixel = this.lastRightPixel;
				}
				uint pixel = this.GetPixelFromComboBox(this.rightComboBox.SelectedIndex, lastPixel);
				if (pixel != 0)
				{
					if (!Pixel.IsTransparent(this.rightPixel) && Pixel.UsesRgb(this.rightPixel))
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
			this.rightColorButton.BackColor = PixelToColor(this.rightPixel);
		}

		private void leftComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.leftComboBox.SelectedIndex == 0)
			{
				if (this.lastLeftPixel != 0 && !Pixel.IsTransparent(this.lastLeftPixel) && Pixel.UsesRgb(this.lastLeftPixel))
				{
					if (Pixel.IsSpecial(this.lastLeftPixel))
					{
						this.leftPixel = Pixel.Create(Pixel.GetRed(this.lastLeftPixel), Pixel.GetGreen(this.lastLeftPixel), Pixel.GetBlue(this.lastLeftPixel));
					}
					else
					{
						this.leftPixel = this.lastLeftPixel;
					}
				}
				else
				{
					this.leftPixel = Pixel.Create(0, 0, 0);
				}
			}
			else if (this.leftComboBox.SelectedIndex == 1)
			{
				if (Pixel.IsTransparent(this.leftPixel) != true)
				{
					this.lastLeftPixel = this.leftPixel;
					this.leftPixel = Pixel.Create(0, 0, 0, Pixel.PixelProperty.Transparent);
				}
			}
			else
			{
				uint lastPixel = 0;
				if (this.leftPixel != 0 && !Pixel.IsTransparent(this.leftPixel) && Pixel.UsesRgb(this.leftPixel))
				{
					lastPixel = this.leftPixel;
				}
				else if (this.lastLeftPixel != 0 && !Pixel.IsTransparent(this.lastLeftPixel) && Pixel.UsesRgb(this.lastLeftPixel))
				{
					lastPixel = this.lastLeftPixel;
				}
				uint pixel = this.GetPixelFromComboBox(this.leftComboBox.SelectedIndex, lastPixel);
				if (pixel != 0)
				{
					if (!Pixel.IsTransparent(this.leftPixel) && Pixel.UsesRgb(this.leftPixel))
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
			this.leftColorButton.BackColor = PixelToColor(this.leftPixel);
		}

		private void Editor_Load(object sender, EventArgs e)
		{
			this.EditorLoaded();
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

					Graphic graphic = new Graphic("Kein Text", ZoomFactor.Zoom2);
					graphic.AddTransparentLayer(LayerID.Foreground);
					this.zoom2Archive.AddGraphic(this.actualGraphic, graphic);
					this.ChangeLayer(LayerID.Foreground);
					this.tabControl.TabPages.Insert(1, this.zoom2Tab);
					this.tabControl.SelectedTab = this.zoom2Tab;
					this.actualZoomFactor = ZoomFactor.Zoom2;
					this.drawPanel.Invalidate();
				}
				else
				{
					DialogResult result = DialogResult.Yes;
					if (!this.zoom2Archive[this.actualGraphic].IsTransparent())
						result = MessageBox.Show("Soll wirklich die Zoom2-Grafik gelöscht werden?", "Zoom2", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
					Graphic graphic = new Graphic("Kein Text", ZoomFactor.Zoom4);
					graphic.AddTransparentLayer(LayerID.Foreground);
					this.zoom4Archive.AddGraphic(this.actualGraphic, graphic);
					this.ChangeLayer(LayerID.Foreground);
					this.tabControl.TabPages.Add(this.zoom4Tab);
					this.tabControl.SelectedTab = this.zoom4Tab;
					this.actualZoomFactor = ZoomFactor.Zoom4;
					this.drawPanel.Invalidate();
				}
				else
				{
					DialogResult result = DialogResult.Yes;
					if (!this.zoom4Archive[this.actualGraphic].IsTransparent())
						result = MessageBox.Show("Soll wirklich die Zoom4-Grafik gelöscht werden?", "Zoom4", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

		private void foregroundRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void backgroundRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void backgroundRadioButton2_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void toBackgroundRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void foregroundAboveRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void frontRadioButton6_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void particleComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.GetActualGraphic();
			if (graphic != null)
			{
				switch (this.particleComboBox.SelectedIndex)
				{
					case 0:
						if (graphic.Properties.HasParticles)
						{
							graphic.Properties.RawData &= ~(GraphicProperties.Properties.Smoke & GraphicProperties.Properties.Steam);
							this.particleWidthNumericUpDown.Enabled = false;
							this.particleXNumericUpDown.Enabled = false;
							this.particleYNumericUpDown.Enabled = false;
						}
						break;
					case 1:
						graphic.Properties.RawData &= ~GraphicProperties.Properties.Smoke;
						graphic.Properties.RawData |= GraphicProperties.Properties.Steam;
						this.particleWidthNumericUpDown.Enabled = true;
						this.particleXNumericUpDown.Enabled = true;
						this.particleYNumericUpDown.Enabled = true;
						break;
					case 2:
						graphic.Properties.RawData &= ~GraphicProperties.Properties.Steam;
						graphic.Properties.RawData |= GraphicProperties.Properties.Smoke;
						this.particleWidthNumericUpDown.Enabled = true;
						this.particleXNumericUpDown.Enabled = true;
						this.particleYNumericUpDown.Enabled = true;
						break;
					default:
						throw new ArgumentOutOfRangeException("particleComboBox.SelectedIndex");
				}
			}
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.NewGraphicArchive();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ExitEditor())
				return;
			this.loadFileDialog.ShowDialog();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(false);
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(true);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void particleXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.GetActualGraphic();
			if(graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleX = (int)particleXNumericUpDown.Value;
			}
		}

		private void particleYNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.GetActualGraphic();
			if (graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleY = (int)particleYNumericUpDown.Value;
			}
		}

		private void particleWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.GetActualGraphic();
			if (graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleWidth = (int)particleWidthNumericUpDown.Value;
			}
		}

		#endregion
	}
}
