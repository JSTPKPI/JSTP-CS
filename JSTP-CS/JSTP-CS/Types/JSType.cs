using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Types {
	enum JSType: byte {
		JSObject = 1,
		JSArray,
		JSString,
		JSNumber,
		JSBool,
		JSNull,
		JSUndefined,
	}
}
