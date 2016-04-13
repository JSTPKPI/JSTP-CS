using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jstp.Types;
using static Jstp.Rs.JSRS;

namespace JSTPTest.Record_Serialization {
	[TestClass]
	public class CommentTest {

		private JSArray expected = new JSArray(new JSValue[]{
				new JSString("Marcus Aurelius"),
				new JSString("AE127095") });

		/// <summary> Tests multiline comments. </summary>
		[TestMethod]
		public void TestMultiLine() {
			string arrayStr = @"['Marcus Aurelius',/*adadasdsds


			d*/'AE127095']";

			JSValue jValue = Parse(arrayStr);
			Assert.IsTrue(jValue.IsArray());

			JSArray actual = (JSArray)jValue;
			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		/// <summary> Tests multiline comments. </summary>
		[TestMethod]
		public void TestOneLine() {
			string arrayStr = @"['Marcus Aurelius',// first value
						'AE127095']";

			JSValue jValue = Parse(arrayStr);
			Assert.IsTrue(jValue.IsArray());

			JSArray actual = (JSArray)jValue;
			Assert.AreEqual(expected.ToString(), actual.ToString());
		}


	}
}
