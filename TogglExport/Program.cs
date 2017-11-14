using System;
using System.Diagnostics;

namespace TogglExport {
    public class Program {
        public static void Main(string[] args) {
            TogglExport.Run(args);
            if (Debugger.IsAttached) Console.ReadLine();
        }
    }
}
