using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Rs {
	enum Token : byte {
		TString = 0,
		TNumber = 1,
		TCurlyOpen = 2,
		TCurlyClose = 3,
		TTrue = 4,
		TFalse = 5,
		TColon = 6,
		TComma = 7,
		TNull = 8,
		TNone = 9
	}
}
