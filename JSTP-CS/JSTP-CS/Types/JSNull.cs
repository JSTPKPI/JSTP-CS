namespace Jstp.Types {
	/// <summary> Represents JavaScript null type. </summary>
	public sealed class JSNull: JSValue {

		/// <summary> Initializes a new instance of the Jstp.Types.JSNull class. </summary>
		private JSNull() {
			type = JSTypes.JSNull;
		}

		private static readonly JSNull nullValue = new JSNull ();

		public static JSNull Null {
			get {
				return nullValue;
			}
		}

		/// <summary> Returns "null" string. </summary>
		public override string ToString() {
			return "null";
		}
	}
}
