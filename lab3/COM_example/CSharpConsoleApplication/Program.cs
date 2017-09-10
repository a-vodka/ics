using System;
using System.Collections.Generic;
using System.Text;
using CSharpServer;

namespace CSharpConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            InterfaceImplementation serv = new InterfaceImplementation();
            serv.PrintHi("vasya");
        }
    }
}
