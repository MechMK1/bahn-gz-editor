using BahnEditor.BahnLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BahnEditor.Editor
{
	public class GraphicPanel : PictureBox
	{
		private Bitmap canvas = new Bitmap(Constants.ElementWidth, Constants.ElementHeight);
		private int zoomLevelZoom1 = 4;
		private int zoomLevelZoom2 = 6;
		private int zoomLevelZoom4 = 8;

		public ZoomFactor ZoomFactor { get; set; }

		public int ZoomLevel
		{
			get
			{
				switch (this.ZoomFactor)
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
				switch (this.ZoomFactor)
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

		public bool DisplayGrid { get; set; }

		public GraphicPanel()
			: this(ZoomFactor.Zoom1, 5)
		{
		}

		public GraphicPanel(ZoomFactor zoomFactor, int zoomlevel)
		{
			this.DoubleBuffered = true;
			this.ZoomFactor = zoomFactor;
			this.ZoomLevel = zoomlevel;
		}

		public void Draw(uint[,] graphic)
		{
			this.canvas.Dispose();
			this.Size = new Size((int)((this.ZoomLevel) * Constants.ElementWidth * 3), (int)((this.ZoomLevel) * Constants.ElementHeight * 8));
			this.canvas = new Bitmap(this.Size.Width, this.Size.Height);
			this.canvas.SetResolution(16, 16);
			using (Graphics g = Graphics.FromImage(canvas))
			{
				if (graphic == null)
				{
					using (TextureBrush backgroundBrush = new TextureBrush(GetBackgroundBitmapByZoomlevel(this.ZoomLevel, this.ZoomFactor)))
					{
						g.FillRectangle(backgroundBrush, 0, 0, Constants.ElementWidth * this.ZoomLevel * 3, Constants.ElementHeight * this.ZoomLevel * 8);
					}
				}
				else
				{
					DrawElement(g, graphic, this.ZoomLevel, true, this.ZoomFactor);
				}
				DrawGrid(g);
			}
			this.Invalidate();
			//this.Image = canvas;
		}

		public void Draw(Point[] points, uint pixel)
		{
			if (points != null && points.Count() > 0)
			{
				using (Graphics g = Graphics.FromImage(canvas))
				{
					Brush brush;
					if (Pixel.IsTransparent(pixel))
						brush = new TextureBrush(GetBackgroundBitmapByZoomlevel(this.ZoomLevel, this.ZoomFactor));
					else if (Pixel.IsSpecial(pixel))
						brush = new HatchBrush(HatchStyle.Percent20, Color.FromArgb(140, 140, 140), PixelToColor(pixel));
					else
						brush = new SolidBrush(PixelToColor(pixel));
					foreach (Point point in points)
					{
						g.FillRectangle(brush,
							(point.X * ZoomLevel) / (int)ZoomFactor,
							((ZoomLevel * Constants.ElementHeight * 8 * (int)this.ZoomFactor) -
							(ZoomLevel * (point.Y + 1))) / (int)this.ZoomFactor,
							ZoomLevel / (float)ZoomFactor,
							ZoomLevel / (float)ZoomFactor);
					}
					brush.Dispose();
				}
				this.Invalidate();
				//this.Image = canvas;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (canvas != null)
			{
				e.Graphics.InterpolationMode = InterpolationMode.Low;
				e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
				e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
				e.Graphics.DrawImage(canvas, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
				//e.Graphics.DrawImageUnscaled(canvas, 0, 0);
			}
			base.OnPaint(e);
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

		private void DrawGrid(Graphics g)
		{
			if (this.DisplayGrid)
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
			}
		}

		internal static void DrawElement(Graphics g, uint[,] element, int ZoomLevel, bool withHatchBrush, ZoomFactor zoomFactor)
		{
			if (withHatchBrush)
			{
				using (TextureBrush backgroundBrush = new TextureBrush(GetBackgroundBitmapByZoomlevel(ZoomLevel, zoomFactor)))
				{
					g.FillRectangle(backgroundBrush, 0, 0, element.GetLength(1) * ZoomLevel / (int)zoomFactor, element.GetLength(0) * ZoomLevel / (int)zoomFactor);
				}
			}
			else
			{
				using (SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(0, 112, 0)))
				{
					g.FillRectangle(backgroundBrush, 0, 0, element.GetLength(1) * ZoomLevel / (int)zoomFactor, element.GetLength(0) * ZoomLevel / (int)zoomFactor);
				}
			}
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
						g.FillRectangle(brush, (j * ZoomLevel) / (int)zoomFactor, ((ZoomLevel * element.GetLength(0)) - (ZoomLevel * (i + 1))) / (int)zoomFactor, ZoomLevel / (float)zoomFactor, ZoomLevel / (float)zoomFactor);
						brush.Dispose();
					}
				}
			}
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
	}
}