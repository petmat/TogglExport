using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading.Tasks;

namespace TogglExport {
    public class TogglClient {
        private const string BaseUrl = "https://www.toggl.com/api/v8";
        private const string Password = "api_token";

        private readonly string apiKey;

        public TogglClient(string apiKey) {
            this.apiKey = apiKey;
        }

        public async Task<T> Execute<T>(RestRequest request) {
            var client = new RestClient(BaseUrl) {
                Authenticator = new HttpBasicAuthenticator(apiKey, Password)
            };

            var response = await client.ExecuteTaskAsync<T>(request);

            if (response.IsSuccessful) {
                return response.Data;
            }

            if (response.ErrorException != null) {
                string message = $"Error retrieving response ({(int)response.StatusCode}). Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }

            throw new ApplicationException($"Error retrieving response ({(int)response.StatusCode}).");
        }

        public static string DateTimeToIso(DateTime value) {
            return value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }
    }
}
