using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jstp.Types;
using static Jstp.Rs.JSRS;

namespace JSTPTest.Record_Serialization {
	[TestClass]
	public class ArrayTest {

		/// <summary> Tests parse of the simple array that contains 2 strings for 1000000 times. </summary>
		[TestMethod]
		public void TestArray1() {
			string arrayStr = @"['Marcus Aurelius','AE127095']";
			JSValue jValue = null;

			for (int i = 0; i < 1000000; i++) {
				 jValue = Parse(arrayStr);
			}

			Assert.IsTrue(jValue.IsArray());

			JSArray actual = (JSArray)jValue;

			JSArray expected = new JSArray(new JSValue[]{
				new JSString("Marcus Aurelius"),
				new JSString("AE127095") });

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}


		/// <summary> Tests parse of the nested arrays. </summary>
		[TestMethod]
		public void TestArray2() {
			string testStr = @"['Marcus Aurelius','AE127095',['1990-02-15','Rome'],['Ukraine','Kiev','03056','Pobedy','37','158']]";
			JSValue jValue = Parse(testStr);

			Assert.IsTrue(jValue.IsArray());

			JSArray actual = (JSArray)jValue;

			JSArray expected = new JSArray(new JSValue[] {
				new JSString("Marcus Aurelius"),
				new JSString("AE127095"),
				new JSArray(new JSValue[]{
						new JSString("1990-02-15"),
						new JSString("Rome")
					}),
				new JSArray(new JSValue[] {
						new JSString("Ukraine"),
						new JSString("Kiev"),
						new JSString("03056"),
						new JSString("Pobedy"),
						new JSString("37"),
						new JSString("158")
					})
				});

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		/// <summary> Tests array with 10 undefined values. </summary>
		[TestMethod]
		public void TestArray3() {
			string testStr = @"[,,,,,,,,,]";

			JSValue jValue = Parse(testStr);

			Assert.IsTrue(jValue.IsArray());

			JSArray actual = (JSArray)jValue;

			JSArray expected = new JSArray();
			for (int i = 0; i < 10; i++) {
				expected.Push(new JSUndefined());
			}

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

	}
}
