using System;

namespace TogglExport {
    public class Parameters {
        public bool Valid { get; private set; }
        public string ApiKey { get; set; }

        public static readonly Parameters InvalidParameters = new Parameters();

        public static Parameters FromArgs(string[] args) {
            return ValidateArgs(args) ? ParseArgs(args) : InvalidParameters;
        }

        private static bool ValidateArgs(string[] args) {
            return args.Length == 2 && args[0] == "--api-key";
        }

        private static Parameters ParseArgs(string[] args) {
            return new Parameters { Valid = true, ApiKey = args[1] };
        }
    }
}
