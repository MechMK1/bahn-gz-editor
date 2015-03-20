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

			Graphic graphic = new Graphic(infoTextExpected, colorInSchematicModeExpected, ZoomFactor.Zoom1);
			graphic.AddLayer(layer);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			CompareGraphic(graphic, newGraphic);
		}

		[TestMethod]
		public void TestGraphicArchive()
		{
			Graphic expectedGraphic1 = new Graphic("test1", Pixel.RGBPixel(50, 50, 50), ZoomFactor.Zoom1);
			Graphic expectedGraphic2 = new Graphic("test2", Pixel.RGBPixel(60, 60, 60), ZoomFactor.Zoom1);

			expectedGraphic1.AddTransparentLayer(Constants.LAYER_VG);
			expectedGraphic2.AddTransparentLayer(Constants.LAYER_VG);

			expectedGraphic1.GetLayer(0).Element[10, 10] = Pixel.RGBPixel(100, 50, 20);
			expectedGraphic2.GetLayer(0).Element[50, 40] = Pixel.RGBPixel(150, 63, 123);

			GraphicArchive expectedArchive = new GraphicArchive(ZoomFactor.Zoom1);
			expectedArchive.AddGraphic(expectedGraphic1);
			expectedArchive.AddGraphic(expectedGraphic2);

			expectedArchive.Save("test2.uz1", true);

			GraphicArchive archive = GraphicArchive.Load("test2.uz1");
			Assert.AreEqual(2, archive.GraphicsCount);

			CompareGraphic(expectedGraphic1, archive[0]);
			CompareGraphic(expectedGraphic2, archive[1]);
		}

		private void CompareGraphic(Graphic expectedGraphic, Graphic graphic)
		{
			Assert.AreEqual<string>(expectedGraphic.InfoText, graphic.InfoText);
			Assert.AreEqual<ZoomFactor>(expectedGraphic.ZoomFactor, graphic.ZoomFactor);
			Assert.AreEqual<Pixel>(expectedGraphic.ColorInSchematicMode, graphic.ColorInSchematicMode);

			for (int i = 0; i < expectedGraphic.GetLayerByLayerID(Constants.LAYER_VG).Element.GetLength(0); i++)
			{
				for (int j = 0; j < expectedGraphic.GetLayerByLayerID(Constants.LAYER_VG).Element.GetLength(1); j++)
				{
					Assert.AreEqual<Pixel>(expectedGraphic.GetLayerByLayerID(Constants.LAYER_VG).Element[i, j], graphic.GetLayerByLayerID(Constants.LAYER_VG).Element[i, j]);
				}
			}
		}
	}
}
