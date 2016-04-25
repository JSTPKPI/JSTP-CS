using System.Collections.Generic;
using System.Linq;

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
				return (jsObject.ContainsKey(key)) ? jsObject[key] : (JSUndefined.Undefined);
			}
			set {
				jsObject[key] = value;
			}
		}

		/// <summary> Returns the string that represents current object. </summary>
		public override string ToString() {
			return "{" + 
				string.Join(",", jsObject.Select(kv => kv.Key.ToString() + ":" + kv.Value.ToString()).ToArray()) 
				+ "}";
		}
	}
}
