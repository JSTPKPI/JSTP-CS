namespace Jstp.Types {
	/// <summary> Represents JavaScript undefined type. </summary>
	public sealed class JSUndefined: JSValue {
		
		/// <summary> Initializes a new instance of the Jstp.Types.JSUndefined class. </summary>
		private JSUndefined() {
			type = JSTypes.JSUndefined;
		}

		private static readonly JSUndefined undefined = new JSUndefined ();

		public static JSUndefined Undefined {
			get {
				return undefined;
			}
		}
		/// <summary> Returns "undefined" string. </summary>
		public override string ToString() {
			return "undefined";
		}
	}
}
