using BahnEditor.BahnLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BahnEditor.Editor.Util
{
	public class ElementCanvas : Canvas
	{
		public uint[,] Element
		{
			get { return (uint[,])GetValue(ElementProperty); }
			set { SetValue(ElementProperty, value); }
		}
		public int ZoomLevel
		{
			get { return (int)GetValue(ZoomLevelProperty); }
			set { SetValue(ZoomLevelProperty, value); }
		}

		public ZoomFactor ZoomFactor
		{
			get { return (ZoomFactor)GetValue(ZoomFactorProperty); }
			set { SetValue(ZoomFactorProperty, value); }
		}

		public bool GridVisible
		{
			get { return (bool)GetValue(GridVisibleProperty); }
			set { SetValue(GridVisibleProperty, value); }
		}

		public static readonly DependencyProperty ElementProperty =
			DependencyProperty.Register("Element", typeof(uint[,]), typeof(ElementCanvas), new FrameworkPropertyMetadata(OnPropertyChanged));

		public static readonly DependencyProperty ZoomLevelProperty =
			DependencyProperty.Register("ZoomLevel", typeof(int), typeof(ElementCanvas), new FrameworkPropertyMetadata(OnPropertyChanged));

		public static readonly DependencyProperty ZoomFactorProperty =
			DependencyProperty.Register("ZoomFactor", typeof(ZoomFactor), typeof(ElementCanvas), new FrameworkPropertyMetadata(OnPropertyChanged));

		public static readonly DependencyProperty GridVisibleProperty =
			DependencyProperty.Register("GridVisible", typeof(bool), typeof(ElementCanvas), new FrameworkPropertyMetadata(OnPropertyChanged));


		private static void OnPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			ElementCanvas canvas = source as ElementCanvas;
			if (canvas != null)
			{
				canvas.InvalidateVisual();
			}
		}

		//protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		//{
		//	base.OnPropertyChanged(e);
		//	if (e.Property == ElementProperty)
		//	{
		//		MessageBox.Show("Element");
		//		InvalidateVisual();
		//	}
		//	else if (e.Property == ZoomLevelProperty)
		//	{
		//		MessageBox.Show("ZoomLevel");
		//		InvalidateVisual();
		//	}
		//	else if (e.Property == ZoomFactorProperty)
		//	{
		//		MessageBox.Show("ZoomFactor");
		//		InvalidateVisual();
		//	}
		//	else if (e.Property == WidthProperty)
		//	{
		//		MessageBox.Show("Width");
		//		//InvalidateVisual();
		//	}
		//	else if (e.Property == HeightProperty)
		//	{
		//		MessageBox.Show("Height");
		//		//InvalidateVisual();
		//	}
		//}

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);
			BitmapImage backgroundImage = PixelHelper.GetBackgroundBitmapByZoomlevel(ZoomLevel, ZoomFactor);
			backgroundImage.Freeze();
			ImageBrush backgroundBrush = new ImageBrush(backgroundImage);
			backgroundBrush.TileMode = TileMode.Tile;
			backgroundBrush.Viewport = new Rect(0, 0, backgroundImage.Width, backgroundImage.Height);
			backgroundBrush.ViewportUnits = BrushMappingMode.Absolute;
			backgroundBrush.Freeze();
			dc.DrawRectangle(backgroundBrush,
				null,
				new Rect(0d, 0d, Constants.GraphicWidth * ZoomLevel, Constants.GraphicHeight * ZoomLevel)
			);

			if (Element != null)
			{
				for (int i = 0; i < Element.GetLength(0); i++)
				{
					for (int j = 0; j < Element.GetLength(1); j++)
					{
						if (Pixel.IsTransparent(Element[i, j]) == false)
						{
							Brush brush;
							if (Pixel.IsSpecial(Element[i, j]))
							{
								brush = new SolidColorBrush(PixelHelper.PixelToColor(Element[i, j]));
							}
							else
							{
								brush = new SolidColorBrush(PixelHelper.PixelToColor(Element[i, j]));
							}
							brush.Freeze();
							dc.DrawRectangle(brush,
								null,
								new Rect(
									(j * ZoomLevel) / (int)ZoomFactor,
									((ZoomLevel * Element.GetLength(0)) - (ZoomLevel * (i + 1))) / (int)ZoomFactor,
									ZoomLevel / (float)ZoomFactor,
									ZoomLevel / (float)ZoomFactor)
							);
						}
					}
				}
			}
			DrawGrid(dc);
		}

		private void DrawGrid(DrawingContext dc)
		{
			if (GridVisible)
			{
				Pen grayPen = new Pen(Brushes.Gray, 1);
				Pen darkGrayPen = new Pen(Brushes.DarkGray, 1);
				double halfPenWidth = grayPen.Thickness / 2;
				grayPen.Freeze();
				darkGrayPen.Freeze();
				GuidelineSet guidelines;
				for (int i = 1; i < Constants.ElementsXDirection; i++)
				{
					Point p1 = new Point(i * ZoomLevel * Constants.ElementWidth, 0);
					Point p2 = new Point(i * ZoomLevel * Constants.ElementWidth, this.ZoomLevel * Constants.GraphicHeight);
					guidelines = new GuidelineSet();
					guidelines.GuidelinesX.Add(p1.X + halfPenWidth);
					guidelines.GuidelinesY.Add(p2.Y + halfPenWidth);

					dc.PushGuidelineSet(guidelines);
					dc.DrawLine(grayPen, p1, p2);
					dc.Pop();
				}
				for (int i = 1; i < Constants.ElementsYDirection; i++)
				{
					Point p1 = new Point(0, i * ZoomLevel * Constants.ElementHeight);
					Point p2 = new Point(ZoomLevel * Constants.GraphicWidth, i * ZoomLevel * Constants.ElementHeight);
					guidelines = new GuidelineSet();
					guidelines.GuidelinesX.Add(p2.X + halfPenWidth);
					guidelines.GuidelinesY.Add(p1.Y + halfPenWidth);
					guidelines.GuidelinesY.Add(p2.Y + halfPenWidth);

					dc.PushGuidelineSet(guidelines);
					dc.DrawLine(grayPen, p1, p2);
					dc.Pop();
				}
				Rect rect = new Rect(Constants.ElementWidth * ZoomLevel, 6 * Constants.ElementHeight * ZoomLevel, Constants.ElementWidth * ZoomLevel, Constants.ElementHeight * ZoomLevel);
				guidelines = new GuidelineSet();
				guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
				guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
				guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
				guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);

				dc.PushGuidelineSet(guidelines);
				dc.DrawRectangle(null, darkGrayPen, rect);
				dc.Pop();
			}
		}
	}
}
