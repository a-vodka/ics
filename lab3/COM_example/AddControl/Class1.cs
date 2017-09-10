using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;


// compile with: /target:library
// post-build command: regasm CSharpServer.dll /tlb:CSharpServer.tlb

namespace CSharpServer
{
    [Guid("DBE0E8C4-1C61-41f3-B6A4-4E2F353D3D05"),
    ComVisible(true)]
    public interface IManagedInterface
    {
        int PrintHi(string name);
    }

    [Guid("C6659361-1625-4746-931C-36014B146679"),
    ClassInterface(ClassInterfaceType.None),
    ComVisible(true)]
    public class InterfaceImplementation : IManagedInterface
    {
        public int PrintHi(string name)
        {
            MessageBox.Show("Hello world");
            Console.WriteLine("Hello, {0}!", name);
            return 33;
        }
    }

}
