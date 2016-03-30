using System;
using System.Collections.Generic;
using System.IO;

namespace JSTP_CS
{
    class Parse
    {
        public Dictionary<string, object> RSParse(string fileName)
        {
            int i = 0;
            List<string> list = new List<string>();
            foreach (string item in File.ReadAllLines(fileName))
            {
                i++;
                list.Add(item);
                foreach(var v in list)
                {
                    //v.Split() -> split by values
                }
            }
            

            return new Dictionary<string, object>();
        }
    }
}
