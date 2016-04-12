using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jstp.Rs {
	enum Token : byte {
		TString		  = 0,
		TKey		  = 1,
		TNumber		  = 2,
		TCurlyOpen	  = 3,
		TCurlyClose	  = 4,
		TBracketOpen  = 5,
		TBracketClose = 6,
		TTrue		  = 7,
		TFalse		  = 8,
		TColon		  = 9,
		TComma		  = 10,
		TNull		  = 11,
		TComment	  = 12,
		TUndefined	  = 13
	}
}
