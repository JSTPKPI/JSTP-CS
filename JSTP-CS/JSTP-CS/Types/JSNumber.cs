namespace Jstp.Types {
	/// <summary> Represents JavaScript number type. </summary>
	public class JSNumber: JSValue{

		private static readonly double DEFAULT_VALUE = 0.0; 
		private double jsNumber;

		/// <summary> Gets or sets number representation of this instance. </summary>
		public double Value {
			get {
				return jsNumber;
			}
			set {
				jsNumber = value;
			}
		}

		/// <summary> Initializes a new instance of the Jstp.Types.JSNumber class. </summary>
		public JSNumber() {
			type = JSTypes.JSNumber;
			jsNumber = DEFAULT_VALUE;
		}

		/// <summary> Initializes a new instance of the Jstp.Types.JSNumber class using specified double value. </summary>
		/// <param name="number"></param>
		public JSNumber(double number) : this() {
			jsNumber = number;
		}

		/// <summary> Converts the numeric value of this instance to its equivalent string representation. </summary>
		public override string ToString() {
			return jsNumber.ToString();
		}
	}
}
