using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BahnEditor.BahnLib;
using System.Collections.Generic;

namespace BahnEditor.Test
{
	[TestClass]
	public class UnitTest
	{
		#region Tests
		[TestMethod]
		public void TestSaveAndLoad()
		{
			string infoTextExpected = "Test";
			short heightExpected = (short)(Constants.ElementHeight * 8);
			short widthExpected = (short)(Constants.ElementWidth * 3);
			//short x0Expected = 7;
			//short y0Expected = 5;
			Pixel colorInSchematicModeExpected = new Pixel(50, 50, 50);
			Pixel[,] elementExpected = new Pixel[heightExpected, widthExpected];
			for (int i = 0; i < elementExpected.GetLength(0); i++)
			{
				for (int j = 0; j < elementExpected.GetLength(1); j++)
				{
					if (i == 2)
						elementExpected[i, j] = new Pixel(200, 200, 200);
					else if (i == 3)
						elementExpected[i, j] = new Pixel(Pixel.SpecialPixelWithoutRGB.BehindGlass);
					else if (i == 5)
						elementExpected[i, j] = new Pixel(Pixel.SpecialPixelWithoutRGB.As_Marking_Point_Bus0);
					else if (i == 4)
						elementExpected[i, j] = new Pixel(Pixel.SpecialPixelWithRGB.Always_Bright, 100, 100, 100);
					else if (i == 6)
						elementExpected[i, j] = new Pixel(Pixel.SpecialPixelWithRGB.Lamp_Red, 0, 100, 100);
					else
						elementExpected[i, j] = Pixel.TransparentPixel();
				}
			}
			Layer layer = new Layer(LayerId.Foreground, elementExpected);

			Graphic graphic = new Graphic(infoTextExpected, ZoomFactor.Zoom1);
			
			graphic.AddLayer(layer);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			CompareGraphic(graphic, newGraphic);
			if((newGraphic.Properties & GraphicProperties.ColorFormat24BPP) != GraphicProperties.ColorFormat24BPP)
			{
				throw new Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException("Graphic has not set ColorFormat24BPP");
			}
		}

		[TestMethod]
		public void TestGraphicArchive()
		{
			Graphic expectedGraphic1 = new Graphic("test1", ZoomFactor.Zoom1);
			Graphic expectedGraphic2 = new Graphic("test2", ZoomFactor.Zoom1);

			expectedGraphic1.AddTransparentLayer(LayerId.Foreground);
			expectedGraphic2.AddTransparentLayer(LayerId.Foreground);

			expectedGraphic1.GetLayer(LayerId.Foreground).Element[10, 10] = new Pixel(100, 50, 20);
			expectedGraphic2.GetLayer(LayerId.Foreground).Element[50, 40] = new Pixel(150, 63, 123);

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
			expectedGraphic.AddTransparentLayer(LayerId.Foreground);
			expectedGraphic.GetLayer(LayerId.Foreground).Element[10, 10] = new Pixel(123, 123, 123);
			expectedGraphic.Properties = GraphicProperties.Clock | GraphicProperties.ColorSchematicMode | GraphicProperties.Smoke;
			expectedGraphic.SteamXPosition = 10;
			expectedGraphic.SteamYPosition = 10;
			expectedGraphic.SteamWidth = 5;
			expectedGraphic.ColorInSchematicMode = new Pixel(100, 100, 100);
			expectedGraphic.ClockXPosition = 10;
			expectedGraphic.ClockYPosition = 10;
			expectedGraphic.ClockZPosition = LayerId.Foreground;
			expectedGraphic.ClockWidth = 5;
			expectedGraphic.ClockHeight = 5;
			expectedGraphic.ClockColorHoursPointer = new Pixel(Pixel.SpecialPixelWithRGB.Always_Bright, 200, 0, 0);
			expectedGraphic.ClockColorMinutesPointer = new Pixel(0, 0, 255);
			expectedGraphic.ClockProperties = ClockProperties.Display24h | ClockProperties.MinutePointer;
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

		[TestMethod]
		public void TestDrivingWay()
		{
			Graphic expectedGraphic = new Graphic("TestDrivingWay");
			expectedGraphic.AddTransparentLayer(LayerId.Foreground);
			expectedGraphic.GetLayer(LayerId.Foreground).Element[10, 10] = new Pixel(123, 123, 123);
			expectedGraphic.Properties = GraphicProperties.Cursor | GraphicProperties.DrivingWay | GraphicProperties.ColorSchematicMode | GraphicProperties.ColorFormat24BPP;
			expectedGraphic.CursorNormalDirection = Direction.South;
			expectedGraphic.CursorReverseDirection = Direction.South;
			expectedGraphic.ColorInSchematicMode = new Pixel(0, 0, 0);
			expectedGraphic.DrivingWay.Add(new DrivingWayElement(DrivingWay.Rail, DrivingWayFunction.Crossing, Direction.North, Direction.North));
			expectedGraphic.DrivingWay.Add(new DrivingWayElement(DrivingWay.Rail, DrivingWayFunction.Crossing, Direction.South, Direction.South));
			expectedGraphic.Save("testDrivingWay.gz1", true);

			Graphic graphic = Graphic.Load("testDrivingWay.gz1");
			Assert.AreEqual(expectedGraphic.Properties | GraphicProperties.ColorFormat24BPP, graphic.Properties);
			CompareGraphic(expectedGraphic, graphic);
			Assert.AreEqual(expectedGraphic.DrivingWay.Count, graphic.DrivingWay.Count);
			for (int i = 0; i < expectedGraphic.DrivingWay.Count; i++)
			{
				CompareDrivingWay(expectedGraphic.DrivingWay[i], graphic.DrivingWay[i]);
			}
		}

		[TestMethod]
		public void TestPixelEnum()
		{
			NeoPixel np = new NeoPixel(10, 0, 0);
			Assert.IsFalse(np.IsSpecial);
			Assert.IsTrue(np.UsesRGB);

			np = new NeoPixel(0, 0, 0, NeoPixel.PixelProperty.Transparent);
			Assert.IsTrue(np.IsSpecial);
			Assert.IsFalse(np.UsesRGB);

			np = new NeoPixel(0, 0, 0, NeoPixel.PixelProperty.AlwaysBright);
			Assert.IsTrue(np.IsSpecial);
			Assert.IsTrue(np.UsesRGB);
		}

		[TestMethod]
		public void TestPixelOldNew()
		{
			NeoPixel np1 = new NeoPixel(123, 221, 90);
			Pixel px1 = new Pixel(123, 221, 90);
			Assert.AreEqual<uint>(np1.ToUInt(), px1.ToUInt());

			NeoPixel np2 = new NeoPixel(0, 0, 0, NeoPixel.PixelProperty.AsRailsTrackbed0);
			Pixel px2 = new Pixel(Pixel.SpecialPixelWithoutRGB.As_Rails_Trackbed0);
			Assert.AreEqual<uint>(np2.ToUInt(), px2.ToUInt());

			NeoPixel np3 = new NeoPixel(123, 221, 90, NeoPixel.PixelProperty.AlwaysBright);
			Pixel px3 = new Pixel(Pixel.SpecialPixelWithRGB.Always_Bright, 123, 221, 90);
			Assert.AreEqual<uint>(np3.ToUInt(), px3.ToUInt());
		}

		[TestMethod]
		public void TestPixelNewEqualsSimpleTrue()
		{
			NeoPixel np1a = new NeoPixel(123, 221, 90);
			NeoPixel np1b = new NeoPixel(123, 221, 90);
			Assert.IsTrue(np1a.Equals(np1b));
		}

		[TestMethod]
		public void TestPixelNewEqualsSimpleFalse()
		{
			NeoPixel np1a = new NeoPixel(123, 221, 90);
			NeoPixel np1b = new NeoPixel(123, 221, 0);
			Assert.IsFalse(np1a.Equals(np1b));
		}

		[TestMethod]
		public void TestPixelNewEqualsPropertyDifferent()
		{
			NeoPixel np1a = new NeoPixel(123, 221, 90);
			NeoPixel np1b = new NeoPixel(123, 221, 90, NeoPixel.PixelProperty.Transparent);
			Assert.IsFalse(np1a.Equals(np1b));
		}

		[TestMethod]
		public void TestPixelNewEqualsNoRGB()
		{
			NeoPixel np1a = new NeoPixel(0, 0, 0, NeoPixel.PixelProperty.Transparent);
			NeoPixel np1b = new NeoPixel(123, 221, 90, NeoPixel.PixelProperty.Transparent);
			Assert.IsTrue(np1a.Equals(np1b));
		}

		[TestMethod]
		public void TestPixelNewEqualsPropertyAndRGB()
		{
			NeoPixel np1a = new NeoPixel(123, 221, 90, NeoPixel.PixelProperty.AlwaysBright);
			NeoPixel np1b = new NeoPixel(123, 221, 90, NeoPixel.PixelProperty.AlwaysBright);
			Assert.IsTrue(np1a.Equals(np1b));

			var test1 = (int)np1b.Property;
			var test2 = (uint)np1b.Property;
			Console.WriteLine("AA");
		}
		#endregion Tests

		#region Private Methods
		private void CompareGraphic(Graphic expectedGraphic, Graphic graphic)
		{
			Assert.AreEqual<string>(expectedGraphic.InfoText, graphic.InfoText);
			Assert.AreEqual<ZoomFactor>(expectedGraphic.ZoomFactor, graphic.ZoomFactor);
			Assert.AreEqual<Pixel>(expectedGraphic.ColorInSchematicMode, graphic.ColorInSchematicMode);

			for (int i = 0; i < expectedGraphic.GetLayer(LayerId.Foreground).Element.GetLength(0); i++)
			{
				for (int j = 0; j < expectedGraphic.GetLayer(LayerId.Foreground).Element.GetLength(1); j++)
				{
					Assert.AreEqual<Pixel>(expectedGraphic.GetLayer(LayerId.Foreground).Element[i, j], graphic.GetLayer(LayerId.Foreground).Element[i, j]);
				}
			}
		}

		private void CompareDrivingWay(DrivingWayElement expectedDrivingWayElement, DrivingWayElement drivingWayElement)
		{
			Assert.AreEqual<DrivingWay>(expectedDrivingWayElement.DrivingWay, drivingWayElement.DrivingWay);
			Assert.AreEqual<DrivingWayFunction>(expectedDrivingWayElement.Function, drivingWayElement.Function);
			Assert.AreEqual<Direction>(expectedDrivingWayElement.Arrival, drivingWayElement.Arrival);
			Assert.AreEqual<Direction>(expectedDrivingWayElement.Departure, drivingWayElement.Departure);
		}
		#endregion Private Methods
	}
}
