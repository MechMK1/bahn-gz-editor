using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BahnEditor.BahnLib;

namespace BahnEditor.Test
{
	[TestClass]
	public class UnitTest
	{
		
		[TestMethod]
		public void TestMethod()
		{
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void TestColorDecompress()
		{
			uint[] input = null; //TODO insert actual value
			uint[] expected = null; //TODO insert actual value
			uint[] output = null;

			output = Color.Decompress(input);

			Assert.Equals(output, expected);
		}
	}
}
