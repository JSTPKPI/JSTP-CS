using System;
using System.Collections.Generic;
using System.Text;

using Jstp.Types;

namespace Jstp.Rs {

	delegate JSValue ParseDelegate(ref int index);
	delegate Token SpecialTokenDelegate(int remainingLength, ref int index);

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
				return new JSNull();
			}

			catch (JSRSFormatException ex) {
				Console.WriteLine(ex);
				return new JSUndefined();
			}	
		}
		//+
		/// <summary>
		/// Determines next token type and parses it.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static JSValue ParseValue(ref int index) {
			ParseDelegate pd;
			if(!parseSwitcher.TryGetValue(LookAhead(index), out pd)) {
				throw new JSRSFormatException();
			}
			return pd(ref index);
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
						throw new JSRSFormatException();
					}

					// Parses Value
					JSValue value = ParseValue(ref index);

					obj[key] = value;
				}
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
				array.Push(new JSUndefined());
				return array;
			}

			while (true) {
				if (index == data.Length) {
					return new JSUndefined();
				}

				if (curToken == Token.TBracketClose) {
					array.Push(new JSUndefined());
					break;
				}

				if (curToken == Token.TComma) {
					NextToken(ref index);
					array.Push(new JSUndefined());
					curToken = LookAhead(index);
				}
				else {
					array.Push(ParseValue(ref index));

					curToken = NextToken(ref index);

					if (curToken == Token.TBracketClose) {
						break;
					}

					else if (curToken != Token.TComma) {
						return new JSUndefined();
					}
					else {
						curToken = LookAhead(index);
					}
				}

			}
			return array;
		}

		//+-
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
					char control;

					if (contorlCharSwitcher.TryGetValue(c, out control)) {
						s.Append(control);
					}
					else if(c == 'u'){
						s.Append(ParseUnicode(index));
					}
					else {
						throw new JSRSFormatException();
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

		//+
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

		//+
		private static JSValue ParseTrue(ref int index) {
			index += 4;
			return new JSBool(true);
		}

		//+
		private static JSValue ParseFalse(ref int index) {
			index += 5;
			return new JSBool(false);
		}

		//+
		private static JSValue ParseNull(ref int index) {
			index += 4;
			return new JSNull();
		}

		//+
		private static JSValue ParseUndefined(ref int index) {
			index += 9;
			return new JSUndefined();
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

		#region Util functions

		#region Switchers
		private static Dictionary<Token, ParseDelegate> parseSwitcher = new Dictionary<Token, ParseDelegate> { 
			{ Token.TCurlyOpen,		new ParseDelegate(ParseObject)	  },
			{ Token.TBracketOpen,	new ParseDelegate(ParseArray)	  },
			{ Token.TString,		new ParseDelegate(ParseString)	  },
			{ Token.TNumber,		new ParseDelegate(ParseNumber)	  },
			{ Token.TTrue,			new ParseDelegate(ParseTrue)	  },
			{ Token.TFalse,			new ParseDelegate(ParseFalse)	  },
			{ Token.TNull,			new ParseDelegate(ParseNull)	  },
			{ Token.TUndefined,		new ParseDelegate(ParseUndefined) },
		};

		private static Dictionary<char, char> contorlCharSwitcher = new Dictionary<char, char> {
			{ '"' , '"'  },
			{ '\\', '\\' },
			{ '/' , '/'  },
			{ 'b' , '\b' },
			{ 'f' , '\n' },
			{ 'n' , '\n' },
			{ 'r' , '\r' },
			{ 't' , '\t' }
		};

		private static Dictionary<char, Token> tokenSwitcher = new Dictionary<char, Token> {
			{ '{', Token.TCurlyOpen		},
			{ '}', Token.TCurlyClose	},
			{ '[', Token.TBracketOpen	},
			{ ']', Token.TBracketClose	},

			{ '"' , Token.TString },
			{ '\'', Token.TString },
			{ '`' , Token.TString },

			{ ',', Token.TComma },
			{ ':', Token.TColon },

			{ '-', Token.TNumber },
			{ '0', Token.TNumber },
			{ '1', Token.TNumber },
			{ '2', Token.TNumber },
			{ '3', Token.TNumber },
			{ '4', Token.TNumber },
			{ '5', Token.TNumber },
			{ '6', Token.TNumber },
			{ '7', Token.TNumber },
			{ '8', Token.TNumber },
			{ '9', Token.TNumber }
		};

		private static Dictionary<char, SpecialTokenDelegate> specialTokenSwither = new Dictionary<char, SpecialTokenDelegate> {
			{'t', new SpecialTokenDelegate(IsTTrue) },
			{'f', new SpecialTokenDelegate(IsTFalse) },
			{'n', new SpecialTokenDelegate(IsTNull) },
			{'u', new SpecialTokenDelegate(IsTUndefined) }
		};
		#endregion


		/// <summary>
		/// Checks next token without moving index(cursor).
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static Token LookAhead(int index) {
			int saveIndex = index;
			return NextToken(ref saveIndex);
		}


		//index++;

		//switch (c) {
		//	case '{':
		//		return Token.TCurlyOpen;
		//	case '}':
		//		return Token.TCurlyClose;
		//	case '[':
		//		return Token.TBracketOpen;
		//	case ']':
		//		return Token.TBracketClose;
		//	case '"':
		//	case '\'':
		//	case '`':
		//		return Token.TString;
		//	case ',':
		//		return Token.TComma;
		//	case '0':
		//	case '1':
		//	case '2':
		//	case '3':
		//	case '4':
		//	case '5':
		//	case '6':
		//	case '7':
		//	case '8':
		//	case '9':
		//	case '-':
		//		return Token.TNumber;
		//	case ':':
		//		return Token.TColon;
		//}
		//index--;

		/// <summary>
		/// Checks next token.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private static Token NextToken(ref int index) {

			if (index == data.Length)
				return Token.TUndefined;

			char c = data[index];
			Token token;
			if(tokenSwitcher.TryGetValue(c, out token)) {
				index++;
				return token;
			}

			int remainingLength = data.Length - index;

			SpecialTokenDelegate sp;
			if(specialTokenSwither.TryGetValue(c, out sp)) {
				Token special = sp(remainingLength, ref index);
				if(special != Token.TNone) {
					return special; 
				}
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
		static private string ParseUnicode(int index) {
			int remainingLength = data.Length - index;

			if (remainingLength >= 4) {

				// Parses the 32 bit hex into an integer codepoint
				uint codePoint;
				if (!(uint.TryParse(new string(data, index, 4),
					System.Globalization.NumberStyles.HexNumber,
					System.Globalization.CultureInfo.InvariantCulture, out codePoint))) {
					throw new JSRSFormatException();
				}

				// Converts the integer codepoint to a unicode char and add to string
				return char.ConvertFromUtf32((int)codePoint);
			}
			else {
				throw new JSRSFormatException();
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
		/// <returns>Token.TFalse if next token false; TNone otherwise.</returns>
		private static Token IsTFalse(int remainingLength, ref int index) {
			if (remainingLength >= 5) {
				if (data[index] == 'f' &&
					data[index + 1] == 'a' &&
					data[index + 2] == 'l' &&
					data[index + 3] == 's' &&
					data[index + 4] == 'e') {
					index += 5;
					return Token.TFalse;
				}
			}

			return Token.TNone;
		}

		/// <summary>
		/// Checks whether next token is 'true'.
		/// </summary>
		/// <param name="remainingLength"></param>
		/// <param name="index"></param>
		/// <returns>Token.TTrue if next token true; TNone otherwise.</returns>
		private static Token IsTTrue(int remainingLength, ref int index) {
			if (remainingLength >= 4) {
				if (data[index] == 't' &&
					data[index + 1] == 'r' &&
					data[index + 2] == 'u' &&
					data[index + 3] == 'e') {
					index += 4;
					return Token.TTrue;
				}
			}

			return Token.TNone;
		}

		/// <summary>
		/// Checks whether next token is 'null'
		/// </summary>
		/// <param name="remainingLength"></param>
		/// <param name="index"></param>
		/// <returns>Token.TNull if next token null; TNone otherwise.</returns>
		private static Token IsTNull(int remainingLength, ref int index) {
			if (remainingLength >= 4) {
				if (data[index] == 'n' &&
					data[index + 1] == 'u' &&
					data[index + 2] == 'l' &&
					data[index + 3] == 'l') {
					index += 4;
					return Token.TNull;
				}
			}

			return Token.TNone;
		}

		private static Token IsTUndefined(int remainingLength, ref int index) {
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
					index += 9;
					return Token.TUndefined;
				}
			}

			return Token.TNone;
		}
		
		#endregion
	}
}
