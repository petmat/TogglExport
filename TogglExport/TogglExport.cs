using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TogglExport {
    public static class TogglExport {
        public static async Task Run(string[] args) {
            var parameters = Parameters.FromArgs(args);
            if (!parameters.Valid) {
                Console.WriteLine("Example usage: toggl-export --api-key xxxx");
                return;
            }

            var timeEntries = await FetchTimeEntries(parameters.ApiKey);
        }

        private static async Task<IList<TimeEntry>> FetchTimeEntries(string apiKey) {
            var client = new RestClient("https://www.toggl.com/api/v8") {
                Authenticator = new HttpBasicAuthenticator(apiKey, "api_token")
            };

            var request = new RestRequest("time_entries");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 1)));
            request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 30)));

            var response = await client.ExecuteTaskAsync<List<TimeEntry>>(request);

            return response.Data;
        }

        private static string DateTimeToIso(DateTime value) {
            return value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }
    }
}
