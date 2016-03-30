using System;
using System.Collections.Generic;
//using Jint;

namespace JSTP_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = @"D:\root\KPI\JSTP-CS\JSTP-CS\parse.txt";
            Parse parse = new Parse();
            parse.RSParse(fileName);     

        }
    }
}