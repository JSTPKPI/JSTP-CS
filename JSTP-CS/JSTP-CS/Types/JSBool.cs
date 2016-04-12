using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Types {
	class JSBool: JSValue {
		private bool jsBool;

		public JSBool(bool jsBool) {
			this.jsBool = jsBool;
		}

		public override string ToString() {
			return jsBool? "true" : "false";
		}
	}
}
