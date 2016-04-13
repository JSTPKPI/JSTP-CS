using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jstp.Rs;
using Jstp.Types;
using System;

namespace JSTPTest {


	[TestClass]
	public class JSTPMainTest {

		#region Data
		string rs = "{"+
			"name:'Marcus Aurelius'," + 
			"passport:'AE127095'," +
			"birth:{" +
				"date:'1990-02-15'," +
				"place:'Rome'" +
			"}," +
			"contacts:{" +
				"email:'marcus@aurelius.it'," +
				"phone:'+380505551234'," +
				"address:{" +
					"country:'Ukraine'," +
					"city:'Kiev'," +
					"zip:'03056'," + 
					"street:'Pobedy'," +
					"building:'37'," + 
					"floor:'1'," +
					"room:'158'" +
				"}" +
			"}" +
		"}";
		#endregion

		[TestMethod]
		public void testRecordSerialization() {
			JSValue jValue = JSRS.Parse(rs);

			Assert.IsTrue(jValue.IsObject());

			Assert.AreEqual(rs, JSRS.Stringify(jValue));
		}

		
	}

}
