using System;
using System.Collections.Generic;

namespace TogglExport {
    public static class TogglExport {
        public static void Run(string[] args) {
            var parameters = Parameters.FromArgs(args);
            if (!parameters.Valid) {
                Console.WriteLine("Example usage: toggl-export --api-key xxxx");
                return;
            }

            var timeEntries = FetchTimeEntries(parameters.ApiKey);
        }

        private static IList<TimeEntry> FetchTimeEntries(string apiKey) {
            return new List<TimeEntry>();
        }
    }
}
