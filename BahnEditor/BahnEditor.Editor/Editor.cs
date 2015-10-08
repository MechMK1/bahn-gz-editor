﻿using BahnEditor.BahnLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BahnEditor.Editor
{
	public partial class Editor : Form
	{
		#region Private Fields

		//transparent brush
		private static Brush transparentBrush = new HatchBrush(HatchStyle.Percent10, Color.FromArgb(140, 140, 140), Color.FromArgb(0, 112, 0));

		//actual selected graphic

		private int actualAlternative = 0;
		private int actualAnimationPhase = Constants.MinAnimationPhase;
		private int actualGraphic;
		private LayerID actualLayer;

		//special drawing

		private bool drawingSpecialMode = false;
		private Point specialModeStartPoint;
		private Point specialModeEndPoint;
		private MouseButtons specialModeMouseButton;

		//selection

		private Point selectStartPoint;
		private Point selectEndPoint;
		private bool selected = false;
		private bool drawingSelection = false;

		//variables to check if value was changed in the code

		private bool cursorNormalDirectionComboBoxCC = false;
		private bool cursorReverseDirectionComboBoxCC = false;
		private bool tabControlSelectedCC = false;
		private bool rightComboBoxCC = false;
		private bool leftComboBoxCC = false;
		private bool layerSelectBoxCC = false;
		private bool animationNumbericUpDownCC = false;
		private bool alternativeCheckBoxCC = false;

		//variables for color of left and right mousebuttons

		private uint lastLeftPixel = 0;
		private uint lastRightPixel = 0;
		private uint leftPixel = 0;
		private uint rightPixel = 255 << 16 | 255 << 8 | 255;

		//position in the overviews

		private int overviewAlternative = 0;
		private int overviewLine = 0;

		//other variables

		private string lastPath = "";
		private bool userMadeChanges = false;
		private AnimationForm animationForm;
		private GraphicArchive zoom1Archive;
		private GraphicArchive zoom2Archive;
		private GraphicArchive zoom4Archive;
		private Dictionary<Tuple<int, int, int, LayerID, ZoomFactor>, UndoRedoStack> urStack;
		private List<Point> changes;
		private bool isMouseCaptured = false;

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

		private UndoRedoStack UndoRedo
		{
			get
			{
				Tuple<int, int, int, LayerID, ZoomFactor> tuple = Tuple.Create<int, int, int, LayerID, ZoomFactor>(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, this.actualLayer, this.graphicPanel.ZoomFactor);

				if (!urStack.ContainsKey(tuple))
					urStack.Add(tuple, new UndoRedoStack());

				return urStack[tuple];
			}
		}

		private bool UndoRedoExists
		{
			get
			{
				return urStack.ContainsKey(Tuple.Create<int, int, int, LayerID, ZoomFactor>(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, this.actualLayer, this.graphicPanel.ZoomFactor));
			}
		}

		private Graphic ActualGraphic
		{
			get
			{
				switch (this.graphicPanel.ZoomFactor)
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

		#region Editor Methods

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
			this.urStack = new Dictionary<Tuple<int, int, int, LayerID, ZoomFactor>, UndoRedoStack>();
			this.actualGraphic = 0;
			this.actualAlternative = 0;
			this.alternativeCheckBoxCC = true;
			this.alternativesCheckBox.Checked = false;
			this.alternativeCheckBoxCC = false;
			this.actualAnimationPhase = 0;
			this.graphicPanel.ZoomFactor = ZoomFactor.Zoom1;
			this.tabControlSelectedCC = true;
			this.tabControl.SelectedIndex = 0;
			this.tabControlSelectedCC = false;
			this.ChangeLayer(LayerID.Foreground);
			this.UserMadeChanges(false);
			this.animationNumbericUpDownCC = true;
			this.animationNumericUpDown.Value = Constants.MinAnimationPhase;
			this.animationNumbericUpDownCC = false;
			this.lastPath = "";
			this.UpdateProperties();
			this.UpdateZoom();
			this.UpdateUndoRedoButtons();
			this.graphicPanel.Draw(this.GetGraphicLayer());
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

				this.urStack = new Dictionary<Tuple<int, int, int, LayerID, ZoomFactor>, UndoRedoStack>();
				this.actualGraphic = 0;
				this.actualAnimationPhase = 0;
				this.graphicPanel.ZoomFactor = ZoomFactor.Zoom1;
				this.tabControlSelectedCC = true;
				this.tabControl.SelectedIndex = 0;
				this.tabControlSelectedCC = false;
				this.alternativeCheckBoxCC = true;
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
				this.alternativeCheckBoxCC = false;
				this.animationNumbericUpDownCC = true;
				this.animationNumericUpDown.Value = Constants.MinAnimationPhase;
				this.animationNumbericUpDownCC = false;
				this.UpdateProperties();
				this.lastPath = this.zoom1Archive.FileName;
				this.UserMadeChanges(false);
				this.ChangeLayer(LayerID.Foreground);
				this.UpdateZoom();
				this.UpdateUndoRedoButtons();
				this.graphicPanel.Draw(this.GetGraphicLayer());
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
			catch (LayerIsEmptyException)
			{
				MessageBox.Show("A graphic is empty (fully transparent)!", "Invalid graphic!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
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

		private void ZoomInOut(bool zoomIn)
		{
			if (zoomIn)
			{
				this.graphicPanel.ZoomLevel += (int)this.graphicPanel.ZoomFactor;
			}
			else
			{
				this.graphicPanel.ZoomLevel -= (int)this.graphicPanel.ZoomFactor;
			}

			this.UpdateZoom();
		}

		#endregion Editor Methods

		#region Drawing

		private void DrawGraphic(Graphics g)
		{
			try
			{
				this.DrawSelection(g);
				this.DrawSpecial(g);
				this.DrawClock(g);
				// Grid for animations
				if (this.graphicPanel.DisplayGrid && this.zoom1Archive.Animation != null && this.zoom1Archive.Animation[this.actualGraphic, this.actualAlternative] != null)
				{
					AnimationProgram program = this.zoom1Archive.Animation[this.actualGraphic, this.actualAlternative];
					Point corner1 = new Point(program.XDiff + 1, 8 - (program.YDiff + 1));
					Point corner2 = new Point(program.XDiff + 1 + program.Width, (8 - (program.YDiff + 1)) - program.Height);

					corner1.X = (corner1.X * Constants.ElementWidth) * (this.graphicPanel.ZoomLevel);
					corner1.Y = (corner1.Y * Constants.ElementHeight) * (this.graphicPanel.ZoomLevel);
					corner2.X = (corner2.X * Constants.ElementWidth) * (this.graphicPanel.ZoomLevel);
					corner2.Y = (corner2.Y * Constants.ElementHeight) * (this.graphicPanel.ZoomLevel);

					Rectangle rectangle = new Rectangle(Math.Min(corner1.X, corner2.X),
														Math.Min(corner1.Y, corner2.Y),
														Math.Abs(corner1.X - corner2.X),
														Math.Abs(corner1.Y - corner2.Y));
					Pen pen = new Pen(Brushes.Red);
					float[] dashValues = { 3, 3 };
					pen.DashPattern = dashValues;
					g.DrawRectangle(pen, rectangle);
					pen.Dispose();
				}
			}
			catch (IndexOutOfRangeException)
			{
				MessageBox.Show("Internal Error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
						GraphicPanel.DrawElement(g, GraphicPreview(this.zoom1Archive[i]), 1, false, ZoomFactor.Zoom1);
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
						GraphicPanel.DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 1]), 1, false, ZoomFactor.Zoom1);
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
						GraphicPanel.DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 2]), 1, false, ZoomFactor.Zoom1);
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
						GraphicPanel.DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 3]), 1, false, ZoomFactor.Zoom1);
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
						GraphicPanel.DrawElement(g, GraphicPreview(this.zoom1Archive[this.actualGraphic, this.actualAnimationPhase, 4]), 1, false, ZoomFactor.Zoom1);
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

		private void DrawSpecial(Graphics g)
		{
			if (this.drawingSpecialMode)
			{
				if (this.rectangleToolStripRadioButton.Checked)
				{
					uint pixel = 0;
					if (this.specialModeMouseButton == MouseButtons.Left)
						pixel = this.leftPixel;
					else if (this.specialModeMouseButton == MouseButtons.Right)
						pixel = this.rightPixel;

					Brush brush;
					if (Pixel.IsTransparent(pixel))
						brush = new TextureBrush(GraphicPanel.GetBackgroundBitmapByZoomlevel(this.graphicPanel.ZoomLevel, this.graphicPanel.ZoomFactor));
					else if (Pixel.IsSpecial(pixel))
						brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), PixelToColor(pixel));
					else
						brush = new SolidBrush(PixelToColor(pixel));

					float elementSize = (this.graphicPanel.ZoomLevel) / (int)this.graphicPanel.ZoomFactor;
					Point corner1 = this.specialModeStartPoint;
					Point corner2 = this.specialModeEndPoint;

					corner1.Y = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - corner1.Y);
					corner2.Y = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - corner2.Y);
					corner1.X = (int)(corner1.X * elementSize);
					corner1.Y = (int)(corner1.Y * elementSize);
					corner2.X = (int)(corner2.X * elementSize);
					corner2.Y = (int)(corner2.Y * elementSize);

					RectangleF rectangle = new RectangleF(Math.Min(corner1.X, corner2.X),
														Math.Min(corner1.Y, corner2.Y) - elementSize,
														Math.Abs(corner1.X - corner2.X) + elementSize,
														Math.Abs(corner1.Y - corner2.Y) + elementSize);
					g.FillRectangle(brush, rectangle);
					brush.Dispose();
				}
				else if (this.lineToolStripRadioButton.Checked)
				{
					uint pixel = 0;
					if (this.specialModeMouseButton == MouseButtons.Left)
						pixel = this.leftPixel;
					else if (this.specialModeMouseButton == MouseButtons.Right)
						pixel = this.rightPixel;

					Brush brush;
					if (Pixel.IsTransparent(pixel))
						brush = new TextureBrush(GraphicPanel.GetBackgroundBitmapByZoomlevel(this.graphicPanel.ZoomLevel, this.graphicPanel.ZoomFactor));
					else if (Pixel.IsSpecial(pixel))
						brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), PixelToColor(pixel));
					else
						brush = new SolidBrush(PixelToColor(pixel));

					Point[] pixels = CalculateLine(this.specialModeStartPoint.X, this.specialModeStartPoint.Y, this.specialModeEndPoint.X, this.specialModeEndPoint.Y);

					foreach (Point item in pixels)
					{
						g.FillRectangle(brush,
							(item.X * graphicPanel.ZoomLevel) / (int)this.graphicPanel.ZoomFactor,
							((graphicPanel.ZoomLevel * Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) -
							(graphicPanel.ZoomLevel * (item.Y + 1))) / (int)this.graphicPanel.ZoomFactor,
							graphicPanel.ZoomLevel / (float)this.graphicPanel.ZoomFactor,
							graphicPanel.ZoomLevel / (float)this.graphicPanel.ZoomFactor);
					}
					brush.Dispose();
				}
			}
		}

		private void DrawSelection(Graphics g)
		{
			if (this.selected)
			{
				float elementSize = (this.graphicPanel.ZoomLevel) / (int)this.graphicPanel.ZoomFactor;
				Point corner1 = this.selectStartPoint;
				Point corner2 = this.selectEndPoint;

				corner1.Y = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - corner1.Y);
				corner2.Y = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - corner2.Y);
				corner1.X = (int)(corner1.X * elementSize);
				corner1.Y = (int)(corner1.Y * elementSize);
				corner2.X = (int)(corner2.X * elementSize);
				corner2.Y = (int)(corner2.Y * elementSize);

				Rectangle rectangle = new Rectangle(Math.Min(corner1.X, corner2.X),
													(Math.Min(corner1.Y, corner2.Y) - (int)elementSize),
													(Math.Abs(corner1.X - corner2.X) + (int)elementSize),
													(Math.Abs(corner1.Y - corner2.Y) + (int)elementSize));
				Pen pen = new Pen(Brushes.Black);
				float[] dashValues = { 3, 3 };
				pen.DashPattern = dashValues;
				g.DrawRectangle(pen, rectangle);
				pen.Dispose();
			}
		}

		private void DrawClock(Graphics g)
		{
			if (this.ActualGraphic != null && this.ActualGraphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				int xc = this.ActualGraphic.Properties.ClockX;
				int yc = this.ActualGraphic.Properties.ClockY;
				int wc = this.ActualGraphic.Properties.ClockWidth;

				Point corner1 = new Point(), corner2 = new Point();
				corner1.X = (xc - (wc / 2) + Constants.ElementWidth * (int)this.graphicPanel.ZoomFactor);
				corner2.X = (xc + ((wc - 1) / 2) + Constants.ElementWidth * (int)this.graphicPanel.ZoomFactor);
				corner1.Y = (yc - (wc / 2) + Constants.ElementHeight * (int)this.graphicPanel.ZoomFactor);
				corner2.Y = (yc + ((wc - 1) / 2) + Constants.ElementHeight * (int)this.graphicPanel.ZoomFactor);

				float elementSize = (this.graphicPanel.ZoomLevel) / (int)this.graphicPanel.ZoomFactor;
				corner1.Y = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - corner1.Y);
				corner2.Y = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - corner2.Y);
				corner1.X = (int)(corner1.X * elementSize);
				corner1.Y = (int)(corner1.Y * elementSize);
				corner2.X = (int)(corner2.X * elementSize);
				corner2.Y = (int)(corner2.Y * elementSize);

				Rectangle rectangle = new Rectangle(Math.Min(corner1.X, corner2.X),
													(Math.Min(corner1.Y, corner2.Y) - (int)elementSize),
													(Math.Abs(corner1.X - corner2.X) + (int)elementSize),
													(Math.Abs(corner1.Y - corner2.Y) + (int)elementSize));
				Pen pen = new Pen(Brushes.Blue);
				float[] dashValues = { 3, 3 };
				pen.DashPattern = dashValues;
				g.DrawRectangle(pen, rectangle);
				pen.Dispose();
			}
		}

		private static uint[,] GraphicPreview(Graphic graphic)
		{
			uint[,] result = new uint[Constants.ElementHeight, Constants.ElementWidth];
			for (int i = 0; i < result.GetLength(0); i++)
			{
				for (int j = 0; j < result.GetLength(1); j++)
				{
					result[i, j] = Constants.ColorTransparent;
				}
			}
			LayerID[] layerIDs = { LayerID.ForegroundAbove, LayerID.Foreground, LayerID.Front, LayerID.Background1, LayerID.Background0 };

			foreach (LayerID layer in layerIDs)
			{
				if (!graphic.IsTransparent(layer))
				{
					uint[,] layerElement = graphic[layer];
					for (int i = 0; i < result.GetLength(0); i++)
					{
						for (int j = 0; j < result.GetLength(1); j++)
						{
							if (!Pixel.IsTransparent(layerElement[i + Constants.ElementHeight, j + Constants.ElementWidth]) && Pixel.IsTransparent(result[i, j]))
							{
								result[i, j] = layerElement[i + Constants.ElementHeight, j + Constants.ElementWidth];
							}
						}
					}
				}
			}

			return result;
		}

		#endregion Drawing

		#region Mouse-events

		private void MouseDownOnGraphic(MouseEventArgs e)
		{
			this.isMouseCaptured = true;
			if (!e.Button.HasFlag(MouseButtons.Left) && !e.Button.HasFlag(MouseButtons.Right))
				return;

			Point p = this.TransformCoordinates(e.X, e.Y);
			this.CreateGraphic();
			uint[,] layer = this.GetGraphicLayer();
			if (p.X >= 0 && p.Y >= 0 && p.X < layer.GetLength(1) && p.Y < layer.GetLength(0))
			{
				if (this.lineToolStripRadioButton.Checked || this.rectangleToolStripRadioButton.Checked)
				{
					this.specialModeStartPoint = p;
					this.specialModeMouseButton = e.Button;
					this.drawingSpecialMode = true;
				}
				else if (this.selectToolStripRadioButton.Checked)
				{
					this.selectStartPoint = p;
					this.selected = true;
					this.drawingSelection = true;
				}
				else
				{
					try
					{
						if (this.normalModeToolStripRadioButton.Checked)
						{
							this.changes = new List<Point>();
							uint pixel;
							if (e.Button == MouseButtons.Left && !layer[p.Y, p.X].Equals(leftPixel))
							{
								pixel = leftPixel;
							}
							else if (e.Button == MouseButtons.Right && !layer[p.Y, p.X].Equals(rightPixel))
							{
								pixel = rightPixel;
							}
							else
								return;

							this.changes.Add(p);
							this.UserMadeChanges(true);
							this.graphicPanel.Draw(new Point[] { new Point(p.X, p.Y) }, pixel);
						}
						else if (this.pickColorToolStripRadioButton.Checked)
						{
							uint el = layer[p.Y, p.X];
							Color c = PixelToColor(el);
							if (e.Button == MouseButtons.Left && !el.Equals(leftPixel))
							{
								leftPixel = el;
								leftColorButton.BackColor = c;
								leftComboBoxCC = true;
								leftComboBox.SelectedIndex = PixelToComboBoxIndex(el);
								leftComboBoxCC = false;
								this.leftLabel.Text = String.Format("Left: {0} - {1} - {2}", c.R, c.G, c.B);
							}
							else if (e.Button == MouseButtons.Right && !el.Equals(rightPixel))
							{
								rightPixel = el;
								rightColorButton.BackColor = c;
								rightComboBoxCC = true;
								rightComboBox.SelectedIndex = PixelToComboBoxIndex(el);
								rightComboBoxCC = false;
								this.rightLabel.Text = String.Format("Right: {0} - {1} - {2}", c.R, c.G, c.B);
							}
						}
						else if (this.fillToolStripRadioButton.Checked)
						{
							uint oldColor = layer[p.Y, p.X];
							uint newColor = 0;
							if (e.Button == MouseButtons.Left)
								newColor = this.leftPixel;
							else if (e.Button == MouseButtons.Right)
								newColor = this.rightPixel;

							if (newColor == oldColor)
								return;
							int x = p.X;
							int y = p.Y;
							List<Point> changes = new List<Point>();

							Stack<Point> stack = new Stack<Point>();
							stack.Push(new Point(x, y));
							while (stack.Count > 0)
							{
								Point point = stack.Pop();
								if (point.X >= 0 && point.Y >= 0 && point.X < layer.GetLength(1) && point.Y < layer.GetLength(0))
								{
									if (layer[point.Y, point.X] == oldColor)
									{
										changes.Add(point);
										stack.Push(new Point(point.X, point.Y + 1));
										stack.Push(new Point(point.X, point.Y - 1));
										stack.Push(new Point(point.X + 1, point.Y));
										stack.Push(new Point(point.X - 1, point.Y));
									}
								}
							}
							this.UndoRedo.Do(new ChangePixelsCommand(newColor, changes.ToArray()), layer);
							this.UserMadeChanges(true);
							this.SetGraphicLayer(layer);
							this.UpdateUndoRedoButtons();
							this.graphicPanel.Draw(layer);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(String.Format("Internal Error!{0}{1}", Environment.NewLine, ex.ToString()), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void MouseMoveOnGraphic(MouseEventArgs e)
		{
			if ((!e.Button.HasFlag(MouseButtons.Left) && !e.Button.HasFlag(MouseButtons.Right)) || !this.isMouseCaptured)
				return;

			Point p = this.TransformCoordinates(e.X, e.Y);
			uint[,] layer = this.GetGraphicLayer();

			if (p.X >= 0 && p.Y >= 0 && p.X < layer.GetLength(1) && p.Y < layer.GetLength(0))
			{
				if (this.drawingSpecialMode && (this.lineToolStripRadioButton.Checked || this.rectangleToolStripRadioButton.Checked))
				{
					if (this.specialModeMouseButton != e.Button)
					{
						this.drawingSpecialMode = false;
					}
					else
					{
						if (this.specialModeEndPoint != p)
						{
							this.specialModeEndPoint = p;
						}
						else
							return;
					}
					this.graphicPanel.Invalidate();
				}
				else if (this.selected && this.drawingSelection && this.selectToolStripRadioButton.Checked)
				{
					if (this.selectEndPoint != p)
					{
						this.selectEndPoint = p;
						this.graphicPanel.Invalidate();
					}
				}
				else if (this.normalModeToolStripRadioButton.Checked)
				{
					if (!this.changes.Contains(p))
					{
						uint pixel;
						if (e.Button == MouseButtons.Left && !layer[p.Y, p.X].Equals(leftPixel))
						{
							pixel = leftPixel;
						}
						else if (e.Button == MouseButtons.Right && !layer[p.Y, p.X].Equals(rightPixel))
						{
							pixel = rightPixel;
						}
						else
							return;

						this.changes.Add(p);
						this.graphicPanel.Draw(new Point[] { new Point(p.X, p.Y) }, pixel);
					}
				}
			}
		}

		private void MouseUpOnGraphic(MouseEventArgs e)
		{
			if ((!e.Button.HasFlag(MouseButtons.Left) && !e.Button.HasFlag(MouseButtons.Right)) || !this.isMouseCaptured)
				return;

			this.isMouseCaptured = false;
			if (this.drawingSpecialMode && (this.lineToolStripRadioButton.Checked || this.rectangleToolStripRadioButton.Checked))
			{
				this.drawingSpecialMode = false;
				this.PrintSpecialOnGraphic();
			}
			else if (this.selected && this.drawingSelection)
			{
				this.drawingSelection = false;
				this.graphicPanel.Invalidate();
			}
			else if (this.normalModeToolStripRadioButton.Checked)
			{
				uint pixel;
				if (e.Button == MouseButtons.Left)
				{
					pixel = leftPixel;
				}
				else if (e.Button == MouseButtons.Right)
				{
					pixel = rightPixel;
				}
				else
					return;

				uint[,] layer = this.GetGraphicLayer();
				layer = this.UndoRedo.Do(new ChangePixelsCommand(pixel, this.changes.ToArray()), layer);
				this.changes = null;
				this.SetGraphicLayer(layer);
				this.UpdateUndoRedoButtons();
				this.graphicPanel.Draw(layer);
			}
			this.overviewPanel.Invalidate();
		}

		private void MouseClickOnOverview(MouseEventArgs e)
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
				this.animationNumbericUpDownCC = true;
				this.animationNumericUpDown.Value = Constants.MinAnimationPhase;
				this.animationNumbericUpDownCC = false;

				this.alternativeCheckBoxCC = true;
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
				this.alternativeCheckBoxCC = false;

				this.UpdateProperties();
				this.UpdateUndoRedoButtons();
				this.graphicPanel.Draw(this.GetGraphicLayer());
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
					this.graphicPanel.Draw(this.GetGraphicLayer());
					this.UpdateAnimation();
				}
			}
		}

		#endregion Mouse-events

		#region Graphic Methods

		private void CreateGraphic()
		{
			if (this.graphicPanel.ZoomFactor == ZoomFactor.Zoom1)
			{
				if (this.ActualZoom1Graphic == null)
				{
					Graphic graphic = new Graphic("No text");
					this.zoom1Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, graphic);
					this.UserMadeChanges(true);
					ChangePropertyComboBoxes(true);
					this.overviewPanel.Invalidate();
				}
			}
			else if (this.graphicPanel.ZoomFactor == ZoomFactor.Zoom2)
			{
				if (this.ActualZoom2Graphic == null)
				{
					Graphic graphic = new Graphic("No text", zoomFactor: ZoomFactor.Zoom2);
					this.zoom2Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, graphic);
					this.UserMadeChanges(true);
					ChangePropertyComboBoxes(true);
					this.overviewPanel.Invalidate();
				}
			}
			else if (this.graphicPanel.ZoomFactor == ZoomFactor.Zoom4)
			{
				if (this.ActualZoom4Graphic == null)
				{
					Graphic graphic = new Graphic("No text", zoomFactor: ZoomFactor.Zoom4);
					this.zoom4Archive.AddGraphic(this.actualGraphic, this.actualAnimationPhase, this.actualAlternative, graphic);
					this.UserMadeChanges(true);
					ChangePropertyComboBoxes(true);
					this.overviewPanel.Invalidate();
				}
			}
		}

		private uint[,] GetGraphicLayer()
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null)
			{
				return graphic[this.actualLayer];
			}
			return null;
		}

		private void SetGraphicLayer(uint[,] layer)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null)
				graphic[this.actualLayer] = layer;
		}

		private void PrintSpecialOnGraphic()
		{
			if (this.rectangleToolStripRadioButton.Checked)
			{
				uint pixel = 0;
				if (this.specialModeMouseButton == MouseButtons.Left)
					pixel = this.leftPixel;
				else if (this.specialModeMouseButton == MouseButtons.Right)
					pixel = this.rightPixel;
				this.CreateGraphic();
				List<Point> changes = new List<Point>();
				uint[,] layer = this.GetGraphicLayer();
				int startX = Math.Min(this.specialModeStartPoint.X, this.specialModeEndPoint.X);
				int startY = Math.Min(this.specialModeStartPoint.Y, this.specialModeEndPoint.Y);
				int width = Math.Abs(this.specialModeStartPoint.X - this.specialModeEndPoint.X) + 1;
				int height = Math.Abs(this.specialModeStartPoint.Y - this.specialModeEndPoint.Y) + 1;
				for (int i = 0; i < width && i + startX < layer.GetLength(1); i++)
				{
					for (int j = 0; j < height && j + startY < layer.GetLength(0); j++)
					{
						changes.Add(new Point(i + startX, j + startY));
					}
				}
				layer = this.UndoRedo.Do(new ChangePixelsCommand(pixel, changes.ToArray()), layer);
				this.SetGraphicLayer(layer);
				this.UpdateUndoRedoButtons();
				this.UserMadeChanges(true);
				this.graphicPanel.Draw(layer);
			}
			else if (this.lineToolStripRadioButton.Checked)
			{
				uint pixel = 0;
				if (this.specialModeMouseButton == MouseButtons.Left)
					pixel = this.leftPixel;
				else if (this.specialModeMouseButton == MouseButtons.Right)
					pixel = this.rightPixel;
				this.CreateGraphic();
				uint[,] layer = this.GetGraphicLayer();
				Point[] pixels = CalculateLine(this.specialModeStartPoint.X, this.specialModeStartPoint.Y, this.specialModeEndPoint.X, this.specialModeEndPoint.Y);
				layer = this.UndoRedo.Do(new ChangePixelsCommand(pixel, pixels), layer);
				this.SetGraphicLayer(layer);
				this.UpdateUndoRedoButtons();
				this.UserMadeChanges(true);
				this.graphicPanel.Draw(layer);
			}
		}

		#endregion Graphic Methods

		#region Update Methods

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
				this.cursorNormalDirectionComboBoxCC = true;
				this.cursorReverseDirectionComboBoxCC = true;
				this.cursorNormalDirectionComboBox.SelectedIndex = (int)z1Graphic.Properties.CursorNormalDirection + 1;
				this.cursorReverseDirectionComboBox.SelectedIndex = (int)z1Graphic.Properties.CursorReverseDirection + 1;
				this.cursorNormalDirectionComboBoxCC = false;
				this.cursorReverseDirectionComboBoxCC = false;
			}
			else if (this.cursorNormalDirectionComboBox.SelectedIndex > -1 || this.cursorReverseDirectionComboBox.SelectedIndex > -1)
			{
				this.cursorNormalDirectionComboBoxCC = true;
				this.cursorReverseDirectionComboBoxCC = true;
				this.cursorNormalDirectionComboBox.SelectedIndex = -1;
				this.cursorReverseDirectionComboBox.SelectedIndex = -1;
				this.cursorNormalDirectionComboBoxCC = false;
				this.cursorReverseDirectionComboBoxCC = false;
			}
		}

		private void UpdateAnimation()
		{
			if (this.animationForm != null && !this.animationForm.IsDisposed)
				this.animationForm.ChangeAnimationProgram();
		}

		private void UpdateZoom()
		{
			if (this.graphicPanel.ZoomLevel > 7 * (int)this.graphicPanel.ZoomFactor)
			{
				this.zoomInButton.Enabled = false;
			}
			else
			{
				this.zoomInButton.Enabled = true;
			}
			if (this.graphicPanel.ZoomLevel < 1 * (int)this.graphicPanel.ZoomFactor + 1)
			{
				this.zoomOutButton.Enabled = false;
			}
			else
			{
				this.zoomOutButton.Enabled = true;
			}
			this.zoomLevelStatusLabel.Text = String.Format("Zoomlevel: {0}  Zoomfactor: {1}", this.graphicPanel.ZoomLevel, this.graphicPanel.ZoomFactor.ToString());
			this.graphicPanel.Draw(this.GetGraphicLayer());
		}

		private void UpdateUndoRedoButtons()
		{
			if (this.UndoRedoExists)
			{
				this.undoToolStripButton.Enabled = false;
				this.undoToolStripMenuItem.Enabled = false;
				this.redoToolStripButton.Enabled = false;
				this.redoToolStripMenuItem.Enabled = false;
			}
			if (this.UndoRedo.UndoCount <= 0)
			{
				this.undoToolStripButton.Enabled = false;
				this.undoToolStripMenuItem.Enabled = false;
			}
			else
			{
				this.undoToolStripButton.Enabled = true;
				this.undoToolStripMenuItem.Enabled = true;
			}
			if (this.UndoRedo.RedoCount <= 0)
			{
				this.redoToolStripButton.Enabled = false;
				this.redoToolStripMenuItem.Enabled = false;
			}
			else
			{
				this.redoToolStripButton.Enabled = true;
				this.redoToolStripMenuItem.Enabled = true;
			}
		}

		#endregion Update Methods

		#region Layer Methods

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

		private void SelectLayer()
		{
			if (this.layerSelectBoxCC)
			{
				this.layerSelectBoxCC = false;
			}
			else
			{
				this.actualLayer = this.GetLayerIDBySelectedIndex();
				this.UpdateUndoRedoButtons();
				this.graphicPanel.Draw(this.GetGraphicLayer());
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

		#endregion Layer Methods

		#region Pixel Methods

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

		private static int PixelToComboBoxIndex(uint pixel)
		{
			if (Pixel.IsTransparent(pixel))
				return 1;
			else if (Pixel.UsesRgb(pixel) && !Pixel.IsSpecial(pixel))
				return 0;
			else if (Pixel.UsesRgb(pixel) && Pixel.IsSpecial(pixel))
			{
				switch (Pixel.GetProperty(pixel))
				{
					case Pixel.PixelProperty.AlwaysBright:
						return 3;

					case Pixel.PixelProperty.LampYellow:
						return 4;

					case Pixel.PixelProperty.LampColdWhite:
						return 5;

					case Pixel.PixelProperty.LampRed:
						return 6;

					case Pixel.PixelProperty.LampYellowWhite:
						return 7;

					case Pixel.PixelProperty.LampGasYellow:
						return 8;

					case Pixel.PixelProperty.WindowYellow0:
						return 9;

					case Pixel.PixelProperty.WindowYellow1:
						return 10;

					case Pixel.PixelProperty.WindowYellow2:
						return 11;

					case Pixel.PixelProperty.WindowNeon0:
						return 12;

					case Pixel.PixelProperty.WindowNeon1:
						return 13;

					case Pixel.PixelProperty.WindowNeon2:
						return 14;

					default:
						MessageBox.Show("Unknown pixelproperty (uses rgb)");
						break;
				}
			}
			else if (Pixel.IsSpecial(pixel) && !Pixel.UsesRgb(pixel))
			{
				switch (pixel)
				{
					case (uint)Pixel.PixelProperty.BehindGlass:
						return 2;

					case (uint)Pixel.PixelProperty.AsBG:
						return 15;

					case (uint)Pixel.PixelProperty.AsSleepers0:
						return 16;

					case (uint)Pixel.PixelProperty.AsSleepers1:
						return 17;

					case (uint)Pixel.PixelProperty.AsSleepers3:
						return 18;

					case (uint)Pixel.PixelProperty.AsRailsRoad0:
						return 19;

					case (uint)Pixel.PixelProperty.AsRailsRoad1:
						return 20;

					case (uint)Pixel.PixelProperty.AsRailsRoad2:
						return 21;

					case (uint)Pixel.PixelProperty.AsRailsRoad3:
						return 22;

					case (uint)Pixel.PixelProperty.AsRailsTrackbed0:
						return 23;

					case (uint)Pixel.PixelProperty.AsRailsTrackbed1:
						return 24;

					case (uint)Pixel.PixelProperty.AsRailsTrackbed2:
						return 25;

					case (uint)Pixel.PixelProperty.AsRailsTrackbed3:
						return 26;

					case (uint)Pixel.PixelProperty.AsMarkingPointBus0:
						return 27;

					case (uint)Pixel.PixelProperty.AsMarkingPointBus1:
						return 28;

					case (uint)Pixel.PixelProperty.AsMarkingPointBus2:
						return 29;

					case (uint)Pixel.PixelProperty.AsMarkingPointBus3:
						return 30;

					case (uint)Pixel.PixelProperty.AsMarkingPointWater:
						return 31;

					case (uint)Pixel.PixelProperty.AsGravel:
						return 32;

					case (uint)Pixel.PixelProperty.AsSmallGravel:
						return 33;

					case (uint)Pixel.PixelProperty.AsGrassy:
						return 34;

					case (uint)Pixel.PixelProperty.AsPathBG:
						return 35;

					case (uint)Pixel.PixelProperty.AsPathFG:
						return 36;

					case (uint)Pixel.PixelProperty.AsText:
						return 37;

					default:
						MessageBox.Show("Unknown pixelproperty (!uses rgb)");
						break;
				}
			}
			return 0;
		}

		private static uint PixelFromComboBoxIndex(int index, uint lastPixel)
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

		#endregion Pixel Methods

		#region Helper

		private void ChangePropertyComboBoxes(bool enabled)
		{
			this.particleComboBox.Enabled = enabled;
			this.clockComboBox.Enabled = enabled;
			this.cursorNormalDirectionComboBox.Enabled = enabled;
			this.cursorReverseDirectionComboBox.Enabled = enabled;
		}

		private Point TransformCoordinates(int x, int y)
		{
			int xElement = (int)((x / (float)this.graphicPanel.ZoomLevel) * (int)this.graphicPanel.ZoomFactor);
			int yElement = (int)((y / (float)this.graphicPanel.ZoomLevel) * (int)this.graphicPanel.ZoomFactor);
			yElement = ((Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor) - yElement) - 1;
			return new Point(xElement, yElement);
		}

		private static int GetSignOfInteger(int x)
		{
			return (x > 0) ? 1 : (x < 0) ? -1 : 0;
		}

		private static Point[] CalculateLine(int startX, int startY, int endX, int endY)
		{
			int dx = endX - startX;
			int dy = endY - startY;

			int incX = GetSignOfInteger(dx);
			int incY = GetSignOfInteger(dy);
			if (dx < 0) dx = -dx;
			if (dy < 0) dy = -dy;

			int pdx, pdy, ddx, ddy, es, el;
			if (dx > dy)
			{
				/* x ist schnelle Richtung */
				pdx = incX; pdy = 0;    /* pd. ist Parallelschritt */
				ddx = incX; ddy = incY; /* dd. ist Diagonalschritt */
				es = dy; el = dx;   /* Fehlerschritte schnell, langsam */
			}
			else
			{
				/* y ist schnelle Richtung */
				pdx = 0; pdy = incY; /* pd. ist Parallelschritt */
				ddx = incX; ddy = incY; /* dd. ist Diagonalschritt */
				es = dx; el = dy;   /* Fehlerschritte schnell, langsam */
			}

			int x = startX, y = startY;
			int err = el / 2;
			List<Point> pixels = new List<Point>();
			pixels.Add(new Point(x, y));

			for (int i = 0; i < el; ++i)
			{
				err -= es;
				if (err < 0)
				{
					err += el;
					x += ddx;
					y += ddy;
				}
				else
				{
					x += pdx;
					y += pdy;
				}
				pixels.Add(new Point(x, y));
			}

			return pixels.ToArray();
		}

		#endregion Helper

		#region Copy and Paste

		private void CopyGraphic(bool cut)
		{
			uint[,] layer = this.GetGraphicLayer();
			if (this.selected && layer != null)
			{
				StringBuilder builder = new StringBuilder();

				int startX = Math.Min(selectStartPoint.X, selectEndPoint.X);
				int startY = Math.Min(selectStartPoint.Y, selectEndPoint.Y);
				int height = Math.Abs(selectStartPoint.Y - selectEndPoint.Y + 1);
				int width = Math.Abs(selectStartPoint.X - selectEndPoint.X - 1);

				List<Point> changes = new List<Point>();

				builder.Append(String.Format("bahneditor copy {0} {1} {2} {3} ", "1", this.graphicPanel.ZoomFactor, width, height));
				int counter = 0;
				uint last = layer[startY, startX];

				for (int i = startY; i < startY + height && i < layer.GetLength(0); i++) //Y - height
				{
					for (int j = startX; j < startX + width && j < layer.GetLength(1); j++) //X - width
					{
						if (last != layer[i, j])
						{
							builder.Append(String.Format("{0}-{1} ", last, counter));
							last = layer[i, j];
							counter = 1;
						}
						else
							counter++;
						if (cut)
							changes.Add(new Point(j, i));
					}
				}
				builder.Append(String.Format("{0}-{1}", last, counter));

				Clipboard.SetText(builder.ToString());
				if (cut)
				{
					layer = this.UndoRedo.Do(new ChangePixelsCommand(Constants.ColorTransparent, changes.ToArray()), layer);
					this.SetGraphicLayer(layer);
					this.UpdateUndoRedoButtons();
					this.UserMadeChanges(true);
					this.graphicPanel.Draw(layer);
				}
			}
		}

		private void CopyGraphicToBitmap(int multiplier)
		{
			uint[,] graphic = this.GetGraphicLayer();
			if (this.selected && graphic != null)
			{
				int startX = Math.Min(selectStartPoint.X, selectEndPoint.X);
				int startY = Math.Min(selectStartPoint.Y, selectEndPoint.Y);
				int height = Math.Abs(selectStartPoint.Y - selectEndPoint.Y + 1);
				int width = Math.Abs(selectStartPoint.X - selectEndPoint.X - 1);

				Bitmap copy = new Bitmap(width, height, PixelFormat.Format24bppRgb);
				Rectangle rect = new Rectangle(0, 0, width, height);
				BitmapData bmpData = copy.LockBits(rect, ImageLockMode.WriteOnly, copy.PixelFormat);
				IntPtr ptr = bmpData.Scan0;

				int bytes = Math.Abs(bmpData.Stride) * copy.Height;
				byte[] rgb = new byte[bytes];
				int counter = 0;

				System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, bytes);

				for (int i = startY + height - 1; i >= 0 && i >= startY; i--) //Y - height
				{
					for (int j = startX; j < graphic.GetLength(1) && j < startX + width; j++) //X - width
					{
						uint pixel = graphic[i, j];
						Color c = PixelToColor(pixel);
						rgb[counter++] = c.R;
						rgb[counter++] = c.G;
						rgb[counter++] = c.B;
					}
				}

				System.Runtime.InteropServices.Marshal.Copy(rgb, 0, ptr, bytes);

				copy.UnlockBits(bmpData);

				Clipboard.SetImage(copy);
			}
		}

		private void PasteGraphic()
		{
			this.CreateGraphic();
			if (Clipboard.ContainsImage())
			{
			}
			else if (Clipboard.ContainsText())
			{
				uint[,] layer = this.GetGraphicLayer();
				string[] array = Clipboard.GetText().Split(' ');
				if (array[0] == "bahneditor" && array[1] == "copy")
				{
					//ZoomFactor zoomFactor;
					int width;
					int height;
					int startX = Math.Min(selectStartPoint.X, selectEndPoint.X); //X - width
					int startY = Math.Min(selectStartPoint.Y, selectEndPoint.Y); //Y - height
					if (array[2] != "1")
					{
						MessageBox.Show("An error happened during pasting!" + Environment.NewLine + "Wrong version!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					if (!int.TryParse(array[4], out width)) return;
					if (!int.TryParse(array[5], out height)) return;

					List<uint> list = new List<uint>();

					for (int i = 6; i < array.Length; i++)
					{
						string[] split = array[i].Split('-');
						uint pixel;
						int count;
						if (!uint.TryParse(split[0], out pixel) || !int.TryParse(split[1], out count))
						{
							MessageBox.Show("An error happened during pasting!" + Environment.NewLine + "Data was in wrong format!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}
						for (int j = 0; j < count; j++)
						{
							list.Add(pixel);
						}
					}
					int counter = 0;
					for (int y = 0; y < height && y + startY < layer.GetLength(0); y++)
					{
						for (int x = 0; x < width && x + startX < layer.GetLength(1); x++)
						{
							layer[y + startY, x + startX] = list[counter];
							counter++;
							if (x + startX >= layer.GetLength(1) - 1)
							{
								counter += width - x - 1;
							}
						}
						if (y + startY >= layer.GetLength(0) - 1)
						{
							counter += height - y - 1;
						}
					}
					this.SetGraphicLayer(layer);
					this.graphicPanel.Draw(layer);
				}
			}
		}

		#endregion Copy and Paste

		#region Undo and Redo

		private void Undo()
		{
			uint[,] layer = this.GetGraphicLayer();
			this.UndoRedo.Undo(layer);
			this.SetGraphicLayer(layer);
			this.UpdateUndoRedoButtons();
			this.UserMadeChanges(true);
			this.graphicPanel.Draw(layer);
		}

		private void Redo()
		{
			uint[,] layer = this.GetGraphicLayer();
			this.UndoRedo.Redo(layer);
			this.SetGraphicLayer(layer);
			this.UpdateUndoRedoButtons();
			this.UserMadeChanges(true);
			this.graphicPanel.Draw(layer);
		}

		#endregion Undo and Redo

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
			this.graphicPanel.Draw(this.GetGraphicLayer());
		}

		internal void ResetAnimationNumericUpDown()
		{
			this.actualAnimationPhase = 0;
			this.animationNumbericUpDownCC = true;
			this.animationNumericUpDown.Value = 0;
			this.animationNumbericUpDownCC = false;
			this.UpdateUndoRedoButtons();
		}

		internal void UserMadeChanges(bool userMadeChanges)
		{
			this.userMadeChanges = userMadeChanges;
			this.Text = userMadeChanges ? "Bahn Editor *" : "Bahn Editor";
		}

		#endregion Internal Methods

		#region Event-Handler

		#region Editor

		private void Editor_Load(object sender, EventArgs e)
		{
			this.EditorLoaded();
		}

		private void Editor_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !this.AskSaveGraphic();
		}

		private void Editor_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape && this.selected)
			{
				this.selected = false;
				this.selectEndPoint = new Point();
				this.selectStartPoint = new Point();
				this.graphicPanel.Invalidate();
			}
			else
			{
				switch (e.KeyData)
				{
					case Keys.D1:
						this.normalModeToolStripRadioButton.Checked = true;
						break;

					case Keys.D2:
						this.lineToolStripRadioButton.Checked = true;
						break;

					case Keys.D3:
						this.rectangleToolStripRadioButton.Checked = true;
						break;

					case Keys.D4:
						this.fillToolStripRadioButton.Checked = true;
						break;

					case Keys.D5:
						this.selectToolStripRadioButton.Checked = true;
						break;

					case Keys.D6:
						this.pickColorToolStripRadioButton.Checked = true;
						break;

					default:
						break;
				}
			}
		}

		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if (tabControlSelectedCC)
				return;

			switch (e.TabPage.Name)
			{
				case "zoom1Tab":
					this.graphicPanel.ZoomFactor = ZoomFactor.Zoom1;
					break;

				case "zoom2Tab":
					this.graphicPanel.ZoomFactor = ZoomFactor.Zoom2;
					break;

				case "zoom4Tab":
					this.graphicPanel.ZoomFactor = ZoomFactor.Zoom4;
					break;

				default:
					throw new Exception("Internal Error!");
			}
			this.UpdateProperties();
			this.UpdateZoom();
			this.UpdateUndoRedoButtons();
			this.graphicPanel.Draw(this.GetGraphicLayer());
		}

		private void loadFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.LoadGraphicArchive();
		}

		private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			this.lastPath = this.saveFileDialog.FileName;
			if (!this.SaveGraphicArchive())
			{
				e.Cancel = true;
			}
		}

		#endregion Editor

		#region Overview

		private void overviewPanel_Paint(object sender, PaintEventArgs e)
		{
			this.DrawOverview(e.Graphics);
		}

		private void overviewPanel_Click(object sender, EventArgs e)
		{
			MouseEventArgs me = e as MouseEventArgs;
			if (me != null)
				MouseClickOnOverview(me);
			else
				MessageBox.Show("Internal Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void overviewUpButton_Click(object sender, EventArgs e)
		{
			if (this.overviewLine < 4)
			{
				this.overviewLine++;
				this.overviewPanel.Invalidate();
			}
			this.overviewPanel.Focus();
		}

		private void overviewDownButton_Click(object sender, EventArgs e)
		{
			if (this.overviewLine > 0)
			{
				this.overviewLine--;
				this.overviewPanel.Invalidate();
			}
			overviewPanel.Focus();
		}

		private void overviewLeftRightButton_Click(object sender, EventArgs e)
		{
			if (this.overviewAlternative == 1)
				this.overviewAlternative = 0;
			else
				this.overviewAlternative = 1;
			this.overviewPanel.Invalidate();
			overviewPanel.Focus();
		}

		private void animationNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			if (this.animationNumbericUpDownCC)
				return;
			this.actualAnimationPhase = (int)this.animationNumericUpDown.Value;
			this.UpdateUndoRedoButtons();
			this.UpdateProperties();
			this.graphicPanel.Draw(this.GetGraphicLayer());
		}

		private void alternativesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (alternativeCheckBoxCC)
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
							this.alternativeCheckBoxCC = true;
							this.alternativesCheckBox.Checked = true;
							this.alternativeCheckBoxCC = false;
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
			this.UpdateUndoRedoButtons();
			this.overviewPanel.Invalidate();
			this.graphicPanel.Draw(this.GetGraphicLayer());
			this.UpdateAnimation();
		}

		#endregion Overview

		#region GraphicPanel

		private void graphicPanel_MouseDown(object sender, MouseEventArgs e)
		{
			this.MouseDownOnGraphic(e);
		}

		private void graphicPanel_MouseMove(object sender, MouseEventArgs e)
		{
			this.MouseMoveOnGraphic(e);
		}

		private void graphicPanel_MouseUp(object sender, MouseEventArgs e)
		{
			this.MouseUpOnGraphic(e);
		}

		private void graphicPanel_Paint(object sender, PaintEventArgs e)
		{
			this.DrawGraphic(e.Graphics);
		}

		private void gridCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.graphicPanel.DisplayGrid = this.gridCheckBox.Checked;
			this.graphicPanel.Draw(this.GetGraphicLayer());
		}

		#endregion GraphicPanel

		#region Properties

		private void propertiesGroupBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawLine(Pens.DarkGray, 1, 76, 198, 76);
			e.Graphics.DrawLine(Pens.DarkGray, 1, 242, 198, 242);
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

		private void clockXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				graphic.Properties.ClockX = (int)clockXNumericUpDown.Value;
				this.UserMadeChanges(true);
				this.graphicPanel.Invalidate();
			}
		}

		private void clockYNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.RawData.HasFlag(GraphicProperties.Properties.Clock))
			{
				graphic.Properties.ClockY = (int)clockYNumericUpDown.Value;
				this.UserMadeChanges(true);
				this.graphicPanel.Invalidate();
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
				this.graphicPanel.Invalidate();
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
						graphic.Properties.ClockProperties &= ~(ClockProperties.RotatedNorthEast | ClockProperties.RotatedNorthWest);
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

		private void cursorNormalDirectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cursorNormalDirectionComboBoxCC)
			{
				return;
			}
			Graphic graphic = this.ActualZoom1Graphic;

			if (graphic != null)
			{
				if (cursorNormalDirectionComboBox.SelectedIndex == 0)
				{
					graphic.Properties.RawData &= ~GraphicProperties.Properties.Cursor;
					this.cursorReverseDirectionComboBoxCC = true;
					this.cursorReverseDirectionComboBox.SelectedIndex = 0;
					this.cursorReverseDirectionComboBoxCC = false;
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
			if (this.cursorReverseDirectionComboBoxCC)
			{
				return;
			}
			Graphic graphic = this.ActualZoom1Graphic;

			if (graphic != null)
			{
				if (cursorReverseDirectionComboBox.SelectedIndex == 0)
				{
					graphic.Properties.RawData &= ~GraphicProperties.Properties.Cursor;
					this.cursorNormalDirectionComboBoxCC = true;
					this.cursorNormalDirectionComboBox.SelectedIndex = 0;
					this.cursorNormalDirectionComboBoxCC = false;
				}
				else
				{
					graphic.Properties.RawData |= GraphicProperties.Properties.Cursor;
					graphic.Properties.CursorReverseDirection = (Direction)(cursorReverseDirectionComboBox.SelectedIndex - 1);
				}
				this.UserMadeChanges(true);
			}
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

		private void particleWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			Graphic graphic = this.ActualGraphic;
			if (graphic != null && graphic.Properties.HasParticles)
			{
				graphic.Properties.ParticleWidth = (int)particleWidthNumericUpDown.Value;
				this.UserMadeChanges(true);
			}
		}

		#endregion Properties

		#region Color and layer

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

		private void frontRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SelectLayer();
		}

		private void leftColorButton_Click(object sender, EventArgs e)
		{
			this.colorDialog.Color = PixelToColor(this.leftPixel);
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
					this.leftComboBoxCC = true;
					this.leftComboBox.SelectedIndex = 0;
					this.leftComboBoxCC = false;
					this.leftPixel = PixelFromColor(c);
				}
				this.leftColorButton.BackColor = c;
				this.leftLabel.Text = String.Format("Left: {0} - {1} - {2}", c.R, c.G, c.B);
			}
		}

		private void rightColorButton_Click(object sender, EventArgs e)
		{
			this.colorDialog.Color = PixelToColor(this.rightPixel);
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
					this.rightComboBoxCC = true;
					this.rightComboBox.SelectedIndex = 0;
					this.rightComboBoxCC = false;
					this.rightPixel = PixelFromColor(c);
				}
				this.rightColorButton.BackColor = c;
				this.rightLabel.Text = String.Format("Right: {0} - {1} - {2}", c.R, c.G, c.B);
			}
		}

		private void leftComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.leftComboBoxCC)
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
				uint pixel = PixelFromComboBoxIndex(this.leftComboBox.SelectedIndex, lastPixel);
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
			Color color = PixelToColor(this.leftPixel);
			this.leftColorButton.BackColor = color;
			this.leftLabel.Text = String.Format("Left: {0} - {1} - {2}", color.R, color.G, color.B);
		}

		private void rightComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.rightComboBoxCC)
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
				uint pixel = PixelFromComboBoxIndex(this.rightComboBox.SelectedIndex, lastPixel);
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
			Color color = PixelToColor(this.rightPixel);
			this.rightColorButton.BackColor = color;
			this.rightLabel.Text = String.Format("Right: {0} - {1} - {2}", color.R, color.G, color.B);
		}

		#endregion Color and layer

		#region Menu and toolstrip

		private void animationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.OpenAnimationForm();
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

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.CopyGraphic(true);
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.CopyGraphic(false);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.PasteGraphic();
		}

		private void copyToBitmapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.CopyGraphicToBitmap(1);
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.selected = true;
			this.selectStartPoint = new Point(0, 0);
			this.selectEndPoint = new Point(Constants.ElementWidth * 3 * (int)this.graphicPanel.ZoomFactor - 1, Constants.ElementHeight * 8 * (int)this.graphicPanel.ZoomFactor - 1);
			//this.toolStripStatusLabel1.Text = String.Format("Select: {0}, {1}, {2}, {3}", this.selectStartPoint.ToString(), this.drawingSelection, this.selectEndPoint.ToString(), this.selected);
			this.graphicPanel.Invalidate();
		}

		private void zoomInButton_Click(object sender, EventArgs e)
		{
			this.ZoomInOut(true);
		}

		private void zoomOutButton_Click(object sender, EventArgs e)
		{
			this.ZoomInOut(false);
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Undo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Redo();
		}

		#endregion Menu and toolstrip

		#endregion Event-Handler

		#region Nested Classes

		private interface ICommand<T>
		{
			T Do(T input);

			T Undo(T input);
		}

		private class ChangePixelsCommand : ICommand<uint[,]>
		{
			private uint[] oldValues;

			public uint Value { get; set; }

			public Point[] Positions { get; set; }

			public ChangePixelsCommand()
			{
				this.Value = 0;
			}

			public ChangePixelsCommand(uint value, Point[] positions)
			{
				this.Value = value;
				this.Positions = positions;
			}

			public uint[,] Do(uint[,] graphic)
			{
				if (graphic == null)
					throw new ArgumentNullException("graphic");
				List<uint> oldValues = new List<uint>();
				foreach (Point point in Positions)
				{
					if ((point.X < 0 && point.Y < 0 && point.X >= graphic.GetLength(1) && point.Y >= graphic.GetLength(0)))
						throw new ArgumentOutOfRangeException("Position not in graphic");
					oldValues.Add(graphic[point.Y, point.X]);
					graphic[point.Y, point.X] = Value;
				}
				if (oldValues.Count != Positions.Length)
					throw new ArgumentException("Length of arrays not equal");
				this.oldValues = oldValues.ToArray();
				return graphic;
			}

			public uint[,] Undo(uint[,] graphic)
			{
				if (graphic == null)
					throw new ArgumentNullException("graphic");
				if (this.oldValues == null || this.oldValues.Length <= 0)
					throw new ArgumentException("Cannot undo changes that are not done yet!");
				for (int i = 0; i < this.Positions.Length; i++)
				{
					graphic[this.Positions[i].Y, this.Positions[i].X] = this.oldValues[i];
				}
				return graphic;
			}
		}

		private class UndoRedoStack
		{
			private Stack<ICommand<uint[,]>> undo;
			private Stack<ICommand<uint[,]>> redo;

			public UndoRedoStack()
			{
				Reset();
			}

			public void Reset()
			{
				this.undo = new Stack<ICommand<uint[,]>>();
				this.redo = new Stack<ICommand<uint[,]>>();
			}

			public int UndoCount
			{
				get
				{
					return undo.Count;
				}
			}

			public int RedoCount
			{
				get
				{
					return redo.Count;
				}
			}

			public uint[,] Do(ICommand<uint[,]> cmd, uint[,] graphic)
			{
				uint[,] output = cmd.Do(graphic);
				this.undo.Push(cmd);
				this.redo.Clear();
				return output;
			}

			public uint[,] Undo(uint[,] graphic)
			{
				if (this.undo.Count > 0)
				{
					ICommand<uint[,]> cmd = this.undo.Pop();
					uint[,] output = cmd.Undo(graphic);
					this.redo.Push(cmd);
					return output;
				}
				else
				{
					return graphic;
				}
			}

			public uint[,] Redo(uint[,] graphic)
			{
				if (this.redo.Count > 0)
				{
					ICommand<uint[,]> cmd = this.redo.Pop();
					uint[,] output = cmd.Do(graphic);
					this.undo.Push(cmd);
					return output;
				}
				else
				{
					return graphic;
				}
			}
		}

		#endregion Nested Classes
	}
}