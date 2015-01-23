using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BahnEditor.BahnLib;

namespace BahnEditor.Test
{
	[TestClass]
	public class UnitTest
	{

		[TestMethod]
		public void TestColorCompressAndDecompress()
		{
			uint[] input = new uint[] { 1684300800, 1684300800, 1684300800, 1684300800 };
			uint[] expected = new uint[] { 1684300800, 1684300800, 1684300800, 1684300800 };
			uint[] tmp = Color.Compress(input);
			uint[] output = Color.Decompress(tmp);

			CollectionAssert.AreEqual(expected, output);
		}

		[TestMethod]
		public void TestColorWithTransparent()
		{
			uint[] input = new uint[] { 1684300800, 1684300800, 1684300800, Color.FARBE_TRANSPARENT, 1684300800, 1717986816, 1684300800, 1684300800 };
			uint[] expected = new uint[] { 1684300800, 1684300800, 1684300800, Color.FARBE_TRANSPARENT, 1684300800, 1717986816, 1684300800, 1684300800 };
			uint[] tmp = Color.Compress(input);
			uint[] output = Color.Decompress(tmp);

			CollectionAssert.AreEqual(expected, output);
		}

		[TestMethod]
		public void TestColorFromRGB()
		{
			PrivateType pt = new PrivateType(typeof(BahnLib.Color)); //Used to access private static methods

			uint blackExpected = 0;
			uint blueExpected = 255;
			uint greenExpected = 65280; // 255 * 256
			uint greenBlueExpected = greenExpected + blueExpected;
			uint redExpected = 16711680; // 255 * 256 * 256
			uint redBlueExpected = redExpected + blueExpected;
			uint redGreenExpected = redExpected + greenExpected;
			uint whiteExpected = redExpected + greenExpected + blueExpected;
			uint purpleExpected = 6553855; // 100 * 256 * 256 + 255

			uint blackActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)0, (byte)0, (byte)0 });
			uint blueActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)0, (byte)0, (byte)255 });
			uint greenActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)0, (byte)255, (byte)0 });
			uint greenBlueActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)0, (byte)255, (byte)255 });
			uint redActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)255, (byte)0, (byte)0 });
			uint redBlueActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)255, (byte)0, (byte)255 });
			uint redGreenActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)255, (byte)255, (byte)0 });
			uint whiteActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)255, (byte)255, (byte)255 });
			uint purpleActual = (uint)pt.InvokeStatic("FromRGB", new Object[] { (byte)100, (byte)0, (byte)255 });

			Assert.AreEqual<uint>(blackExpected, blackActual);
			Assert.AreEqual<uint>(blueExpected, blueActual);
			Assert.AreEqual<uint>(greenExpected, greenActual);
			Assert.AreEqual<uint>(greenBlueExpected, greenBlueActual);
			Assert.AreEqual<uint>(redExpected, redActual);
			Assert.AreEqual<uint>(redBlueExpected, redBlueActual);
			Assert.AreEqual<uint>(redGreenExpected, redGreenActual);
			Assert.AreEqual<uint>(whiteExpected, whiteActual);
			Assert.AreEqual<uint>(purpleExpected, purpleActual);
		}

		[TestMethod]
		public void TestSave()
		{
			Graphic g = new Graphic("Test", 1, 10, 5, 5, 7);
			g.Save("test.gz1", true);
		}
	}
}
