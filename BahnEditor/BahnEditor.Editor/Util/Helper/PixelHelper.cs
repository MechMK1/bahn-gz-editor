using BahnEditor.BahnLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BahnEditor.Editor.Util
{
	public static class PixelHelper
	{
		private const string uriFormat = "pack://application:,,,/BahnEditor;component/Resources/{0}.bmp";

		public static uint PixelFromColor(Color color)
		{
			return Pixel.Create(color.R, color.G, color.B);
		}

		public static Color PixelToColor(uint pixel)
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
					return Color.FromArgb(255, Pixel.GetRed(pixel), Pixel.GetGreen(pixel), Pixel.GetBlue(pixel));

				//Without RGB
				case Pixel.PixelProperty.Transparent:
					return Color.FromArgb(255, 0, 112, 0);

				case Pixel.PixelProperty.BehindGlass:
					return Color.FromArgb(255, 0, 50, 100);

				case Pixel.PixelProperty.AsBG:
					return Color.FromArgb(255, 0, 112, 0);

				case Pixel.PixelProperty.AsSleepers0:
					return Color.FromArgb(255, 188, 188, 188);

				case Pixel.PixelProperty.AsSleepers1:
					return Color.FromArgb(255, 84, 40, 0);

				case Pixel.PixelProperty.AsSleepers3:
					return Color.FromArgb(255, 84, 40, 0);

				case Pixel.PixelProperty.AsRailsRoad0:
					return Color.FromArgb(255, 168, 168, 168);

				case Pixel.PixelProperty.AsRailsRoad1:
					return Color.FromArgb(255, 60, 60, 60);

				case Pixel.PixelProperty.AsRailsRoad2:
					return Color.FromArgb(255, 168, 168, 168);

				case Pixel.PixelProperty.AsRailsRoad3:
					return Color.FromArgb(255, 104, 104, 104);

				case Pixel.PixelProperty.AsRailsTrackbed0:
					return Color.FromArgb(255, 104, 104, 104);

				case Pixel.PixelProperty.AsRailsTrackbed1:
					return Color.FromArgb(255, 148, 148, 148);

				case Pixel.PixelProperty.AsRailsTrackbed2:
					return Color.FromArgb(255, 148, 148, 148);

				case Pixel.PixelProperty.AsRailsTrackbed3:
					return Color.FromArgb(255, 104, 104, 104);

				case Pixel.PixelProperty.AsMarkingPointBus0:
					return Color.FromArgb(255, 252, 252, 252);

				case Pixel.PixelProperty.AsMarkingPointBus1:
					return Color.FromArgb(255, 252, 252, 252);

				case Pixel.PixelProperty.AsMarkingPointBus2:
					return Color.FromArgb(255, 252, 252, 252);

				case Pixel.PixelProperty.AsMarkingPointBus3:
					return Color.FromArgb(255, 252, 252, 252);

				case Pixel.PixelProperty.AsMarkingPointWater:
					return Color.FromArgb(255, 84, 252, 252);

				case Pixel.PixelProperty.AsGravel:
					return Color.FromArgb(255, 60, 60, 60);

				case Pixel.PixelProperty.AsSmallGravel:
					return Color.FromArgb(255, 168, 136, 0);

				case Pixel.PixelProperty.AsGrassy:
					return Color.FromArgb(255, 0, 168, 0);

				case Pixel.PixelProperty.AsPathBG:
					return Color.FromArgb(255, 30, 180, 20);

				case Pixel.PixelProperty.AsPathFG:
					return Color.FromArgb(255, 168, 140, 0);

				case Pixel.PixelProperty.AsText:
					return Color.FromArgb(255, 252, 252, 252);

				default:
					throw new ArgumentOutOfRangeException("property", "A property which was not defined here was encountered");
			}
		}

		public static BitmapImage GetBackgroundBitmapByZoomlevel(int zoomLevel, ZoomFactor zoomFactor)
		{
			float zoom = (float)zoomLevel / (float)zoomFactor;
			switch ((int)zoom)
			{
				case 1:
				default:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background"), UriKind.Absolute));

				case 2:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background2"), UriKind.Absolute));

				case 3:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background3"), UriKind.Absolute));

				case 4:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background4"), UriKind.Absolute));

				case 5:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background5"), UriKind.Absolute));

				case 6:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background6"), UriKind.Absolute));

				case 7:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background7"), UriKind.Absolute));

				case 8:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background8"), UriKind.Absolute));

				case 9:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background9"), UriKind.Absolute));

				case 10:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background10"), UriKind.Absolute));

				case 11:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background11"), UriKind.Absolute));

				case 12:
					return new BitmapImage(new Uri(string.Format(uriFormat, "background12"), UriKind.Absolute));
			}
		}
	}
}
