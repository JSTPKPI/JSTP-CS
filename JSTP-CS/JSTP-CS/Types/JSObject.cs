using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Types {
	class JSObject: JSValue{
		// temporary
		private Dictionary<string, JSValue> jsObject;

		public override string ToString() {
			return jsObject.ToString();
		}

		public JSValue this[string key] {
			get {
				return (jsObject.ContainsKey(key)) ? jsObject[key] : new JSUndefind();
			}
			set {
				jsObject[key] = value;
			}
		}
	}
}
