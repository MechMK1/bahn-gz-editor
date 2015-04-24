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
			uint colorInSchematicModeExpected = 50 << 16 | 50 << 8 | 50;
			uint[,] elementExpected = new uint[heightExpected, widthExpected];
			for (int i = 0; i < elementExpected.GetLength(0); i++)
			{
				for (int j = 0; j < elementExpected.GetLength(1); j++)
				{
					if (i == 2)
						elementExpected[i, j] = (200 << 16 | 200 << 8 | 200);
					else if (i == 3)
						elementExpected[i, j] = (uint)Pixel.PixelProperty.BehindGlass;
					else if (i == 5)
						elementExpected[i, j] = (uint)Pixel.PixelProperty.AsMarkingPointBus0;
					else if (i == 4)
						elementExpected[i, j] = 100 << 16 | 100 << 8 | 100 | (uint) Pixel.PixelProperty.AlwaysBright;
					else if (i == 6)
						elementExpected[i, j] = 100 << 8 | 100 | (uint) Pixel.PixelProperty.LampRed;
					else
						elementExpected[i, j] = Constants.ColorTransparent;
				}
			}
			Layer layer = new Layer(LayerId.Foreground, elementExpected);

			Graphic graphic = new Graphic(infoTextExpected, ZoomFactor.Zoom1);

			graphic.AddLayer(LayerId.Foreground, layer);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			CompareGraphic(graphic, newGraphic);
			if ((newGraphic.Properties & GraphicProperties.ColorFormat24BPP) != GraphicProperties.ColorFormat24BPP)
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

			expectedGraphic1.GetLayer(LayerId.Foreground).Element[10, 10] = 100 << 16 | 50 << 8 | 20;
			expectedGraphic2.GetLayer(LayerId.Foreground).Element[50, 40] = 150 << 16 | 63 << 8 | 123;

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
			expectedGraphic.GetLayer(LayerId.Foreground).Element[10, 10] = 123 << 16 | 123 << 8 | 123;
			expectedGraphic.Properties = GraphicProperties.Clock | GraphicProperties.ColorSchematicMode | GraphicProperties.Smoke;
			expectedGraphic.SteamXPosition = 10;
			expectedGraphic.SteamYPosition = 10;
			expectedGraphic.SteamWidth = 5;
			//HACK FAIL
			//expectedGraphic.ColorInSchematicMode = 100 << 16 | 100 << 8 | 100;
			expectedGraphic.ClockXPosition = 10;
			expectedGraphic.ClockYPosition = 10;
			expectedGraphic.ClockZPosition = LayerId.Foreground;
			expectedGraphic.ClockWidth = 5;
			expectedGraphic.ClockHeight = 5;
			//expectedGraphic.ClockColorHoursPointer = 200 << 16 | (uint)Pixel.PixelProperty.AlwaysBright;
			//expectedGraphic.ClockColorMinutesPointer = 255;
			expectedGraphic.ClockProperties = ClockProperties.Display24h | ClockProperties.MinutePointer;
			expectedGraphic.Save("testProperties.gz1", true);

			Graphic graphic = Graphic.Load("testProperties.gz1");
			Assert.AreEqual(expectedGraphic.Properties, graphic.Properties);
			Assert.AreEqual(expectedGraphic.SteamXPosition, graphic.SteamXPosition);
			Assert.AreEqual(expectedGraphic.SteamYPosition, graphic.SteamYPosition);
			Assert.AreEqual(expectedGraphic.SteamWidth, graphic.SteamWidth);
			//Assert.AreEqual(expectedGraphic.ColorInSchematicMode, graphic.ColorInSchematicMode);
			Assert.AreEqual(expectedGraphic.ClockXPosition, graphic.ClockXPosition);
			Assert.AreEqual(expectedGraphic.ClockYPosition, graphic.ClockYPosition);
			Assert.AreEqual(expectedGraphic.ClockZPosition, graphic.ClockZPosition);
			Assert.AreEqual(expectedGraphic.ClockProperties, graphic.ClockProperties);
			Assert.AreEqual(expectedGraphic.ClockHeight, graphic.ClockHeight);
			Assert.AreEqual(expectedGraphic.ClockWidth, graphic.ClockWidth);
			//Assert.AreEqual(expectedGraphic.ClockColorHoursPointer, graphic.ClockColorHoursPointer);
			//Assert.AreEqual(expectedGraphic.ClockColorMinutesPointer, graphic.ClockColorMinutesPointer);
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
		public void TestPixelToUInt()
		{
			Pixel rgbOnly = new Pixel(1, 2, 3);
			Assert.AreEqual<uint>(66051, rgbOnly.ToUInt());

			Pixel rgbWithProperty = new Pixel(1, 2, 3, Pixel.PixelProperty.AlwaysBright);
			Assert.AreEqual<uint>(66051 | Constants.ColorAlwaysBright, rgbWithProperty.ToUInt());

			Pixel propertyOnly = new Pixel(0, 0, 0, Pixel.PixelProperty.Transparent);
			Assert.AreEqual<uint>(Constants.ColorTransparent, propertyOnly.ToUInt());
		}

		[TestMethod]
		public void TestPixelEquals()
		{
			Pixel a = new Pixel(1, 2, 3, Pixel.PixelProperty.Transparent);
			Pixel b = new Pixel(0, 0, 0, Pixel.PixelProperty.Transparent);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(b.Equals(a));

			b.Property = Pixel.PixelProperty.AlwaysBright;
			Assert.IsFalse(a.Equals(b));
			Assert.IsFalse(b.Equals(a));

			a.Property = Pixel.PixelProperty.AlwaysBright;
			Assert.IsFalse(a.Equals(b));
			Assert.IsFalse(b.Equals(a));

			a.Red = 0;
			a.Green = 0;
			a.Blue = 0;
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(b.Equals(a));
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
