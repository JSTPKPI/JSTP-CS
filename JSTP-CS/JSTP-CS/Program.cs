using System;
using System.Collections.Generic;
using Jint;

namespace JSTP_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            var add = new Engine().Execute("function add(a, b) { return a + b; }").GetValue("add");
        
            add.Invoke(1, 2);
        }
    }
}