using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Jstp.Rs;

namespace JSTPTest.Record_Serialization {
	[TestClass]
	public class UnitTest1 {
		[TestMethod]
		public void TestMethod1() {
			string test1 = @"{
				name: 'Marcus Aurelius', 
				passport: 'AE127095',
				birth: {
					date: '1990-02-15',
					place: 'Rome'
				},
				contacts: {
					email: 'marcus@aurelius.it',
					phone: '+380505551234'
				}
			}";
			Dictionary<string, object> testRecord = null;
			for (int i = 0; i < 1000000; i++) {
				testRecord = (Dictionary<string, object>)JSRS.Parse(test1);
			}

			Console.WriteLine(testRecord);

			Dictionary<string, object> expectedRecord = new Dictionary<string, object>();
			Dictionary<string, object> birth = new Dictionary<string, object>();
			Dictionary<string, object> contacts = new Dictionary<string, object>();
			expectedRecord.Add("name", "Marcus Aurelius");
			expectedRecord.Add("passport", "AE127095");
			birth.Add("date", "1990-02-15");
			birth.Add("place", "Rome");
			expectedRecord.Add("birth", birth);
			contacts.Add("email", "marcus@aurelius.it");
			contacts.Add("phone", "+380505551234");
			expectedRecord.Add("contacts", contacts);


			Assert.AreEqual(expectedRecord.ToString(), testRecord.ToString());
		}
	}
}
