using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Jstp.Types;

namespace Jstp.JSTP.Record_Data
{
    /// <summary>
    ///  Static class for parsing Record Data.
    /// </summary>
    public static class JSRD
    {
        /// <summary>
        /// Parces Record Data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JSValue Parse(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data to parse is null!");
            }

            data = data.Substring(1, data.Length - 2).Replace("'", "");

            return Split(data, EluminateBrackets(data));
        }

        /// <summary>
        /// Splits data string using delimiters positions
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiterPositions"></param>
        /// <returns></returns>
        public static JSValue Split(string input, List<int> delimiterPositions)
        {
            JSArray output = new JSArray();

            for (int i = 0; i < delimiterPositions.Count; i++)
            {
                int index = i == 0 ? 0 : delimiterPositions[i - 1] + 1;
                int length = delimiterPositions[i] - index;
                string s = input.Substring(index, length);
                output.Push(GetValidSubstring(s));
            }
            string lastString = input.Substring(delimiterPositions.Last() + 1);
            output.Push(GetValidSubstring(lastString));

            return output;
        }

        /// <summary>
        /// Search positions of brackets
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<int> EluminateBrackets(string data)
        {
            var delimiterPositions = new List<int>();
            int bracesDepth = 0;
            int bracketsDepth = 0;

            if (data[0] == '[' && data[data.Length - 1] == ']')
            {
                data = data.Substring(1, data.Length - 2);
            }
            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case '[':
                        bracketsDepth++;
                        break;
                    case ']':
                        bracketsDepth--;
                        break;

                    default:
                        if (bracesDepth == 0 && bracketsDepth == 0 && data[i] == ',')
                        {
                            delimiterPositions.Add(i);
                        }
                        break;
                }
            }
            return delimiterPositions;
        }

        /// <summary>
        /// Get right data after substring
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static JSValue GetValidSubstring(string data)
        {
            if (data == string.Empty)
            {
                return JSUndefined.Undefined;
            }
            if (data[0] == '[' && data[data.Length - 1] == ']')
            {
                data = data.Substring(1, data.Length - 2);
                var bracketsPositions = EluminateBrackets(data);
                if (bracketsPositions.Count > 0)
                {
                    return Split(data, bracketsPositions);
                }
            }
            if (data == "true")
            {
                return new JSBool(true);
            }
            if (data == "false")
            {
                return new JSBool(true);
            }
            if (data == "null")
            {
                return JSNull.Null;
            }
            double number;
            if (double.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
            {
                return new JSNumber(number);
            }
            return new JSString(data);
        }

        /// <summary>
        /// Decoding data and metadata
        /// </summary>
        public static void Decode(string data, string metadata)
        {
            throw new NotImplementedException();
        }
    }


}
