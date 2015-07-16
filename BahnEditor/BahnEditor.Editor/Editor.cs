﻿using BahnEditor.BahnLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace BahnEditor.Editor
{
	public partial class Editor : Form
	{
		#region Private Fields

		private static Brush transparentBrush = new HatchBrush(HatchStyle.Percent10, Color.FromArgb(140, 140, 140), Color.FromArgb(0, 112, 0));
		private int actualAlternative = 0;
		private int actualAnimationPhase = Constants.MinAnimationPhase;
		private int actualGraphic;
		private LayerID actualLayer;
		private ZoomFactor actualZoomFactor = ZoomFactor.Zoom1;
		private AnimationForm animationForm;
		private bool hasLoadedGraphic = false;
		private bool cursorNormalDirectionCBCodeChanged = false;
		private bool cursorReverseDirectionCBCodeChanged = false;
		private uint lastLeftPixel = 0;
		private string lastPath = "";
		private uint lastRightPixel = 0;
		private bool rightComboBoxSelectedCodeChanged = false;
		private bool leftComboBoxSelectedCodeChanged = false;
		private bool layerSelectBoxCodeChanged = false;
		private bool animationPhaseCodeChanged = false;
		private bool alternativeCheckBoxCodeChanged = false;
		private uint leftPixel = 0;
		private int overviewAlternative = 0;
		private int overviewLine = 0;
		private uint rightPixel = 255 << 16 | 255 << 8 | 255;
		private bool userMadeChanges = false;
		private GraphicArchive zoom1Archive;
		private GraphicArchive zoom2Archive;
		private GraphicArchive zoom4Archive;
		private int zoomLevelZoom1 = 4;
		private int zoomLevelZoom2 = 6;
		private int zoomLevelZoom4 = 8;

		#endregion Private Fields

		#region Internal Properties

		internal int ActualGraphicID
		{
			get
			{
				return this.actualGraphic;
			}
		}

		internal int ActualAlternativeID
		{
			get
			{
				return this.actualAlternative;
			}
		}

		internal GraphicArchive Zoom1Archive
		{
			get
			{
				return this.zoom1Archive;
			}
		}

		#endregion Internal Properties

		#region Private Properties

		private int ZoomLevel
		{
			get
			{
				switch (this.actualZoomFactor)
				{
					case ZoomFactor.Zoom1:
						return this.zoomLevelZoom1;
					case ZoomFactor.Zoom2:
						return this.zoomLevelZoom2;
					case ZoomFactor.Zoom4:
						return this.zoomLevelZoom4;
					default:
						return 6;
				}
			}
			set
			{
				switch (this.actualZoomFactor)
				{
					case ZoomFactor.Zoom1:
						this.zoomLevelZoom1 = value;
						break;
					case ZoomFactor.Zoom2:
						this.zoomLevelZoom2 = value;
						break;
					case ZoomFactor.Zoom4:
						this.zoomLevelZoom4 = value;
						break;
					default:
						throw new ArgumentException("Unknown zoomFactor");
				}
			}
		}

		private Graphic ActualGraphic
		{
			get
			{
				switch (this.actualZoomFactor)
				{
					case ZoomFactor.Zoom1:
						if (this.zoom1Archive != null)
						{
							return this.ActualZoom1Graphic;
						}
						break;

					case ZoomFactor.Zoom2:
						if (this.zoom2Archive != null)
						{
							return this.ActualZoom2Graphic;
						}
						break;

					case ZoomFactor.Zoom4:
						if (this.zoom4Archive != null)
						{
							return this.ActualZoom4Graphic;
						}
						break;

					default:
						break;
				}
				return null;
			}
		}

		private Graphic ActualZoom1Graphic
		{
			get
			{
				return this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, this.actualAlternative];
			}
		}

		private Graphic ActualZoom2Graphic
		{
			get
			{
				return this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, this.actualAlternative];
			}
		}

		private Graphic ActualZoom4Graphic
		{
			get
			{
				return this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, this.actualAlternative];
			}
		}

		#endregion Private Properties

		#region Public Constructors

		public Editor()
		{
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Private Methods

		private void EditorLoaded()
		{
			this.rightComboBox.SelectedIndex = 1;
			this.leftComboBox.SelectedIndex = 0;
			this.particleComboBox.SelectedIndex = 0;
			this.clockComboBox.SelectedIndex = 0;
			this.clockRotationComboBox.SelectedIndex = 0;
			this.animationForm = new AnimationForm(this);
			this.NewGraphicArchive();
		}

		private void NewGraphicArchive()
		{
			this.zoom1Archive = new GraphicArchive(ZoomFactor.Zoom1);
			this.zoom2Archive = new GraphicArchive(ZoomFactor.Zoom2);
			this.zoom4Archive = new GraphicArchive(ZoomFactor.Zoom4);
			this.actualGraphic = 0;
			this.actualAlternative = 0;
			this.alternativeCheckBoxCodeChanged = true;
			this.alternativesCheckBox.Checked = false;
			this.alternativeCheckBoxCodeChanged = false;
			this.actualAnimationPhase = 0;
			this.ChangeLayer(LayerID.Foreground);
			this.UserMadeChanges(false);
			this.animationPhaseCodeChanged = true;
			this.animationNumericUpDown.Value = Constants.MinAnimationPhase;
			this.animationPhaseCodeChanged = false;
			this.lastPath = "";
			this.UpdateProperties();
			this.ResizeDrawPanel();
			this.UpdateZoom();
			this.drawPanel.Invalidate();
			this.overviewPanel.Invalidate();
			this.UpdateAnimation();
		}

		private void LoadGraphicArchive()
		{
			try
			{
				this.zoom1Archive = GraphicArchive.Load(this.loadFileDialog.FileName);
				if (this.zoom1Archive.ZoomFactor != ZoomFactor.Zoom1)
				{
					throw new InvalidDataException("Zoomfactor of archive not 1");
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
				this.actualAnimationPhase = 0;
				this.actualZoomFactor = ZoomFactor.Zoom1;
				this.alternativeCheckBoxCodeChanged = true;
				if (this.zoom1Archive.HasAlternatives(this.actualGraphic))
				{
					this.alternativesCheckBox.Checked = true;
					this.actualAlternative = 1;
				}
				else
				{
					this.alternativesCheckBox.Checked = false;
					this.actualAlternative = 0;
				}
				this.alternativeCheckBoxCodeChanged = false;
				this.animationPhaseCodeChanged = true;
				this.animationNumericUpDown.Value = Constants.MinAnimationPhase;
				this.animationPhaseCodeChanged = false;
				this.UpdateProperties();
				this.lastPath = this.zoom1Archive.FileName;
				this.hasLoadedGraphic = true;
				this.UserMadeChanges(false);
				this.ChangeLayer(LayerID.Foreground);
				this.ResizeDrawPanel();
				this.UpdateZoom();
				this.drawPanel.Invalidate();
				this.overviewPanel.Invalidate();
				this.UpdateAnimation();
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("There was an error while loading: {0}!", ex.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.NewGraphicArchive();
			}
		}

		private bool SaveGraphicArchive()
		{
			try
			{
				this.RemoveUnusedGraphics();
				try
				{
					if (!this.zoom1Archive.Save(lastPath, true))
					{
						MessageBox.Show("There was an error while saving the zoom1-archive!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
				catch (ArchiveIsEmptyException)
				{
					MessageBox.Show("The archive is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				try
				{
					if (!this.zoom2Archive.Save(lastPath.Remove(lastPath.Length - 3) + "uz2", true))
					{
						MessageBox.Show("There was an error while saving the zoom2-archive!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
				catch (ArchiveIsEmptyException) { }
				try
				{
					if (!this.zoom4Archive.Save(lastPath.Remove(lastPath.Length - 3) + "uz4", true))
					{
						MessageBox.Show("There was an error while saving the zoom4-archive!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
				catch (ArchiveIsEmptyException) { }
				this.UserMadeChanges(false);
				return true;
			}
			catch (ElementIsEmptyException)
			{
				MessageBox.Show("A graphic is empty (transparent)!", "Invalid graphic!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}
		}

		private void DrawGraphic(Graphics g)
		{
			try
			{
				if ((this.actualZoomFactor == ZoomFactor.Zoom1 && (this.ActualZoom1Graphic == null || this.ActualZoom1Graphic.GetLayer(this.actualLayer) == null)) ||
					(this.actualZoomFactor == ZoomFactor.Zoom2 && (this.ActualZoom2Graphic == null || this.ActualZoom2Graphic.GetLayer(this.actualLayer) == null)) ||
					(this.actualZoomFactor == ZoomFactor.Zoom4 && (this.ActualZoom4Graphic == null || this.ActualZoom4Graphic.GetLayer(this.actualLayer) == null)))
				{
					//g.FillRectangle(new SolidBrush(Color.FromArgb(0, 112, 0)), 0, 0, Constants.ElementWidth * this.ZoomLevel * 3, Constants.ElementHeight * this.ZoomLevel * 8); //transparent 0, 112, 0
					g.FillRectangle(new TextureBrush(GetBackgroundBitmapByZoomlevel(this.ZoomLevel, this.actualZoomFactor)), 0, 0, Constants.ElementWidth * this.ZoomLevel * 3, Constants.ElementHeight * this.ZoomLevel * 8);
				}
				else
				{
					uint[,] element = this.GetElement();
					if (element != null)
					{
						if (g.ClipBounds.Width == (this.ZoomLevel * 2) / (int)this.actualZoomFactor && g.ClipBounds.Height == (this.ZoomLevel * 2) / (int)this.actualZoomFactor)
						{
							int x = (int)g.ClipBounds.X;
							int y = (int)g.ClipBounds.Y;
							int xElement = (int)((x / (float)this.ZoomLevel) * (int)this.actualZoomFactor);
							int yElement = (int)((y / (float)this.ZoomLevel) * (int)this.actualZoomFactor);
							yElement = ((Constants.ElementHeight * 8 * (int)this.actualZoomFactor) - yElement) - 1;
							//g.FillRectangle(transparentBrush, x, y, this.ZoomLevel * 2, this.ZoomLevel * 2);
							g.FillRectangle(new TextureBrush(GetBackgroundBitmapByZoomlevel(this.ZoomLevel, this.actualZoomFactor)), 0, 0, Constants.ElementWidth * this.ZoomLevel * 3, Constants.ElementHeight * this.ZoomLevel * 8);

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
											brush = new HatchBrush(HatchStyle.Percent30, Color.FromArgb(140, 140, 140), PixelToColor(element[i, j]));
										else
											brush = new SolidBrush(PixelToColor(element[i, j]));
										g.FillRectangle(brush, (j * ZoomLevel) / (int)this.actualZoomFactor, ((ZoomLevel * element.GetLength(0)) - (ZoomLevel * (i + 1))) / (int)this.actualZoomFactor, ZoomLevel / (float)this.actualZoomFactor, ZoomLevel / (float)this.actualZoomFactor);
									}
								}
							}
						}
						else
						{
							DrawElement(g, element, this.ZoomLevel, true, this.actualZoomFactor);
						}
					}
				}
				// Grid
				if (gridCheckBox.Checked)
				{
					for (int i = 1; i < 3; i++)
					{
						g.DrawLine(Pens.Gray, new Point(i * this.ZoomLevel * Constants.ElementWidth, 0), new Point(i * this.ZoomLevel * Constants.ElementWidth, this.ZoomLevel * Constants.ElementHeight * 8));
					}
					for (int i = 1; i < 8; i++)
					{
						g.DrawLine(Pens.Gray, new Point(0, i * this.ZoomLevel * Constants.ElementHeight), new Point(this.ZoomLevel * Constants.ElementWidth * 3, i * this.ZoomLevel * Constants.ElementHeight));
					}
					g.DrawRectangle(Pens.DarkGray, Constants.ElementWidth * (this.ZoomLevel), (6 * Constants.ElementHeight) * (this.ZoomLevel), Constants.ElementWidth * (this.ZoomLevel), Constants.ElementHeight * (this.ZoomLevel));

					// Grid for animations
					if (this.zoom1Archive.Animation != null && this.zoom1Archive.Animation[this.actualGraphic, this.actualAlternative] != null)
					{
						AnimationProgram program = this.zoom1Archive.Animation[this.actualGraphic, this.actualAlternative];
						Point corner1 = new Point(program.XDiff + 1, 8 - (program.YDiff + 1));
						Point corner2 = new Point(program.XDiff + 1 + program.Width, (8 - (program.YDiff + 1)) - program.Height);

						corner1.X = (corner1.X * Constants.ElementWidth) * (this.ZoomLevel);
						corner1.Y = (corner1.Y * Constants.ElementHeight) * (this.ZoomLevel);
						corner2.X = (corner2.X * Constants.ElementWidth) * (this.ZoomLevel);
						corner2.Y = (corner2.Y * Constants.ElementHeight) * (this.ZoomLevel);

						Rectangle rectangle = new Rectangle(Math.Min(corner1.X, corner2.X),
															Math.Min(corner1.Y, corner2.Y),
															Math.Abs(corner1.X - corner2.X),
															Math.Abs(corner1.Y - corner2.Y));
						Pen pen = new Pen(Brushes.Red);
						float[] dashValues = { 3, 3 };
						pen.DashPattern = dashValues;
						g.DrawRectangle(pen, rectangle);
					}
				}
			}
			catch (IndexOutOfRangeException)
			{
				MessageBox.Show("Internal Error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void DrawElement(Graphics g, uint[,] element, int ZoomLevel, bool withHatchBrush, ZoomFactor zoomfactor)
		{
			if (withHatchBrush)
				g.FillRectangle(new TextureBrush(GetBackgroundBitmapByZoomlevel(ZoomLevel, zoomfactor)), 0, 0, element.GetLength(1) * ZoomLevel / (int)zoomfactor, element.GetLength(0) * ZoomLevel / (int)zoomfactor);
			else
				g.FillRectangle(new SolidBrush(Color.FromArgb(0, 112, 0)), 0, 0, element.GetLength(1) * ZoomLevel / (int)zoomfactor, element.GetLength(0) * ZoomLevel / (int)zoomfactor);
			for (int i = 0; i < element.GetLength(0); i++)
			{
				for (int j = 0; j < element.GetLength(1); j++)
				{
					if (Pixel.IsTransparent(element[i, j]) != true)
					{
						Brush brush;
						if (withHatchBrush && Pixel.IsSpecial(element[i, j]))
							brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), PixelToColor(element[i, j]));
						else
							brush = new SolidBrush(PixelToColor(element[i, j]));
						g.FillRectangle(brush, (j * ZoomLevel) / (int)zoomfactor, ((ZoomLevel * element.GetLength(0)) - (ZoomLevel * (i + 1))) / (int)zoomfactor, ZoomLevel / (float)zoomfactor, ZoomLevel / (float)zoomfactor);
					}
				}
			}
		}

		private void DrawOverview(Graphics g)
		{
			if (this.zoom1Archive != null)
			{
				g.TranslateTransform(40, 15);
				for (int i = this.overviewLine * 18 + this.overviewAlternative, j = 1; j <= 9; i += 2, j++)
				{
					if (this.zoom1Archive[i] != null)
					{
						DrawElement(g, GraphicPreview(this.zoom1Archive[i]), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ElementWidth, Constants.ElementHeight);
					}
					if (i == this.actualGraphic)
						g.DrawRectangle(Pens.Blue, -1, -1, Constants.ElementWidth + 1, Constants.ElementHeight + 1);
					g.DrawString(String.Format("{0} - {1}", j, i), DefaultFont, Brushes.Black, 1, 20);
					g.TranslateTransform(Constants.ElementWidth + 10, 0);
				}

				if (this.alternativesCheckBox.Checked)
				{
					g.ResetTransform();
					g.TranslateTransform(440, 5);
					/*alternative a*/
					if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 1] != null)
					{
						DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 1]), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ElementWidth, Constants.ElementHeight);
					}
					if (this.actualAlternative == 1)
						g.DrawRectangle(Pens.Blue, -1, -1, Constants.ElementWidth + 1, Constants.ElementHeight + 1);
					g.DrawString("a", DefaultFont, Brushes.Black, 10, 15);

					/*alternative b*/
					g.TranslateTransform(Constants.ElementWidth + 10, 0);
					if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 2] != null)
					{
						DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 2]), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ElementWidth, Constants.ElementHeight);
					}
					if (this.actualAlternative == 2)
						g.DrawRectangle(Pens.Blue, -1, -1, Constants.ElementWidth + 1, Constants.ElementHeight + 1);
					g.DrawString("b", DefaultFont, Brushes.Black, 10, 15);

					/*alternative c*/
					g.TranslateTransform(-Constants.ElementWidth - 10, 30);
					if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 3] != null)
					{
						DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 3]), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ElementWidth, Constants.ElementHeight);
					}
					if (this.actualAlternative == 3)
						g.DrawRectangle(Pens.Blue, -1, -1, Constants.ElementWidth + 1, Constants.ElementHeight + 1);
					g.DrawString("c", DefaultFont, Brushes.Black, 10, 15);

					/*alternative d*/
					g.TranslateTransform(Constants.ElementWidth + 10, 0);
					if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 4] != null)
					{
						DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 4]), 1, false, ZoomFactor.Zoom1);
					}
					else
					{
						g.FillRectangle(transparentBrush, 0, 0, Constants.ElementWidth, Constants.ElementHeight);
					}
					if (this.actualAlternative == 4)
						g.DrawRectangle(Pens.Blue, -1, -1, Constants.ElementWidth + 1, Constants.ElementHeight + 1);
					g.DrawString("d", DefaultFont, Brushes.Black, 10, 15);
				}
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

		private void ClickGraphic(MouseEventArgs e)
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
				if (this.ActualZoom1Graphic == null)
				{
					Graphic graphic = new Graphic("No text");
					this.zoom1Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, graphic);
					this.UserMadeChanges(true);
					ChangePropertyComboBoxes(true);
					this.overviewPanel.Invalidate();
				}
				if (this.ActualZoom1Graphic.GetLayer(this.actualLayer) == null)
				{
					LayerID LayerID = GetLayerIDBySelectedIndex();
					this.ActualZoom1Graphic.AddTransparentLayer(LayerID);
					this.UserMadeChanges(true);
					this.ChangeLayer(LayerID);
				}
			}
			else if (this.actualZoomFactor == ZoomFactor.Zoom2)
			{
				if (this.ActualZoom2Graphic == null)
				{
					Graphic graphic = new Graphic("No text", zoomFactor: ZoomFactor.Zoom2);
					this.zoom2Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, graphic);
					this.UserMadeChanges(true);
					ChangePropertyComboBoxes(true);
					this.overviewPanel.Invalidate();
				}
				if (this.ActualZoom2Graphic.GetLayer(this.actualLayer) == null)
				{
					LayerID LayerID = GetLayerIDBySelectedIndex();
					this.ActualZoom2Graphic.AddTransparentLayer(LayerID);
					this.UserMadeChanges(true);
					this.ChangeLayer(LayerID);
				}
			}
			else if (this.actualZoomFactor == ZoomFactor.Zoom4)
			{
				if (this.ActualZoom4Graphic == null)
				{
					Graphic graphic = new Graphic("No text", zoomFactor: ZoomFactor.Zoom4);
					this.zoom4Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, graphic);
					this.UserMadeChanges(true);
					ChangePropertyComboBoxes(true);
					this.overviewPanel.Invalidate();
				}
				if (this.ActualZoom4Graphic.GetLayer(this.actualLayer) == null)
				{
					LayerID LayerID = GetLayerIDBySelectedIndex();
					this.ActualZoom4Graphic.AddTransparentLayer(LayerID);
					this.UserMadeChanges(true);
					this.ChangeLayer(LayerID);
				}
			}
			try
			{
				uint[,] element = this.GetElement();

				int xElement = (int)((e.X / (float)this.ZoomLevel) * (int)this.actualZoomFactor);
				int yElement = (int)((e.Y / (float)this.ZoomLevel) * (int)this.actualZoomFactor);
				yElement = ((Constants.ElementHeight * 8 * (int)this.actualZoomFactor) - yElement) - 1;
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
					this.UserMadeChanges(true);
					drawPanel.Invalidate(new Rectangle(e.X - this.ZoomLevel / ((int)this.actualZoomFactor), e.Y - this.ZoomLevel / ((int)this.actualZoomFactor), (this.ZoomLevel * 2) / (int)this.actualZoomFactor, (this.ZoomLevel * 2) / (int)this.actualZoomFactor));
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Internal Error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ClickOverview(MouseEventArgs e)
		{
			if (!e.Button.HasFlag(MouseButtons.Left))
				return;
			int element = -1;
			int x = e.X - 40;
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

				this.actualAnimationPhase = Constants.MinAnimationPhase;
				this.animationPhaseCodeChanged = true;
				this.animationNumericUpDown.Value = Constants.MinAnimationPhase;
				this.animationPhaseCodeChanged = false;

				this.alternativeCheckBoxCodeChanged = true;
				if (this.zoom1Archive.HasAlternatives(this.actualGraphic))
				{
					this.alternativesCheckBox.Checked = true;
					this.actualAlternative = 1;
				}
				else
				{
					this.alternativesCheckBox.Checked = false;
					this.actualAlternative = 0;
				}
				this.alternativeCheckBoxCodeChanged = false;

				this.UpdateProperties();
				this.drawPanel.Invalidate();
				this.overviewPanel.Invalidate();
				this.UpdateAnimation();
			}
			else if (this.alternativesCheckBox.Checked)
			{
				int xa = e.X - 440;
				int ya = e.Y - 5;
				int alternative = -1;
				if (xa >= 0 && Constants.ElementWidth >= xa)
				{
					if (ya >= 0 && 25 >= ya)
					{
						alternative = 1;
					}
					else if (ya >= 30 && 55 >= ya)
					{
						alternative = 3;
					}
				}
				else if (xa >= Constants.ElementWidth + 10 && Constants.ElementWidth * 2 + 10 >= xa)
				{
					if (ya >= 0 && 25 >= ya)
					{
						alternative = 2;
					}
					else if (ya >= 30 && 55 >= ya)
					{
						alternative = 4;
					}
				}
				if (alternative != -1 && alternative != this.actualAlternative)
				{
					this.actualAlternative = alternative;
					this.UpdateProperties();
					this.overviewPanel.Invalidate();
					this.drawPanel.Invalidate();
					this.UpdateAnimation();
				}
			}
		}

		private static Bitmap GetBackgroundBitmapByZoomlevel(int zoomLevel, ZoomFactor zoomFactor)
		{
			float zoom = (float)zoomLevel / (float)zoomFactor;
			switch ((int)zoom)
			{
				case 1:
				default:
					return Properties.Resources.background;
				case 2:
					return Properties.Resources.background2;
				case 3:
					return Properties.Resources.background3;
				case 4:
					return Properties.Resources.background4;
				case 5:
					return Properties.Resources.background5;
				case 6:
					return Properties.Resources.background6;
				case 7:
					return Properties.Resources.background7;
				case 8:
					return Properties.Resources.background8;
				case 9:
					return Properties.Resources.background9;
				case 10:
					return Properties.Resources.background10;
				case 11:
					return Properties.Resources.background11;
				case 12:
					return Properties.Resources.background12;

			}
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

		private bool AskSaveGraphic()
		{
			if (this.userMadeChanges == true)
			{
				DialogResult dr = MessageBox.Show("Do you want to save the graphic?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dr == DialogResult.Cancel)
				{
					return false;
				}
				else if (dr == DialogResult.Yes)
				{
					this.ClickOnSaveButton(false);
				}
			}
			return true;
		}

		private void ChangeLayer(LayerID id)
		{
			if (this.actualLayer != id)
			{
				this.actualLayer = id;
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
			}
		}

		private void ClickOnSaveButton(bool forceSave)
		{
			if (lastPath == "" || forceSave)
			{
				this.saveFileDialog.ShowDialog();
			}
			else
			{
				this.SaveGraphicArchive();
			}
		}
		private uint[,] GetElement()
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.GetLayer(this.actualLayer) != null)
			{
				return graphic.GetLayer(this.actualLayer);
			}
			return null;
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

		private void OpenAnimationForm()
		{
			if (animationForm.Created && animationForm.Visible)
			{
				animationForm.Hide();
				return;
			}
			if (animationForm.IsDisposed)
				animationForm = new AnimationForm(this);
			animationForm.Show();
		}

		private void ResizeDrawPanel()
		{
			this.drawPanel.Size = new Size((int)((this.ZoomLevel) * Constants.ElementWidth * 3), (int)((this.ZoomLevel) * Constants.ElementHeight * 8));
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

		private void ChangePropertyComboBoxes(bool enabled)
		{
			this.particleComboBox.Enabled = enabled;
			this.clockComboBox.Enabled = enabled;
			this.cursorNormalDirectionComboBox.Enabled = enabled;
			this.cursorReverseDirectionComboBox.Enabled = enabled;
		}

		private void UpdateProperties()
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic == null)
			{
				ChangePropertyComboBoxes(false);
			}
			else
			{
				ChangePropertyComboBoxes(true);
			}
			if (graphic != null && graphic.Properties.HasParticles)
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
			else if (this.particleComboBox.SelectedIndex != 0)
			{
				this.particleComboBox.SelectedIndex = 0;
				this.particleWidthNumericUpDown.Enabled = false;
				this.particleXNumericUpDown.Enabled = false;
				this.particleYNumericUpDown.Enabled = false;
				this.particleWidthNumericUpDown.Value = 0;
				this.particleXNumericUpDown.Value = 0;
				this.particleYNumericUpDown.Value = 0;
			}
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				if (graphic.Properties.ClockProperties.HasFlag(ClockProperties.Display24h))
					this.clockComboBox.SelectedIndex = 2;
				else
					this.clockComboBox.SelectedIndex = 1;
				this.clockColorHoursPointerButton.Enabled = true;
				this.clockColorMinutesPointerButton.Enabled = true;
				this.clockMinutesPointerCheckBox.Enabled = true;
				this.clockRotationComboBox.Enabled = true;
				this.clockWidthNumericUpDown.Enabled = true;
				this.clockXNumericUpDown.Enabled = true;
				this.clockYNumericUpDown.Enabled = true;
				this.clockXNumericUpDown.Value = graphic.Properties.ClockX;
				this.clockYNumericUpDown.Value = graphic.Properties.ClockY;
				this.clockWidthNumericUpDown.Value = graphic.Properties.ClockWidth;
				this.clockMinutesPointerCheckBox.Checked = graphic.Properties.ClockProperties.HasFlag(ClockProperties.MinutePointer);
				if (graphic.Properties.ClockProperties.HasFlag(ClockProperties.RotatedNorthWest))
					this.clockRotationComboBox.SelectedIndex = 1;
				else if (graphic.Properties.ClockProperties.HasFlag(ClockProperties.RotatedNorthEast))
					this.clockRotationComboBox.SelectedIndex = 2;
				else
					this.clockRotationComboBox.SelectedIndex = 0;
				this.clockColorHoursPointerButton.BackColor = PixelToColor(graphic.Properties.ClockColorHoursPointer);
				this.clockColorMinutesPointerButton.BackColor = PixelToColor(graphic.Properties.ClockColorMinutesPointer);
			}
			else if (this.clockComboBox.SelectedIndex != 0)
			{
				this.clockComboBox.SelectedIndex = 0;
				this.clockColorHoursPointerButton.Enabled = false;
				this.clockColorMinutesPointerButton.Enabled = false;
				this.clockMinutesPointerCheckBox.Enabled = false;
				this.clockRotationComboBox.Enabled = false;
				this.clockWidthNumericUpDown.Enabled = false;
				this.clockXNumericUpDown.Enabled = false;
				this.clockYNumericUpDown.Enabled = false;
				this.clockXNumericUpDown.Value = 0;
				this.clockYNumericUpDown.Value = 0;
				this.clockWidthNumericUpDown.Value = 0;
				this.clockMinutesPointerCheckBox.Checked = false;
				this.clockRotationComboBox.SelectedIndex = 0;
				this.clockColorHoursPointerButton.BackColor = Color.Black;
				this.clockColorMinutesPointerButton.BackColor = Color.Black;
			}
			Graphic z1Graphic = this.ActualZoom1Graphic;
			if (z1Graphic != null && z1Graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Cursor))
			{
				this.cursorNormalDirectionCBCodeChanged = true;
				this.cursorReverseDirectionCBCodeChanged = true;
				this.cursorNormalDirectionComboBox.SelectedIndex = (int)z1Graphic.Properties.CursorNormalDirection + 1;
				this.cursorReverseDirectionComboBox.SelectedIndex = (int)z1Graphic.Properties.CursorReverseDirection + 1;
				this.cursorNormalDirectionCBCodeChanged = false;
				this.cursorReverseDirectionCBCodeChanged = false;
			}
			else if (this.cursorNormalDirectionComboBox.SelectedIndex > -1 || this.cursorReverseDirectionComboBox.SelectedIndex > -1)
			{
				this.cursorNormalDirectionCBCodeChanged = true;
				this.cursorReverseDirectionCBCodeChanged = true;
				this.cursorNormalDirectionComboBox.SelectedIndex = -1;
				this.cursorReverseDirectionComboBox.SelectedIndex = -1;
				this.cursorNormalDirectionCBCodeChanged = false;
				this.cursorReverseDirectionCBCodeChanged = false;
			}
		}

		private void RemoveUnusedGraphics()
		{
			for (int element = 0; element < Constants.MaxElementsInArchive; element++)
			{
				for (int alternative = 0; alternative <= Constants.MaxAlternative; alternative++)
				{
					for (int animationPhase = 0; animationPhase <= Constants.MaxAnimationPhase; animationPhase++)
					{
						if (this.zoom1Archive[element, animationPhase, alternative] != null && this.zoom1Archive[element, animationPhase, alternative].IsTransparent())
						{
							this.zoom1Archive.RemoveGraphic(element, animationPhase, alternative);
						}
						if (this.zoom2Archive[element, animationPhase, alternative] != null && this.zoom2Archive[element, animationPhase, alternative].IsTransparent())
						{
							this.zoom2Archive.RemoveGraphic(element, animationPhase, alternative);
						}
						if (this.zoom4Archive[element, animationPhase, alternative] != null && this.zoom4Archive[element, animationPhase, alternative].IsTransparent())
						{
							this.zoom4Archive.RemoveGraphic(element, animationPhase, alternative);
						}
					}
				}
			}
		}

		private void UpdateAnimation()
		{
			if (this.animationForm != null && !this.animationForm.IsDisposed)
				this.animationForm.ChangeAnimationProgram();
		}

		private void ZoomInOut(bool zoomIn)
		{
			if (zoomIn)
			{
				this.ZoomLevel += 2;
			}
			else
			{
				this.ZoomLevel -= 2;
			}

			this.UpdateZoom();
		}

		private void UpdateZoom()
		{
			if (this.ZoomLevel > 10 + (int)this.actualZoomFactor)
			{
				this.zoomInButton.Enabled = false;
			}
			else
			{
				this.zoomInButton.Enabled = true;
			}
			if (this.ZoomLevel < 4 + (int)this.actualZoomFactor)
			{
				this.zoomOutButton.Enabled = false;
			}
			else
			{
				this.zoomOutButton.Enabled = true;
			}
			this.zoomLevelStatusLabel.Text = String.Format("Zoomlevel: {0}  Zoomfactor: {1}", this.ZoomLevel, this.actualZoomFactor.ToString());
			this.ResizeDrawPanel();
			this.drawPanel.Invalidate();
		}

		#endregion Private Methods

		#region Internal Methods

		internal void UpdateAnimation(bool animationExists)
		{
			if (animationExists)
			{
				this.animationNumericUpDown.Enabled = true;
			}
			else
			{
				this.animationNumericUpDown.Enabled = false;
			}
			drawPanel.Invalidate();
		}

		internal void ResetAnimationNumericUpDown()
		{
			this.actualAnimationPhase = 0;
			this.animationPhaseCodeChanged = true;
			this.animationNumericUpDown.Value = 0;
			this.animationPhaseCodeChanged = false;
		}

		internal void UserMadeChanges(bool userMadeChanges)
		{
			this.userMadeChanges = userMadeChanges;
			this.Text = userMadeChanges ? "Bahn Editor *" : "Bahn Editor";
		}

		#endregion Internal Methods

		#region Event-Handler

		private void animationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.OpenAnimationForm();
		}

		private void backgroundRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void backgroundRadioButton2_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void clockColorHoursPointerButton_Click(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				DialogResult dr = this.colorDialog.ShowDialog();
				if (dr == DialogResult.OK)
				{
					graphic.Properties.ClockColorHoursPointer = PixelFromColor(this.colorDialog.Color);
					this.UserMadeChanges(true);
					this.clockColorHoursPointerButton.BackColor = this.colorDialog.Color;
				}
			}
		}

		private void clockColorMinutesPointerButton_Click(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				DialogResult dr = this.colorDialog.ShowDialog();
				if (dr == DialogResult.OK)
				{
					graphic.Properties.ClockColorMinutesPointer = PixelFromColor(this.colorDialog.Color);
					this.UserMadeChanges(true);
					this.clockColorMinutesPointerButton.BackColor = this.colorDialog.Color;
				}
			}
		}

		private void clockComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null)
			{
				switch (this.clockComboBox.SelectedIndex)
				{
					case 0:
						if (graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
						{
							graphic.Properties.RawData &= ~(GraphicProperties.Properties.Clock);
							this.clockColorHoursPointerButton.Enabled = false;
							this.clockColorMinutesPointerButton.Enabled = false;
							this.clockMinutesPointerCheckBox.Enabled = false;
							this.clockRotationComboBox.Enabled = false;
							this.clockWidthNumericUpDown.Enabled = false;
							this.clockXNumericUpDown.Enabled = false;
							this.clockYNumericUpDown.Enabled = false;
							this.UserMadeChanges(true);
						}
						break;

					case 1:
						graphic.Properties.RawData |= GraphicProperties.Properties.Clock;
						graphic.Properties.ClockProperties &= ~ClockProperties.Display24h;
						graphic.Properties.ClockZ = this.actualLayer;
						this.clockColorHoursPointerButton.Enabled = true;
						this.clockColorMinutesPointerButton.Enabled = true;
						this.clockMinutesPointerCheckBox.Enabled = true;
						this.clockRotationComboBox.Enabled = true;
						this.clockWidthNumericUpDown.Enabled = true;
						this.clockXNumericUpDown.Enabled = true;
						this.clockYNumericUpDown.Enabled = true;
						this.UserMadeChanges(true);
						break;

					case 2:
						graphic.Properties.RawData |= GraphicProperties.Properties.Clock;
						graphic.Properties.ClockProperties |= ClockProperties.Display24h;
						graphic.Properties.ClockZ = this.actualLayer;
						this.clockColorHoursPointerButton.Enabled = true;
						this.clockColorMinutesPointerButton.Enabled = true;
						this.clockMinutesPointerCheckBox.Enabled = true;
						this.clockRotationComboBox.Enabled = true;
						this.clockWidthNumericUpDown.Enabled = true;
						this.clockXNumericUpDown.Enabled = true;
						this.clockYNumericUpDown.Enabled = true;
						this.UserMadeChanges(true);
						break;

					default:
						throw new ArgumentOutOfRangeException("clockComboBox.SelectedIndex");
				}
			}
		}

		private void clockMinutesPointerCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null)
			{
				if (clockMinutesPointerCheckBox.Checked)
				{
					graphic.Properties.ClockProperties |= ClockProperties.MinutePointer;
				}
				else
				{
					graphic.Properties.ClockProperties &= ~ClockProperties.MinutePointer;
				}
				this.UserMadeChanges(true);
			}
		}

		private void clockRotationComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null)
			{
				switch (this.clockRotationComboBox.SelectedIndex)
				{
					case 0:
						graphic.Properties.ClockProperties &= ~(ClockProperties.RotatedNorthEast & ClockProperties.RotatedNorthWest);
						break;

					case 1:
						graphic.Properties.ClockProperties &= ~ClockProperties.RotatedNorthEast;
						graphic.Properties.ClockProperties |= ClockProperties.RotatedNorthWest;
						break;

					case 2:
						graphic.Properties.ClockProperties &= ~ClockProperties.RotatedNorthWest;
						graphic.Properties.ClockProperties |= ClockProperties.RotatedNorthEast;
						break;

					default:
						throw new ArgumentOutOfRangeException("clockRotationComboBox.SelectedIndex");
				}
				this.UserMadeChanges(true);
			}
		}

		private void clockWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				graphic.Properties.ClockWidth = (int)clockWidthNumericUpDown.Value;
				graphic.Properties.ClockHeight = (int)clockWidthNumericUpDown.Value;
				this.UserMadeChanges(true);
			}
		}

		private void clockXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				graphic.Properties.ClockX = (int)clockXNumericUpDown.Value;
				this.UserMadeChanges(true);
			}
		}

		private void clockYNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				graphic.Properties.ClockY = (int)clockYNumericUpDown.Value;
				this.UserMadeChanges(true);
			}
		}

		private void cursorNormalDirectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cursorNormalDirectionCBCodeChanged)
			{
				return;
			}
			Graphic graphic = this.ActualZoom1Graphic;

			if (graphic != null)
			{
				if (cursorNormalDirectionComboBox.SelectedIndex == 0)
				{
					graphic.Properties.RawData &= ~GraphicProperties.Properties.Cursor;
					this.cursorReverseDirectionCBCodeChanged = true;
					this.cursorReverseDirectionComboBox.SelectedIndex = 0;
					this.cursorReverseDirectionCBCodeChanged = false;
				}
				else
				{
					graphic.Properties.RawData |= GraphicProperties.Properties.Cursor;
					graphic.Properties.CursorNormalDirection = (Direction)(cursorNormalDirectionComboBox.SelectedIndex - 1);
				}
				this.UserMadeChanges(true);
			}
		}

		private void cursorReverseDirectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cursorReverseDirectionCBCodeChanged)
			{
				return;
			}
			Graphic graphic = this.ActualZoom1Graphic;

			if (graphic != null)
			{
				if (cursorReverseDirectionComboBox.SelectedIndex == 0)
				{
					graphic.Properties.RawData &= ~GraphicProperties.Properties.Cursor;
					this.cursorNormalDirectionCBCodeChanged = true;
					this.cursorNormalDirectionComboBox.SelectedIndex = 0;
					this.cursorNormalDirectionCBCodeChanged = false;
				}
				else
				{
					graphic.Properties.RawData |= GraphicProperties.Properties.Cursor;
					graphic.Properties.CursorReverseDirection = (Direction)(cursorReverseDirectionComboBox.SelectedIndex - 1);
				}
				this.UserMadeChanges(true);
			}
		}

		private void drawPanel_MouseClick(object sender, MouseEventArgs e)
		{
			this.ClickGraphic(e);
		}

		private void drawPanel_MouseMove(object sender, MouseEventArgs e)
		{
			this.ClickGraphic(e);
		}

		private void drawPanel_MouseUp(object sender, MouseEventArgs e)
		{
			this.overviewPanel.Invalidate();
		}

		private void drawPanel_Paint(object sender, PaintEventArgs e)
		{
			this.DrawGraphic(e.Graphics);
		}

		private void Editor_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !this.AskSaveGraphic();
		}

		private void Editor_Load(object sender, EventArgs e)
		{
			this.EditorLoaded();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void foregroundAboveRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void foregroundRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void frontRadioButton6_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
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
					this.leftComboBoxSelectedCodeChanged = true;
					this.leftComboBox.SelectedIndex = 0;
					this.leftComboBoxSelectedCodeChanged = false;
					this.leftPixel = PixelFromColor(c);
				}
				this.leftColorButton.BackColor = c;
			}
		}

		private void leftComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.leftComboBoxSelectedCodeChanged)
				return;
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
					MessageBox.Show("Internal Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			this.leftColorButton.BackColor = PixelToColor(this.leftPixel);
		}

		private void loadFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.LoadGraphicArchive();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!this.AskSaveGraphic())
				return;
			this.NewGraphicArchive();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!this.AskSaveGraphic())
				return;
			this.loadFileDialog.ShowDialog();
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

		private void overviewLeftRightButton_Click(object sender, EventArgs e)
		{
			if (this.overviewAlternative == 1)
				this.overviewAlternative = 0;
			else
				this.overviewAlternative = 1;
			this.overviewPanel.Invalidate();
			drawPanel.Focus();
		}

		private void overviewPanel_Click(object sender, EventArgs e)
		{
			MouseEventArgs me = e as MouseEventArgs;
			if (me != null)
				ClickOverview(me);
			else
				MessageBox.Show("Internal Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void overviewPanel_Paint(object sender, PaintEventArgs e)
		{
			this.DrawOverview(e.Graphics);
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

		private void particleComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
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
							this.UserMadeChanges(true);
						}
						break;

					case 1:
						graphic.Properties.RawData &= ~GraphicProperties.Properties.Smoke;
						graphic.Properties.RawData |= GraphicProperties.Properties.Steam;
						this.particleWidthNumericUpDown.Enabled = true;
						this.particleXNumericUpDown.Enabled = true;
						this.particleYNumericUpDown.Enabled = true;
						this.UserMadeChanges(true);
						break;

					case 2:
						graphic.Properties.RawData &= ~GraphicProperties.Properties.Steam;
						graphic.Properties.RawData |= GraphicProperties.Properties.Smoke;
						this.particleWidthNumericUpDown.Enabled = true;
						this.particleXNumericUpDown.Enabled = true;
						this.particleYNumericUpDown.Enabled = true;
						this.UserMadeChanges(true);
						break;

					default:
						throw new ArgumentOutOfRangeException("particleComboBox.SelectedIndex");
				}
			}
		}

		private void particleWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleWidth = (int)particleWidthNumericUpDown.Value;
				this.UserMadeChanges(true);
			}
		}

		private void particleXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleX = (int)particleXNumericUpDown.Value;
				this.UserMadeChanges(true);
			}
		}

		private void particleYNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleY = (int)particleYNumericUpDown.Value;
				this.UserMadeChanges(true);
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
					this.rightComboBoxSelectedCodeChanged = true;
					this.rightComboBox.SelectedIndex = 0;
					this.rightComboBoxSelectedCodeChanged = false;
					this.rightPixel = PixelFromColor(c);
				}
				this.rightColorButton.BackColor = c;
			}
		}

		private void rightComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.rightComboBoxSelectedCodeChanged)
				return;
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
					MessageBox.Show("Internal Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			this.rightColorButton.BackColor = PixelToColor(this.rightPixel);
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(true);
		}

		private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.lastPath = this.saveFileDialog.FileName;
			if (!this.SaveGraphicArchive())
			{
				this.ClickOnSaveButton(true);
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ClickOnSaveButton(false);
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
			this.UpdateProperties();
			this.UpdateZoom();
			this.drawPanel.Invalidate();
		}

		private void toBackgroundRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void zoomInButton_Click(object sender, EventArgs e)
		{
			this.ZoomInOut(true);
		}

		private void zoomOutButton_Click(object sender, EventArgs e)
		{
			this.ZoomInOut(false);
		}

		private void animationNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (this.animationPhaseCodeChanged)
				return;
			this.actualAnimationPhase = (int)this.animationNumericUpDown.Value;
			this.UpdateProperties();
			this.drawPanel.Invalidate();
		}

		private void propertiesGroupBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.DarkGray, 1, 76, 198, 76);
			e.Graphics.DrawLine(Pens.DarkGray, 1, 242, 198, 242);
		}

		private void alternativesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (alternativeCheckBoxCodeChanged)
				return;
			if (this.alternativesCheckBox.Checked)
			{
				if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative] != null && !this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative].IsTransparent())
				{
					this.zoom1Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, 1, this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative]);
					this.zoom1Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative);
					if (this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative] != null && !this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative].IsTransparent())
					{
						this.zoom2Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, 1, this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative]);
						this.zoom2Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative);
					}
					if (this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative] != null && !this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative].IsTransparent())
					{
						this.zoom4Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, 1, this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative]);
						this.zoom4Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative);
					}
					if (this.zoom1Archive.Animation != null && this.zoom1Archive.Animation[this.actualGraphic, Constants.NoAlternative] != null)
					{
						this.zoom1Archive.Animation.AddAnimationProgram(this.zoom1Archive.Animation[this.actualGraphic, Constants.NoAlternative], this.actualGraphic, 1);
						this.zoom1Archive.Animation.RemoveAnimationProgram(this.actualGraphic, Constants.NoAlternative);
					}
				}
				this.actualAlternative = 1;
				this.UserMadeChanges(true);
			}
			else
			{
				if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 1] != null && !this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 1].IsTransparent())
				{
					if ((this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 2] != null && !this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 2].IsTransparent())
						|| (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 3] != null && !this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 3].IsTransparent())
						|| (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 4] != null && !this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 4].IsTransparent()))
					{
						if (MessageBox.Show("Only the first alternative will be saved!" + Environment.NewLine + "The other alternatives will be deleted!", "Alternatives", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
						{
							this.alternativeCheckBoxCodeChanged = true;
							this.alternativesCheckBox.Checked = true;
							this.alternativeCheckBoxCodeChanged = false;
							return;
						}
						else
						{
							for (int i = 2; i <= Constants.MaxAlternative; i++)
							{
								if (this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, i] != null)
								{
									this.zoom1Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, i);
									if (this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, i] != null) this.zoom2Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, i);
									if (this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, i] != null) this.zoom4Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, i);
									if (this.zoom1Archive.Animation != null && this.zoom1Archive.Animation[this.actualGraphic, i] != null) this.zoom1Archive.Animation.RemoveAnimationProgram(this.actualGraphic, i);
								}
							}
						}
					}

					this.zoom1Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative, this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 1]);
					this.zoom1Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, 1);
					if (this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, 1] != null && !this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, 1].IsTransparent())
					{
						this.zoom2Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative, this.zoom2Archive[this.actualGraphic, this.actualAnimationPhase, 1]);
						this.zoom2Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, 1);
					}
					if (this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, 1] != null && !this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, 1].IsTransparent())
					{
						this.zoom4Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, Constants.NoAlternative, this.zoom4Archive[this.actualGraphic, this.actualAnimationPhase, 1]);
						this.zoom4Archive.RemoveGraphic(this.actualGraphic, this.actualAnimationPhase, 1);
					}
					if (this.zoom1Archive.Animation != null && this.zoom1Archive.Animation[this.actualGraphic, 1] != null)
					{
						this.zoom1Archive.Animation.AddAnimationProgram(this.zoom1Archive.Animation[this.actualGraphic, 1], this.actualGraphic, Constants.NoAlternative);
						this.zoom1Archive.Animation.RemoveAnimationProgram(this.actualGraphic, 1);
					}
				}
				this.actualAlternative = Constants.NoAlternative;
				this.UserMadeChanges(true);
			}
			this.overviewPanel.Invalidate();
			this.drawPanel.Invalidate();
			this.UpdateAnimation();
		}

		private void gridCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.drawPanel.Invalidate();
		}

		#endregion Event-Handler
	}
}