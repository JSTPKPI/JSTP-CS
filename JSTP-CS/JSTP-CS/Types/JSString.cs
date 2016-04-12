using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Types {
	class JSString: JSValue {
		private string jsString;

		public JSString(string jsString) {
			this.jsString = jsString;
		}

		public override string ToString() {
			return jsString;
		}
	}
}
