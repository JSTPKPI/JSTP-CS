namespace Jstp.Types {
	/// <summary> Represent JavaScript type. </summary>
	public abstract class JSValue {
		/// <summary> Type of the current instance. </summary>
		protected internal JSTypes type;

		/// <summary> Gets type of current instance.  </summary>
		public JSTypes Type {
			get {
				return type;
			}
		}

		/// <summary> Returns true if this instance has array type; otherwise false. </summary>
		public bool isArray() {
			return type == JSTypes.JSArray;
		}

		/// <summary> Returns true if this instance has string type; otherwise false. </summary>
		public bool isString() {
			return type == JSTypes.JSString;
		}

		/// <summary>  Returns true if this instance has number type; otherwise false. </summary>
		public bool isNumber() {
			return type == JSTypes.JSNumber;
		}

		/// <summary> Returns true if this instance has object type; otherwise false. </summary>
		public bool isObject() {
			return type == JSTypes.JSObject;
		}

		/// <summary> Returns true if this instance has bool type; otherwise false. </summary>
		public bool isBool() {
			return type == JSTypes.JSBool;
		}

		/// <summary> Returns true if this instance has null type; otherwise false. </summary>
		public bool isNull() {
			return type == JSTypes.JSNull;
		}

		/// <summary> Returns true if this instance has undefined type; otherwise false. </summary>
		public bool isUndefined() {
			return type == JSTypes.JSUndefined;
		}
	}
}
