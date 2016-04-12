using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Types {
	class JSNumber: JSValue{

		private double jsNumber;

		public JSNumber(double number) {
			this.jsNumber = number;
		}

		public override string ToString() {
			return jsNumber.ToString();
		}
	}
}
