using System;
using System.Collections.Generic;
using System.Text;

using Jstp.Types;

namespace Jstp.Rs {
	/// <summary> Static class for parsing Record Serialization. </summary>
	public static class JSRS {
		private static char[] data;

		/// <summary> Parses Record Seralization data. </summary>
		/// <param name="dataToParse"></param>
		/// <returns></returns>
		public static JSValue Parse(string dataToParse) {
			try {
				if (dataToParse == null) {
					throw new ArgumentNullException("Data which required to be parsed is null!");
				}

				char[] data = RemoveComment(dataToParse).ToCharArray();
				int index = 0;

				JSValue value = ParseValue(ref index);

				return value;
			}

			catch (ArgumentNullException ex) {
				Console.WriteLine(ex);
			}

			catch (JSRSFormatException ex) {
				Console.WriteLine(ex);
			}

			return new JSUndefined();
		}

		/// <summary>
		/// Determines next token type and parses it.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static JSValue ParseValue(ref int index) {
			switch (LookAhead(index)) {
				case Token.TCurlyOpen:
					return ParseObject(ref index);
				case Token.TString:
					return ParseString(ref index);
				case Token.TNumber:
					return ParseNumber(ref index);
				case Token.TTrue:
					NextToken(ref index);
					return new JSBool(true);
				case Token.TFalse:
					NextToken(ref index);
					return new JSBool(false);
				case Token.TNull:
					NextToken(ref index);
					return new JSNull();
				case Token.TUndefined:
					return new JSUndefined();
			}

			return new JSUndefined();
		}

		/// <summary>
		/// Parses data to JSNumber.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static JSValue ParseNumber(ref int index) {

			int lastIndex = getIndexOfLastDigit(index);
			int numberLength = lastIndex - index + 1;

			double number;

			if (double.TryParse(new string(data, index, numberLength),
				System.Globalization.NumberStyles.Any,
				System.Globalization.CultureInfo.InvariantCulture,
				out number)) {

				index += numberLength;
				return new JSNumber(number);
			}

			else {
				throw new JSRSFormatException();
			}

		}

		/// <summary>
		/// Parses data to JSString
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static JSValue ParseString(ref int index) {
			StringBuilder s = new StringBuilder(20);
			char c;

			char stringStart = data[index++];

			bool complete = false;
			while (!complete) {

				if (index == data.Length) {
					break;
				}

				c = data[index++];

				// end of the string
				if (c == stringStart) {
					complete = true;
					break;
				}

				#region Control character
				else if (c == '\\') {

					if (index == data.Length) {
						break;
					}

					c = data[index++];
					switch (c) {
						case '"':
							s.Append('"');
							break;
						case '\\':
							s.Append('\\');
							break;
						case '/':
							s.Append('/');
							break;
						case 'b':
							s.Append('\b');
							break;
						case 'f':
							s.Append('\f');
							break;
						case 'n':
							s.Append('\n');
							break;
						case 'r':
							s.Append('\r');
							break;
						case 't':
							s.Append('\t');
							break;
						case 'u':
							// Unicode characters
							if (tryParseUnicode(index, s)) {
								index += 4;
							}
							else {
								throw new JSRSFormatException();
							}
							break;
					}
				}
				#endregion

				// any other character
				else {
					s.Append(c);
				}

			}

			if (!complete) {
				throw new JSRSFormatException();
			}

			return new JSString(s.ToString());
		}

		/// <summary>
		/// Parses data to JSObject.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static JSValue ParseObject(ref int index) {
			Dictionary<string, object> obj1 = new Dictionary<string, object>();
			Token token;

			JSObject obj = new JSObject();

			// Skipes "{"
			NextToken(ref index);

			while (true) {
				token = LookAhead(index);
				if (token == Token.TUndefined) {
					return new JSUndefined();
				}
				else if (token == Token.TComma) {
					NextToken(ref index);
				}
				else if (token == Token.TCurlyClose) {
					NextToken(ref index);
					return obj;
				}
				else {

					// Parses key
					string key = ParseKey(ref index);

					// :
					token = NextToken(ref index);
					if (token != Token.TColon) {
						return new JSUndefined(); // throw new JSRSFormatException();
					}

					// Parses Value
					JSValue value = ParseValue(ref index);

					obj[key] = value;
				}
			}
		}

		/// <summary>
		/// Parses data to key(string).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static string ParseKey(ref int index) {
			StringBuilder sb = new StringBuilder(20);
			char c;

			c = data[index++];
			bool complete = false;
			while (!complete) {
				if (index == data.Length) {
					break;
				}

				if (char.IsLetterOrDigit(c)) {
					sb.Append(c);
					c = data[index++];
				}
				else {
					complete = true;
					break;
				}
			}

			if (sb.Length != 0 && complete) {
				index--;
				return sb.ToString();
			}
			else {
				return null; // throw new JSRSFormatException();
			}
		}

		/// <summary>
		/// Parses data to JSArray
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static JSValue ParseArray(ref int index) {
			return null;
		}

		#region Util functions

		/// <summary>
		/// Checks next token without moving index(cursor).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static Token LookAhead(int index) {
			int saveIndex = index;
			return NextToken(ref saveIndex);
		}

		/// <summary>
		/// Checks next token.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static Token NextToken(ref int index) {

			if (index == data.Length)
				return Token.TUndefined;

			char c = data[index];
			index++;
			switch (c) {
				case '{':
					return Token.TCurlyOpen;
				case '}':
					return Token.TCurlyClose;
				case '"':
				case '\'':
					return Token.TString;
				case ',':
					return Token.TComma;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return Token.TNumber;
				case ':':
					return Token.TColon;
			}
			index--;

			int remainingLength = data.Length - index;

			if (IsTFalse(remainingLength, index)) {
				index += 5;
				return Token.TFalse;
			}

			if (IsTTrue(remainingLength, index)) {
				index += 4;
				return Token.TFalse;
			}

			if (IsTNull(remainingLength, index)) {
				index += 4;
				return Token.TNull;
			}

			// TODO: implement key token check
			if (char.IsLetter(data[index])) {

				return Token.TKey;
			}

			return Token.TUndefined;
		}

			}
        /// <summary>
        /// Find and remove comments and whtespaces
        /// </summary>
        /// <param name="dataToParse"></param>
        /// <returns></returns>
		private static string RemoveComment(string dataToParse)
        {
            bool isCommentStart = false, isValueStart = false;

            char[] arr = new char[dataToParse.Length];
            for (int i = 0; i < dataToParse.Length; i++)
            {
                if (dataToParse[i] == '/' || isCommentStart == true) { isCommentStart = true; continue; }
                else
                    arr[i] = dataToParse[i];
            }
            string forReturn = new string(arr);
            for (int i = 0; i < dataToParse.Length; i++)
            {
                if ((dataToParse[i] == ' ') && isValueStart == false)
                    forReturn = forReturn.Remove(i, 1).Insert(i, string.Empty);
                if (dataToParse[i] == '"' || dataToParse[i] == '\'' || isValueStart == true)
                    isValueStart = true;
            }
            return forReturn;
        }

		/// <summary> Parses Unicode escape sequence. </summary>
		/// <param name="index"></param>
		/// <param name="sb"></param>
		/// <returns> Returns true if parse was successful</returns>
		static private bool tryParseUnicode(int index, StringBuilder sb) {
			int remainingLength = data.Length - index;

			if (remainingLength >= 4) {

				// Parses the 32 bit hex into an integer codepoint
				uint codePoint;
				if (!(uint.TryParse(new string(data, index, 4),
					System.Globalization.NumberStyles.HexNumber,
					System.Globalization.CultureInfo.InvariantCulture, out codePoint))) {
					return false;
				}

				// Converts the integer codepoint to a unicode char and add to string
				sb.Append(char.ConvertFromUtf32((int)codePoint));
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Searches for the last index of the number.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Last index</returns>
		private static int getIndexOfLastDigit(int index) {
			int lastIndex = index;
			string numbs = "0123456789+-.eE";
			for (; lastIndex < data.Length; lastIndex++) {
				if (numbs.IndexOf(data[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}

		/// <summary>
		/// Checks whether token is 'false'.
		/// </summary>
		/// <param name="remainingLength"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static bool IsTFalse(int remainingLength, int index) {
			if (remainingLength >= 5) {
				if (data[index] == 'f' &&
					data[index + 1] == 'a' &&
					data[index + 2] == 'l' &&
					data[index + 3] == 's' &&
					data[index + 4] == 'e') {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks whether next token is 'true'.
		/// </summary>
		/// <param name="remainingLength"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static bool IsTTrue(int remainingLength, int index) {
			if (remainingLength >= 4) {
				if (data[index] == 't' &&
					data[index + 1] == 'r' &&
					data[index + 2] == 'u' &&
					data[index + 3] == 'e') {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks whether next token is 'null'
		/// </summary>
		/// <param name="remainingLength"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static bool IsTNull(int remainingLength, int index) {
			if (remainingLength >= 4) {
				if (data[index] == 'n' &&
					data[index + 1] == 'u' &&
					data[index + 2] == 'l' &&
					data[index + 3] == 'l') {
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}
