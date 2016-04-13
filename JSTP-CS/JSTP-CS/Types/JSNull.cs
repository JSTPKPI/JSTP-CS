namespace Jstp.Types {
	/// <summary> Represents JavaScript null type. </summary>
	public sealed class JSNull: JSValue {

		/// <summary> Initializes a new instance of the Jstp.Types.JSNull class. </summary>
		public JSNull() {
			type = JSTypes.JSNull;
		}

		/// <summary> Returns "null" string. </summary>
		public override string ToString() {
			return "null";
		}
	}
}
