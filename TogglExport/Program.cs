using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TogglExport {
    public class Program {
        public static async Task Main(string[] args) {
            await TogglExport.Run(args);
            if (Debugger.IsAttached) Console.ReadLine();
        }
    }
}
