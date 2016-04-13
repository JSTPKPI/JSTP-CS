using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jstp.Types;
using static Jstp.Rs.JSRS;

namespace JSTPTest.Record_Serialization {
	[TestClass]
	public class RecordSerialization {

		/// <summary> Tests parse of the null value. </summary>
		[TestMethod]
		public void TestNullValue() {
			JSValue jNull = Parse(null);

			Assert.IsTrue(jNull.IsNull());
		}

		/// <summary> Tests parse of the "null" string. </summary>
		[TestMethod]
		public void TestNullString() {
			JSValue jNull = Parse("null");

			Assert.IsTrue(jNull.IsNull());
		}

		/// <summary> Tests parse of a bool value. </summary>
		[TestMethod]
		public void TestBool() {
			JSValue jTrue = Parse("true");
			JSValue jFalse = Parse("false");

			Assert.IsTrue(jTrue.IsBool());
			Assert.IsTrue(jFalse.IsBool());


			Assert.AreEqual("true", jTrue.ToString());
			Assert.AreEqual("false", jFalse.ToString());
		}

		/// <summary> Tests parse of a number value. </summary>
		[TestMethod]
		public void TestNumber() {
			JSValue jNumber = Parse("32.2");

			Assert.IsTrue(jNumber.IsNumber());
			Assert.AreEqual("32,2", jNumber.ToString());
		}


	}
}
