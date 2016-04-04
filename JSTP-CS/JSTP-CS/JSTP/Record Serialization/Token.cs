using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Rs {
	enum Token : byte {
		TString		= 0,
		TKey		= 1,
		TNumber		= 2,
		TCurlyOpen	= 3,
		TCurlyClose = 4,
		TTrue		= 5,
		TFalse		= 6,
		TColon		= 7,
		TComma		= 8,
		TNull		= 9,
		TNone		= 10
	}
}
