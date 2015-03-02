using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BahnEditor.BahnLib;
using System.Collections.Generic;

namespace BahnEditor.Test
{
	[TestClass]
	public class UnitTest
	{

		[TestMethod]
		public void TestSaveAndLoad()
		{
			string infoTextExpected = "Test";
			byte zoomFactorExpected = 1;
			short heightExpected = (short)(Constants.SYMHOEHE * 8);
			short widthExpected = (short)(Constants.SYMBREITE * 3);
			//short x0Expected = 7;
			//short y0Expected = 5;
			Pixel colorInSchematicModeExpected = Pixel.RGBPixel(50, 50, 50);
			Pixel[,] elementExpected = new Pixel[heightExpected, widthExpected];
			for (int i = 0; i < elementExpected.GetLength(0); i++)
			{
				for (int j = 0; j < elementExpected.GetLength(1); j++)
				{
					if (i == 2)
						elementExpected[i, j] = Pixel.RGBPixel(200, 200, 200);
					else if (i == 3)
						elementExpected[i, j] = Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.BehindGlass);
					else if (i == 5)
						elementExpected[i, j] = Pixel.SpecialPixelWithoutRGB(Pixel.SpecialColorWithoutRGB.As_Marking_Point_Bus0);
					else if (i == 4)
						elementExpected[i, j] = Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Always_Bright, 100, 100, 100);
					else if (i == 6)
						elementExpected[i, j] = Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Lamp_Red, 0, 100, 100);
					else
						elementExpected[i, j] = Pixel.TransparentPixel();
				}
			}
			Layer layer = new Layer((short)Constants.LAYER_VG, elementExpected);

			Graphic graphic = new Zoom1Graphic(infoTextExpected, colorInSchematicModeExpected);
			graphic.AddLayer(layer);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			Assert.AreEqual<string>(infoTextExpected, newGraphic.InfoText);
			Assert.AreEqual<byte>(zoomFactorExpected, newGraphic.ZoomFactor);
			Assert.AreEqual<Pixel>(colorInSchematicModeExpected, newGraphic.ColorInSchematicMode);

			for (int i = 0; i < heightExpected; i++)
			{
				for (int j = 0; j < widthExpected; j++)
				{
					Assert.AreEqual<Pixel>(layer.Element[i, j], newGraphic.GetLayerByID(Constants.LAYER_VG).Element[i, j]);
				}
			}
		}
	}
}
