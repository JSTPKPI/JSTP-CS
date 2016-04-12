using System.Collections.Generic;

namespace Jstp.Types {
	/// <summary>  Represents JavaScript object type. </summary>
	public class JSObject: JSValue{

		private Dictionary<string, JSValue> jsObject;

		/// <summary> Initializes a new instance of the Jstp.Types.JSObject class that is empty. </summary>
		public JSObject() {
			type = JSTypes.JSObject;
			jsObject = new Dictionary<string, JSValue>();
		}

		/// <summary> Gets or sets the value associated with the specified key. </summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <returns></returns>
		public JSValue this[string key] {
			get {
				return (jsObject.ContainsKey(key)) ? jsObject[key] : (new JSUndefined());
			}
			set {
				jsObject[key] = value;
			}
		}

		/// <summary> Returns the string that represents current object. </summary>
		public override string ToString() {
			return jsObject.ToString();
		}
	}
}
