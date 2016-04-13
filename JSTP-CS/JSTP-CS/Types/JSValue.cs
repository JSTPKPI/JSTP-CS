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
		public bool IsArray() {
			return type == JSTypes.JSArray;
		}

		/// <summary> Returns true if this instance has string type; otherwise false. </summary>
		public bool IsString() {
			return type == JSTypes.JSString;
		}

		/// <summary>  Returns true if this instance has number type; otherwise false. </summary>
		public bool IsNumber() {
			return type == JSTypes.JSNumber;
		}

		/// <summary> Returns true if this instance has object type; otherwise false. </summary>
		public bool IsObject() {
			return type == JSTypes.JSObject;
		}

		/// <summary> Returns true if this instance has bool type; otherwise false. </summary>
		public bool IsBool() {
			return type == JSTypes.JSBool;
		}

		/// <summary> Returns true if this instance has null type; otherwise false. </summary>
		public bool IsNull() {
			return type == JSTypes.JSNull;
		}

		/// <summary> Returns true if this instance has undefined type; otherwise false. </summary>
		public bool IsUndefined() {
			return type == JSTypes.JSUndefined;
		}
	}
}
