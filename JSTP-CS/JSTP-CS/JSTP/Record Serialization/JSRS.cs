using System.Collections.Generic;
using System.Text;

namespace Jstp.Rs {
	/// <summary>
	/// Static class for parsing Record Serialization.
	/// </summary>
	public static class JSRS {

		private static char[] data;

		// Default capacity for StringBuilder
		private static readonly int DEFAULT_CAPACITY = 20;

		/// <summary>
		/// Parses Record Seralization data.
		/// </summary>
		/// <param name="dataToParse"></param>
		/// <returns></returns>
		public static object Parse(string dataToParse) {
			if (data != null) {
				char[] data = RemoveComment(dataToParse).ToCharArray();
				int index = 0;
				object value = ParseValue(ref index); 
				return value;
			} else {
				return null; //throw new JSRSFormatException();
			}
		}

		private static string RemoveComment(string dataToParse) {
			// TODO: remove comment and whitespaces
			return null;
		}

		/// <summary>
		/// Determines next token type and parses it.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static object ParseValue(ref int index) {
			switch(LookAhead(index)) {
				case Token.TCurlyOpen:
					return ParseObject(ref index);
				case Token.TString:
					return ParseString(ref index);
				case Token.TNumber:
					return ParseNumber(ref index);
				case Token.TTrue:
					NextToken(ref index);
					return true;
				case Token.TFalse:
					NextToken(ref index);
					return false;
				case Token.TNull:
					NextToken(ref index);
					return null;
				case Token.TUndefined:
					return null; // throw new JSRSFormatException();
			}

			return null;
		}

		/// <summary>
		/// Parses data to number.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static double ParseNumber(ref int index) {

			int lastIndex = getIndexOfLastDigit(index);
			int numberLength = lastIndex - index + 1;

			double number;
			if(double.TryParse(new string(data, index, numberLength), 
				System.Globalization.NumberStyles.Any,
				System.Globalization.CultureInfo.InvariantCulture,
				out number)) {

				index += numberLength;
				return number;
			} else {
				return number; //throw new JSRSFormatException();
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
			for(;lastIndex < data.Length; lastIndex++) {
				if(numbs.IndexOf(data[lastIndex]) == -1) {
					break;
				}
			}
			return lastIndex - 1;
		}

		/// <summary>
		/// Parses data to string
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static object ParseString(ref int index) {
			StringBuilder s = new StringBuilder(DEFAULT_CAPACITY);
			char c;

			// "
			c = data[index++];

			bool complete = false;
			while (!complete) {

				if (index == data.Length) {
					break;
				}

				c = data[index++];
				if (c == '"' || c == '\'') {
					complete = true;
					break;
				}
				else if (c == '\\') {

					if (index == data.Length) {
						break;
					}
					c = data[index++];

					if (c == '"') {
						s.Append('"');
					}
					else if (c == '\\') {
						s.Append('\\');
					}
					else if (c == '/') {
						s.Append('/');
					}
					else if (c == 'b') {
						s.Append('\b');
					}
					else if (c == 'f') {
						s.Append('\f');
					}
					else if (c == 'n') {
						s.Append('\n');
					}
					else if (c == 'r') {
						s.Append('\r');
					}
					else if (c == 't') {
						s.Append('\t');
					}
					else if (c == 'u') {
						int remainingLength = data.Length - index;
						if (remainingLength >= 4) {
							// parse the 32 bit hex into an integer codepoint
							uint codePoint;
							if (!(uint.TryParse(new string(data, index, 4),
								System.Globalization.NumberStyles.HexNumber,
								System.Globalization.CultureInfo.InvariantCulture, out codePoint))) {
								return ""; //throw new JSRSFormatException()
							}
							// convert the integer codepoint to a unicode char and add to string
							s.Append(char.ConvertFromUtf32((int)codePoint));
							// skip 4 chars
							index += 4;
						}
						else {
							break;
						}
					}

				}
				else {
					s.Append(c);
				}

			}

			if (!complete) {
				return null;  // throw new JSRSFormatExceptino()
			}

			return s.ToString();
		}

		/// <summary>
		/// Parses data to object(Dictionary&lt;string, object&gt;).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static object ParseObject(ref int index) {
			Dictionary<string, object> obj = new Dictionary<string, object>();
			Token token;
			
			// Skipes "{"
			NextToken(ref index);

			while (true) {
				token = LookAhead(index);
				if(token == Token.TUndefined) {
					return null;  // throw new JSRSFormatException();
				} else if(token == Token.TComma) {
					NextToken(ref index);
				} else if(token == Token.TCurlyClose) {
					NextToken(ref index);
					return obj;
				} else {

					// Parses key
					string key = ParseKey(ref index);

					// :
					token = NextToken(ref index);
					if(token != Token.TColon) {
						return null; // throw new JSRSFormatException();
					}

					// Parses Value
					object value = ParseValue(ref index);


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
			StringBuilder sb = new StringBuilder(DEFAULT_CAPACITY);
			char c;

			c = data[index++];
			bool complete = false;
			while (!complete) {
				if(index == data.Length) {
					break;
				}

				if (char.IsLetterOrDigit(c)) {
					sb.Append(c);
					c = data[index++];
				} else {
					complete = true;
					break;
				}
			}

			if (sb.Length != 0 && complete) {
				index--;
				return sb.ToString();
			} else {
				return null; // throw new JSRSFormatException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static object[] ParseArray(ref int index) {
			NextToken(ref index);

			return null;
		}

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
				case '0': case '1': case '2': case '3': case '4':
				case '5': case '6': case '7': case '8': case '9':
				case '-':
					return Token.TNumber;
				case ':':
					return Token.TColon;
			}
			index--;

			int remainingLength = data.Length - index;

			if(IsTFalse(remainingLength, index)) {
				index += 5;
				return Token.TFalse;
			}

			if(IsTTrue(remainingLength, index)) {
				index += 4;
				return Token.TFalse;
			}

			if(IsTNull(remainingLength, index)) {
				index += 4;
				return Token.TNull;
			}

			// TODO: implement key token check
			if (char.IsLetter(data[index])) {

				return Token.TKey;
			}

			return Token.TUndefined;
		}

		/// <summary>
		/// Checks whether token is 'false'.
		/// </summary>
		/// <param name="remainingLength"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static bool IsTFalse(int remainingLength, int index) {
			if(remainingLength >= 5) {
				if(	data[index]	  == 'f' &&
					data[index+1] == 'a' &&
					data[index+2] == 'l' &&
					data[index+3] == 's' &&
					data[index+4] == 'e') {
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
	}
}
