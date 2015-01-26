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
			Pixel[,] elementExpected = new Pixel[heightExpected, widthExpected];
			for (int i = 0; i < heightExpected; i++)
			{
				for (int j = 0; j < widthExpected; j++)
				{
					if (i == 2)
					{
						elementExpected[i, j] = Pixel.TransparentPixel();
					}
					else if(j == 3)
					{
						elementExpected[i, j] = Pixel.RGBPixel(200, 200, 200);
					}
					else
					{
						elementExpected[i, j] = Pixel.RGBPixel(80, 90, 100);
					}
				}
			}
			Layer layer = new Layer(heightExpected, widthExpected, x0Expected, y0Expected, (short)Constants.GFY_Z_VG, elementExpected);
			
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
