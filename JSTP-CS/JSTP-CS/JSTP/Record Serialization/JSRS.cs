using System;
using System.Collections.Generic;
using System.Text;

using Jstp.Types;

namespace Jstp.Rs {
	/// <summary> Static class for parsing Record Serialization. </summary>
	public static class JSRS {
		private static char[] data;

		/// <summary> Returns string representation of JSValue </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string Stringify(JSValue value) {
			return value.ToString();
		}

		/// <summary> Parses Record Seralization data. </summary>
		/// <param name="dataToParse"></param>
		/// <returns></returns>
		public static JSValue Parse(string dataToParse) {
			try {
				if (dataToParse == null) {
					throw new ArgumentNullException("Data which required to be parsed is null!");
				}

				data = RemoveComment(dataToParse);
				int index = 0;

				JSValue value = ParseValue(ref index);

				return value;
			}

			catch (ArgumentNullException ex) {
				Console.WriteLine(ex);
				return JSNull.Null;
			}

			catch (JSRSFormatException ex) {
				Console.WriteLine(ex);
				return JSUndefined.Undefined;
			}	
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
				case Token.TBracketOpen:
					return ParseArray(ref index);
				case Token.TTrue:
					NextToken(ref index);
					return new JSBool(true);
				case Token.TFalse:
					NextToken(ref index);
					return new JSBool(false);
				case Token.TNull:
					NextToken(ref index);
					return JSNull.Null;
				case Token.TUndefined:
					return JSUndefined.Undefined;
			}

			return JSUndefined.Undefined;
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
			Token token;

			JSObject obj = new JSObject();

			// Skipes "{"
			NextToken(ref index);

			while (true) {
				token = LookAhead(index);
				if (token == Token.TUndefined) {
					return JSUndefined.Undefined;
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
						return JSUndefined.Undefined; // throw new JSRSFormatException();
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
			JSArray array = new JSArray();

			// Skip [
			NextToken(ref index);

			Token curToken = LookAhead(index);

			if (curToken == Token.TBracketClose) {
				NextToken(ref index);
				array.Push(JSUndefined.Undefined);
				return array;
			}

			while (true) {
				if (index == data.Length) {
					return JSUndefined.Undefined;
				}

				if (curToken == Token.TComma) {
					NextToken(ref index);
					array.Push(JSUndefined.Undefined);
					curToken = LookAhead(index);
				}
				else {
					array.Push(ParseValue(ref index));

					curToken = NextToken(ref index);

					if (curToken == Token.TBracketClose) {
						break;
					}
					else if (curToken != Token.TComma) {
						return JSUndefined.Undefined;
					}
					else {
						curToken = LookAhead(index);
					}
				}

			}
			return array;
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
				case '[':
					return Token.TBracketOpen;
				case ']':
					return Token.TBracketClose;
				case '"':
				case '\'':
				case '`':
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
				return Token.TTrue;
			}

			if (IsTNull(remainingLength, index)) {
				index += 4;
				return Token.TNull;
			}

			if(IsTUndefined(remainingLength, index)) {
				index += 9;
			}

			if (char.IsLetter(data[index])) {
				return Token.TKey;
			}

			return Token.TNone;
		}

		/// <summary> Enum for comments mode </summary>
		private enum CommentMode : byte {
			Disabled = 0,
			OneLine,
			MultiLine
		}

		/// <summary>
		/// Find and remove comments and whitespaces
		/// </summary>
		/// <param name="dataToParse"></param>
		/// <returns></returns>
		private static char[] RemoveComment(string dataToParse) {

			StringBuilder result = new StringBuilder(dataToParse.Length);

			bool stringMode = false;
			CommentMode commentMode = CommentMode.Disabled;

			for (int i = 0; i < dataToParse.Length; i++) {
				// Checks for string start
				if ((dataToParse[i] == '\"' || dataToParse[i] == '\'') && (i == 0 || dataToParse[i - 1] != '\\')) {
					stringMode = !stringMode;
				}

				// If not string
				if (!stringMode) {
					// Checks for comment start
					if (commentMode == CommentMode.Disabled && dataToParse[i] == '/') {
						switch (dataToParse[i + 1]) {
							case '/':
								commentMode = CommentMode.OneLine;
								break;
							case '*':
								commentMode = CommentMode.MultiLine;
								break;
						}
					}

					// Adds char to resulting string if it's not comment and whitespace
					if (commentMode == CommentMode.Disabled && !char.IsWhiteSpace(dataToParse[i])) {
						result.Append(dataToParse[i]);
					}

					// Checks for comment end
					if ((commentMode == CommentMode.OneLine && (dataToParse[i] == '\n' || dataToParse[i] == '\r')) ||
						(commentMode == CommentMode.MultiLine && dataToParse[i - 1] == '*' && dataToParse[i] == '/')) {

						commentMode = CommentMode.Disabled;
					}
				}

				else {
					result.Append(dataToParse[i]);
				}

			}
			return result.ToString().ToCharArray();
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

		private static bool IsTUndefined(int remainingLength, int index) {
			if (remainingLength >= 9) {
				if (data[index] == 'u' &&
					data[index + 1] == 'n' &&
					data[index + 2] == 'd' &&
					data[index + 3] == 'e' &&
					data[index + 4] == 'f' &&
					data[index + 5] == 'i' &&
					data[index + 6] == 'n' &&
					data[index + 7] == 'e' &&
					data[index + 8] == 'd') {
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}
