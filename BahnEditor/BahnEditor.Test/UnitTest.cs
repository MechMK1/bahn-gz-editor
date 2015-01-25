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
			short heightExpected = 10;
			short widthExpected = 5;
			short x0Expected = 7;
			short y0Expected = 5;
			Pixel colorInSchematicModeExpected = Pixel.RGBPixel(50, 50, 50);

			Layer layer = new Layer();
			layer.Height = heightExpected;
			layer.Width = widthExpected;
			layer.X0 = x0Expected;
			layer.Y0 = y0Expected;
			layer.LayerID = (short)Constants.GFX_Z_HI;
			layer.Element = new Pixel[heightExpected, widthExpected];

			for (int i = 0; i < heightExpected; i++)
			{
				for (int j = 0; j < widthExpected; j++)
				{
					layer.Element[i, j] = Pixel.RGBPixel(80, 90, 100);
				}
			}

			List<Layer> layerList = new List<Layer> { layer };

			Graphic graphic = new Graphic(infoTextExpected, zoomFactorExpected, colorInSchematicModeExpected, layerList);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			Assert.AreEqual<string>(infoTextExpected, newGraphic.InfoText);
			Assert.AreEqual<byte>(zoomFactorExpected, newGraphic.ZoomFactor);
			Assert.AreEqual<short>(heightExpected, newGraphic.Layers[0].Height);
			Assert.AreEqual<short>(widthExpected, newGraphic.Layers[0].Width);
			Assert.AreEqual<short>(x0Expected, newGraphic.Layers[0].X0);
			Assert.AreEqual<short>(y0Expected, newGraphic.Layers[0].Y0);
			Assert.AreEqual<Pixel>(colorInSchematicModeExpected, newGraphic.ColorInSchematicMode);

			for (int i = 0; i < heightExpected; i++)
			{
				for (int j = 0; j < widthExpected; j++)
				{
					Assert.AreEqual<Pixel>(layer.Element[i, j], newGraphic.Layers[0].Element[i, j]);
				}
			}
		}
	}
}
