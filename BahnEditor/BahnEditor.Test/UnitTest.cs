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

			Graphic graphic = new Graphic(infoTextExpected, ZoomFactor.Zoom1);
			
			graphic.AddLayer(layer);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			CompareGraphic(graphic, newGraphic);
		}

		[TestMethod]
		public void TestGraphicArchive()
		{
			Graphic expectedGraphic1 = new Graphic("test1", ZoomFactor.Zoom1);
			Graphic expectedGraphic2 = new Graphic("test2", ZoomFactor.Zoom1);

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

		[TestMethod]
		public void TestGraphicProperties()
		{
			Graphic expectedGraphic = new Graphic("test");
			expectedGraphic.AddTransparentLayer(Constants.LAYER_VG);
			expectedGraphic.GetLayer(0).Element[10, 10] = Pixel.RGBPixel(123, 123, 123);
			expectedGraphic.Properties = GraphicProperties.Clock | GraphicProperties.ColorSchematicMode | GraphicProperties.Smoke;
			expectedGraphic.SteamXPosition = 10;
			expectedGraphic.SteamYPosition = 10;
			expectedGraphic.SteamWidth = 5;
			expectedGraphic.ColorInSchematicMode = Pixel.RGBPixel(100, 100, 100);
			expectedGraphic.ClockXPosition = 10;
			expectedGraphic.ClockYPosition = 10;
			expectedGraphic.ClockZPosition = Constants.LAYER_VG;
			expectedGraphic.ClockWidth = 5;
			expectedGraphic.ClockHeight = 5;
			expectedGraphic.ClockColorHoursPointer = Pixel.SpecialPixelWithRGB(Pixel.SpecialColorWithRGB.Always_Bright, 200, 0, 0);
			expectedGraphic.ClockColorMinutesPointer = Pixel.RGBPixel(0, 0, 255);
			expectedGraphic.ClockProperties = GraphicClockProperties.Display24h | GraphicClockProperties.MinutePointer;
			expectedGraphic.Save("testProperties.gz1", true);

			Graphic graphic = Graphic.Load("testProperties.gz1");
			Assert.AreEqual(expectedGraphic.Properties, graphic.Properties);
			Assert.AreEqual(expectedGraphic.SteamXPosition, graphic.SteamXPosition);
			Assert.AreEqual(expectedGraphic.SteamYPosition, graphic.SteamYPosition);
			Assert.AreEqual(expectedGraphic.SteamWidth, graphic.SteamWidth);
			Assert.AreEqual(expectedGraphic.ColorInSchematicMode, graphic.ColorInSchematicMode);
			Assert.AreEqual(expectedGraphic.ClockXPosition, graphic.ClockXPosition);
			Assert.AreEqual(expectedGraphic.ClockYPosition, graphic.ClockYPosition);
			Assert.AreEqual(expectedGraphic.ClockZPosition,graphic.ClockZPosition);
			Assert.AreEqual(expectedGraphic.ClockProperties, graphic.ClockProperties);
			Assert.AreEqual(expectedGraphic.ClockHeight, graphic.ClockHeight);
			Assert.AreEqual(expectedGraphic.ClockWidth, graphic.ClockWidth);
			Assert.AreEqual(expectedGraphic.ClockColorHoursPointer, graphic.ClockColorHoursPointer);
			Assert.AreEqual(expectedGraphic.ClockColorMinutesPointer, graphic.ClockColorMinutesPointer);
			CompareGraphic(expectedGraphic, graphic);
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
