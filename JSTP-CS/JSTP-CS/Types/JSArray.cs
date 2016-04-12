using System;
using System.Collections.Generic;

namespace Jstp.Types {
	class JSArray: JSValue {

		private List<JSValue> jsArray = new List<JSValue>();

		public override string ToString() {
			return jsArray.ToString();
		}

		public JSValue this[int i] {
			get {
				return (i >= 0 && i < jsArray.Count) ? jsArray[i] : new JSUndefined();
			}
			set {
				if (i > 0) {
					if( i > jsArray.Count) {
						while (jsArray.Count < i) {
							jsArray.Add(new JSUndefined());
						}

						jsArray.Add(value);
					}
					else {
						jsArray[i] = value;
					}
				}
			}
		}

		public void Add(JSValue obj) {
			jsArray.Add(obj);
		}
	}
}
