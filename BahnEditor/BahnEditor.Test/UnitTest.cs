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
			//uint colorInSchematicModeExpected = 50 << 16 | 50 << 8 | 50;
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
			

			Graphic graphic = new Graphic(infoTextExpected, ZoomFactor.Zoom1);

			graphic.SetLayer(LayerID.Foreground, elementExpected);
			graphic.Save("test.gz1", true);
			Graphic newGraphic = Graphic.Load("test.gz1");

			CompareGraphic(graphic, newGraphic);
			if (!newGraphic.Properties.RawData.HasFlag(GraphicProperties.Properties.ColorFormat24BPP))
			{
				Assert.Fail("Graphic has not set ColorFormat24BPP");
			}
			System.IO.File.Delete("test.gz1");
		}

		[TestMethod]
		public void TestGraphicArchive()
		{
			Graphic expectedGraphic1 = new Graphic("test1", ZoomFactor.Zoom1);
			Graphic expectedGraphic2 = new Graphic("test2", ZoomFactor.Zoom1);

			expectedGraphic1.AddTransparentLayer(LayerID.Foreground);
			expectedGraphic2.AddTransparentLayer(LayerID.Foreground);

			expectedGraphic1.GetLayer(LayerID.Foreground)[10, 10] = 100 << 16 | 50 << 8 | 20;
			expectedGraphic2.GetLayer(LayerID.Foreground)[50, 40] = 150 << 16 | 63 << 8 | 123;

			GraphicArchive expectedArchive = new GraphicArchive(ZoomFactor.Zoom1);
			expectedArchive.AddGraphic(expectedGraphic1);
			expectedArchive.AddGraphic(expectedGraphic2);

			expectedArchive.Save("test2.uz1", true);

			GraphicArchive archive = GraphicArchive.Load("test2.uz1");
			Assert.AreEqual(2, archive.GraphicsCount);

			CompareGraphic(expectedGraphic1, archive[0]);
			CompareGraphic(expectedGraphic2, archive[1]);

			System.IO.File.Delete("test2.uz1");
		}

		[TestMethod]
		public void TestGraphicProperties()
		{
			Graphic expectedGraphic = new Graphic("test");
			expectedGraphic.AddTransparentLayer(LayerID.Foreground);
			expectedGraphic.GetLayer(LayerID.Foreground)[10, 10] = 123 << 16 | 123 << 8 | 123;
			expectedGraphic.Properties.RawData = GraphicProperties.Properties.Clock | GraphicProperties.Properties.ColorSchematicMode | GraphicProperties.Properties.Smoke;
			expectedGraphic.Properties.ParticleX = 10;
			expectedGraphic.Properties.ParticleY = 10;
			expectedGraphic.Properties.ParticleWidth = 5;
			expectedGraphic.Properties.ColorInSchematicMode = 100 << 16 | 100 << 8 | 100;
			expectedGraphic.Properties.ClockX = 10;
			expectedGraphic.Properties.ClockY = 10;
			expectedGraphic.Properties.ClockZ = LayerID.Foreground;
			expectedGraphic.Properties.ClockWidth = 5;
			expectedGraphic.Properties.ClockHeight = 5;
			expectedGraphic.Properties.ClockColorHoursPointer = 200 << 16 | (uint)Pixel.PixelProperty.AlwaysBright;
			expectedGraphic.Properties.ClockColorMinutesPointer = 255;
			expectedGraphic.Properties.ClockProperties = ClockProperties.Display24h | ClockProperties.MinutePointer;
			expectedGraphic.Save("testProperties.gz1", true);

			Graphic graphic = Graphic.Load("testProperties.gz1");
			Assert.AreEqual(expectedGraphic.Properties.RawData, graphic.Properties.RawData);
			Assert.AreEqual(expectedGraphic.Properties.ParticleX, graphic.Properties.ParticleX);
			Assert.AreEqual(expectedGraphic.Properties.ParticleY, graphic.Properties.ParticleY);
			Assert.AreEqual(expectedGraphic.Properties.ParticleWidth, graphic.Properties.ParticleWidth);
			Assert.AreEqual(expectedGraphic.Properties.ColorInSchematicMode, graphic.Properties.ColorInSchematicMode);
			Assert.AreEqual(expectedGraphic.Properties.ClockX, graphic.Properties.ClockX);
			Assert.AreEqual(expectedGraphic.Properties.ClockY, graphic.Properties.ClockY);
			Assert.AreEqual(expectedGraphic.Properties.ClockZ, graphic.Properties.ClockZ);
			Assert.AreEqual(expectedGraphic.Properties.ClockProperties, graphic.Properties.ClockProperties);
			Assert.AreEqual(expectedGraphic.Properties.ClockHeight, graphic.Properties.ClockHeight);
			Assert.AreEqual(expectedGraphic.Properties.ClockWidth, graphic.Properties.ClockWidth);
			Assert.AreEqual(expectedGraphic.Properties.ClockColorHoursPointer, graphic.Properties.ClockColorHoursPointer);
			Assert.AreEqual(expectedGraphic.Properties.ClockColorMinutesPointer, graphic.Properties.ClockColorMinutesPointer);
			CompareGraphic(expectedGraphic, graphic);

			System.IO.File.Delete("testProperties.gz1");
		}

		[TestMethod]
		public void TestDrivingWay()
		{
			Graphic expectedGraphic = new Graphic("TestDrivingWay");
			expectedGraphic.AddTransparentLayer(LayerID.Foreground);
			expectedGraphic.GetLayer(LayerID.Foreground)[10, 10] = (uint)(123 << 16 | 123 << 8 | 123);

			expectedGraphic.Properties.RawData = GraphicProperties.Properties.Cursor | GraphicProperties.Properties.DrivingWay | GraphicProperties.Properties.ColorSchematicMode | GraphicProperties.Properties.ColorFormat24BPP;
			expectedGraphic.Properties.CursorNormalDirection = Direction.South;
			expectedGraphic.Properties.CursorReverseDirection = Direction.South;
			expectedGraphic.Properties.ColorInSchematicMode = 0;
			expectedGraphic.DrivingWay.Add(new DrivingWayElement(DrivingWay.Rail, DrivingWayFunction.Crossing, Direction.North, Direction.North));
			expectedGraphic.DrivingWay.Add(new DrivingWayElement(DrivingWay.Rail, DrivingWayFunction.Crossing, Direction.South, Direction.South));
			expectedGraphic.Save("testDrivingWay.gz1", true);

			Graphic graphic = Graphic.Load("testDrivingWay.gz1");
			Assert.AreEqual(expectedGraphic.Properties.RawData | GraphicProperties.Properties.ColorFormat24BPP, graphic.Properties.RawData);
			CompareGraphic(expectedGraphic, graphic);
			Assert.AreEqual(expectedGraphic.DrivingWay.Count, graphic.DrivingWay.Count);
			for (int i = 0; i < expectedGraphic.DrivingWay.Count; i++)
			{
				CompareDrivingWay(expectedGraphic.DrivingWay[i], graphic.DrivingWay[i]);
			}
			System.IO.File.Delete("testDrivingWay.gz1");
		}

		[TestMethod]
		public void TestPixelToUInt()
		{
			uint rgbOnly = (uint)(1 << 16 | 2 << 8 | 3);
			Assert.AreEqual<uint>(66051, rgbOnly);

			uint rgbWithProperty = (uint)((1 << 16 | 2 << 8 | 3) | (uint)(Pixel.PixelProperty.AlwaysBright));
			Assert.AreEqual<uint>(66051 | Constants.ColorAlwaysBright, rgbWithProperty);

			uint propertyOnly = (uint)Pixel.PixelProperty.Transparent;
			Assert.AreEqual<uint>(Constants.ColorTransparent, propertyOnly);
		}

		[TestMethod]
		public void TestPixelEquals()
		{
			uint a = Pixel.Create(1, 2, 3, Pixel.PixelProperty.Transparent);
			uint b = Pixel.Create(0, 0, 0, Pixel.PixelProperty.Transparent);
			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(b.Equals(a));

			Pixel.SetProperty(ref b, Pixel.PixelProperty.AlwaysBright);
			Assert.IsFalse(a.Equals(b));
			Assert.IsFalse(b.Equals(a));

			Pixel.SetProperty(ref a, Pixel.PixelProperty.AlwaysBright);
			Pixel.SetRed(ref b, 1);
			Pixel.SetGreen(ref b, 2);
			Pixel.SetBlue(ref b, 3);

			Pixel.SetRed(ref a, 1);
			Pixel.SetGreen(ref a, 2);
			Pixel.SetBlue(ref a, 3);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(b.Equals(a));
		}

		[TestMethod]
		public void TestPixelGetRed()
		{
			uint a = Pixel.Create(1, 2, 3);
			Assert.AreEqual<byte>(1, Pixel.GetRed(a));
			Assert.AreEqual<byte>(1, (byte)(a >> 16));
		}

		[TestMethod]
		public void TestAnimation()
		{
			GraphicArchive archive = GraphicArchive.Load("testSteam.uz1");
			Assert.IsNotNull(archive.Animation);
			Assert.AreEqual<int>(archive.Animation[0, 0].XDiff, 0);
			Assert.AreEqual<int>(archive.Animation[0, 0].YDiff, 0);
			Assert.AreEqual<int>(archive.Animation[0, 0].Width, 1);
			Assert.AreEqual<int>(archive.Animation[0, 0].Height, 1);
			Assert.AreEqual<int>(archive.Animation[0, 0][0].AnimationPhase, 0);
			Assert.AreEqual<int>(archive.Animation[0, 0][0].MinimumTime, 5);
			Assert.AreEqual<int>(archive.Animation[0, 0][0].MaximumTime, 5);
			Assert.AreEqual<int>(archive.Animation[0, 0][0].Sound, 0);
			Assert.IsNull(archive.Animation[1, 0]);
			Assert.IsNull(archive.Animation[0, 0][1]);
		}

		[TestMethod]
		public void TestAnimationSave()
		{
			GraphicArchive expectedArchive = new GraphicArchive(ZoomFactor.Zoom1);
			Graphic graphic1 = new Graphic("test1", ZoomFactor.Zoom1);
			Graphic graphic2 = new Graphic("test2", ZoomFactor.Zoom1);
			Graphic graphic3 = new Graphic("test3", ZoomFactor.Zoom1);
			Graphic graphic4 = new Graphic("test4", ZoomFactor.Zoom1);

			graphic1.AddTransparentLayer(LayerID.Foreground);
			graphic2.AddTransparentLayer(LayerID.Foreground);
			graphic3.AddTransparentLayer(LayerID.Foreground);
			graphic4.AddTransparentLayer(LayerID.Foreground);

			graphic1.GetLayer(LayerID.Foreground)[10, 10] = 100 << 16 | 50 << 8 | 20;
			graphic2.GetLayer(LayerID.Foreground)[50, 40] = 150 << 16 | 63 << 8 | 123;
			graphic3.GetLayer(LayerID.Foreground)[50, 40] = 150 << 16 | 63 << 8 | 123;
			graphic4.GetLayer(LayerID.Foreground)[50, 40] = 150 << 16 | 63 << 8 | 123;

			expectedArchive.AddGraphic(0, 0, Constants.NoAlternative, graphic1);
			expectedArchive.AddGraphic(0, 1, Constants.NoAlternative, graphic2);
			expectedArchive.AddGraphic(0, 2, Constants.NoAlternative, graphic3);
			expectedArchive.AddGraphic(2, graphic4);

			expectedArchive.AddAnimation();
			AnimationProgram expectedProgram = new AnimationProgram(0, 0, 1, 1);
			expectedProgram.AddAnimationStep(new AnimationStep(0, 5, 6, 0));
			expectedProgram.AddAnimationStep(new AnimationStep(1, 5, 6, 0));
			expectedProgram.AddAnimationStep(new AnimationStep(2, 10, 10, 0));
			expectedProgram.AddAnimationStep(new AnimationStep(1, 5, 6, 0));
			expectedArchive.Animation.AddAnimationProgram(expectedProgram, 0, Constants.NoAlternative);

			Assert.IsTrue(expectedArchive.Save("testAnimation.uz1", true));

			GraphicArchive archive = GraphicArchive.Load("testAnimation.uz1");
			CompareGraphic(graphic1, archive[0, 0, Constants.NoAlternative]);
			CompareGraphic(graphic2, archive[0, 1, Constants.NoAlternative]);
			CompareGraphic(graphic3, archive[0, 2, Constants.NoAlternative]);
			CompareGraphic(graphic4, archive[2, 0, Constants.NoAlternative]);

			Assert.IsNotNull(archive.Animation);
			Assert.AreEqual<int>(expectedArchive.Animation.AnimationProgramCount, archive.Animation.AnimationProgramCount);
			CompareAnimationProgram(expectedProgram, archive.Animation[0,Constants.NoAlternative]);

			System.IO.File.Delete("testAnimation.uz1");
			System.IO.File.Delete("testAnimation.bnm");
		}
		#endregion Tests

		#region Private Methods
		private void CompareGraphic(Graphic expectedGraphic, Graphic graphic)
		{
			Assert.AreEqual<string>(expectedGraphic.InfoText, graphic.InfoText);
			Assert.AreEqual<ZoomFactor>(expectedGraphic.ZoomFactor, graphic.ZoomFactor);
			Assert.AreEqual<uint>(expectedGraphic.Properties.ColorInSchematicMode, graphic.Properties.ColorInSchematicMode);

			for (int i = 0; i < expectedGraphic.GetLayer(LayerID.Foreground).GetLength(0); i++)
			{
				for (int j = 0; j < expectedGraphic.GetLayer(LayerID.Foreground).GetLength(1); j++)
				{
					Assert.AreEqual<uint>(expectedGraphic.GetLayer(LayerID.Foreground)[i, j], graphic.GetLayer(LayerID.Foreground)[i, j]);
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

		private void CompareAnimationProgram(AnimationProgram expectedAnimationProgram, AnimationProgram animationProgram)
		{
			Assert.AreEqual<int>(expectedAnimationProgram.XDiff, animationProgram.XDiff);
			Assert.AreEqual<int>(expectedAnimationProgram.YDiff, animationProgram.YDiff);
			Assert.AreEqual<int>(expectedAnimationProgram.Width, animationProgram.Width);
			Assert.AreEqual<int>(expectedAnimationProgram.Height, animationProgram.Height);
			Assert.AreEqual<int>(expectedAnimationProgram.AnimationStepCount, animationProgram.AnimationStepCount);
			for (int i = 0; i < expectedAnimationProgram.AnimationStepCount; i++)
			{
				Assert.AreEqual<int>(expectedAnimationProgram[0].AnimationPhase, animationProgram[0].AnimationPhase);
				Assert.AreEqual<int>(expectedAnimationProgram[0].MinimumTime, animationProgram[0].MinimumTime);
				Assert.AreEqual<int>(expectedAnimationProgram[0].MaximumTime, animationProgram[0].MaximumTime);
				Assert.AreEqual<int>(expectedAnimationProgram[0].Sound, animationProgram[0].Sound);
			}
		}
		#endregion Private Methods
	}
}
