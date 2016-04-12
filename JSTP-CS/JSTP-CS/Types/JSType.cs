namespace Jstp.Types {
	/// <summary> JavaScript types. </summary>
	public enum JSTypes: byte {
		/// <summary> Object type. </summary>
		JSObject = 0,
		/// <summary> Array type. </summary>
		JSArray,
		/// <summary> String type. </summary>
		JSString,
		/// <summary> Number type. </summary>
		JSNumber,
		/// <summary> Bool type. </summary>
		JSBool,
		/// <summary> Null. </summary>
		JSNull,
		/// <summary> Undefined. </summary>
		JSUndefined,
	}
}
