namespace Jstp.Types {
	/// <summary> Represents JavaScript string type </summary>
	public class JSString: JSValue {
		private string jsString;

		/// <summary> Initializes a new instance of the Jstp.Types.JSString class. </summary>
		public JSString() {
			type = JSTypes.JSString;
		}

		/// <summary>
		/// Initializes a new instance of the Jstp.Types.JSString class using specified string.
		/// </summary>
		/// <param name="jsString"></param>
		public JSString(string jsString) : this() {
			this.jsString = jsString;
			jsString.ToString();
		}

		/// <summary> Converts value of this instance to string. </summary>
		public override string ToString() {
			return jsString;
		}
	}
}
