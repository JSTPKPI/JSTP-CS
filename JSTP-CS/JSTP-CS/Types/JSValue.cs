namespace Jstp.Types {
	abstract class JSValue {
		private JSType type;

		public JSType Type {
			get {
				return type;
			}
		}

		public bool isArray() {
			return type == JSType.JSArray;
		}

		public bool isString() {
			return type == JSType.JSString;
		}

		public bool isNumber() {
			return type == JSType.JSNumber;
		}

		public bool isObject() {
			return type == JSType.JSObject;
		}

		public bool isBool() {
			return type == JSType.JSBool;
		}

		public bool isNull() {
			return type == JSType.JSNull;
		}

		public bool isUndefined() {
			return type == JSType.JSUndefined;
		}


	}
}
