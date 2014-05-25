﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BahnEditor.BahnLib;

namespace BahnEditor.Test
{
	[TestClass]
	public class UnitTest
	{

		[TestMethod]
		public void TestColorDecompress()
		{
			uint[] input = null;
			uint[] expected = null; //TODO insert actual value
			uint[] output = null;

			output = Color.Decompress(input);

			Assert.Equals(output, expected);
		}

		[TestMethod]
		public void TestColor()
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
			uint[] input = new uint[] { 1684300800, 1684300800, 1684300800, Color.FARBE_TRANSPARENT, Color.FARBE_TRANSPARENT, 1684300800, 1515870720, 1684300800, 1684300800 };
			uint[] expected = new uint[] { 1684300800, 1684300800, 1684300800, Color.FARBE_TRANSPARENT, Color.FARBE_TRANSPARENT, 1684300800, 1515870720, 1684300800, 1684300800 };
			uint[] tmp = Color.Compress(input);
			uint[] output = Color.Decompress(tmp);

			CollectionAssert.AreEqual(expected, output);
		}
	}
}
