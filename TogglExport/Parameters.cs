namespace TogglExport {
    public class Parameters {
        public bool Valid { get; private set; }

        public static Parameters FromArgs(string[] args) {
            return new Parameters {
                Valid = false
            };
        }
    }
}
