namespace Jstp.Types {
	/// <summary> Represents JavaScript bool type. </summary>
	public sealed class JSBool: JSValue {
		private bool jsBool;

		/// <summary> Gets or sets bool representation of this instance. </summary>
		public bool Value {
			get {
				return jsBool;
			}

			set {
				jsBool = value;
			}
		}

		/// <summary> Initializes a new instance of the Jstp.Types.JSBool class. </summary>
		public JSBool() {
			type = JSTypes.JSBool;
		}

		/// <summary>
		/// Initializes a new instance of the Jstp.Types.JSBool class using specified bool value.
		/// </summary>
		/// <param name="jsBool"></param>
		public JSBool(bool jsBool) : this() {
			this.jsBool = jsBool;
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation (either "true" or "false").
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return jsBool? "true" : "false";
		}
	}
}
