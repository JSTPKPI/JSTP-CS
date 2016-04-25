using Jstp.JSTP.Record_Data;
using Jstp.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSTPTest.Record_Data
{
    [TestClass]
    public class JSRecordDataTest
    {
        [TestMethod]
        public void ParseTest()
        {
            string testString =
                @"['Marcus Aurelius','AE127095',['1990-02-15','Rome'],['Ukraine','Kiev','03056','Pobedy','37','158']]";

            JSValue jValue = null;
            for (int i = 0; i < 1000000; i++)
            {
                jValue = JSRD.Parse(testString);
            }

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
                        new JSNumber(03056), 
                        new JSString("Pobedy"),
                        new JSNumber(37), 
                        new JSNumber(158), 
                    })
                });
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }

        [TestMethod]
        public void ParseUndefinedTest()
        {
            string testString =
                @"[,,[,],[,,,,,]]";
           
            JSValue jValue = null;
            for (int i = 0; i < 1000000; i++)
            {
                jValue = JSRD.Parse(testString);
            }

            Assert.IsTrue(jValue.IsArray());

            JSArray actual = (JSArray)jValue;

            JSArray expected = new JSArray(new JSValue[] {
                JSUndefined.Undefined,
                 JSUndefined.Undefined,
                new JSArray(new JSValue[]{
                         JSUndefined.Undefined,
                         JSUndefined.Undefined
                    }),
                new JSArray(new JSValue[] {
                         JSUndefined.Undefined,
                         JSUndefined.Undefined,
                         JSUndefined.Undefined,
                         JSUndefined.Undefined,
                         JSUndefined.Undefined,
                         JSUndefined.Undefined
                    })
                });
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
