namespace Jstp.Types {
	/// <summary> Represents JavaScript undefined type. </summary>
	public sealed class JSUndefined: JSValue{

		/// <summary> Initializes a new instance of the Jstp.Types.JSUndefined class. </summary>
		public JSUndefined() {
			type = JSTypes.JSUndefined;
		}

		/// <summary> Returns "undefined" string. </summary>
		public override string ToString() {
			return "undefined";
		}
	}
}
