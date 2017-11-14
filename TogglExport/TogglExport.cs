using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TogglExport.Models;

namespace TogglExport {
    public static class TogglExport {
        public static async Task Run(string[] args) {
            var parameters = Parameters.FromArgs(args);
            if (!parameters.Valid) {
                Console.WriteLine("Example usage: toggl-export --api-key xxxx");
                return;
            }

            var timeEntries = await FetchTimeEntries(parameters.ApiKey);
            var projects = await FetchProjects(parameters.ApiKey, timeEntries.Select(entry => entry.Wid).Distinct());
            var outputEntries = MapToOutputEntries(timeEntries, projects);
        }
        
        private static async Task<IList<TimeEntry>> FetchTimeEntries(string apiKey) {
            var client = new TogglClient(apiKey);

            var request = new RestRequest("time_entries");
            request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 1)));
            request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 30)));

            return await client.Execute<List<TimeEntry>>(request);
        }

        private static string DateTimeToIso(DateTime value) {
            return value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }

        private static async Task<IList<Project>> FetchProjects(string apiKey, IEnumerable<int> workspaceIds) {
            var tasks = workspaceIds.Select(async id => {
                var client = new TogglClient(apiKey);

                var request = new RestRequest("workspaces");
                request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 1)));
                request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 30)));

                return await client.Execute<List<Project>>(request);
            });

            var projects = await Task.WhenAll(tasks);
            return projects.SelectMany(p => p).ToList();
        }
        
        private static IList<OutputEntry> MapToOutputEntries(IEnumerable<TimeEntry> timeEntries, IEnumerable<Project> projects) {
            return timeEntries.GroupBy(entry => new { entry.Start.Date, entry.Description, entry.Pid })
                .Select(item => new OutputEntry {
                    Date = item.Key.Date,
                    Code = GetProjectName(item.Key.Pid),
                    Description = item.Key.Description,
                    Duration = RoundToHalfHours(item.Sum(i => i.Duration)),
                    Identifier = ParseIdentifier(item.Key.Description)
                })
                .ToList();

            string GetProjectName(int projectId) => projects.Single(p => p.Id == projectId).Name;

            double RoundToHalfHours(int durationInSeconds) => Math.Round(durationInSeconds / 3600.0 * 2) / 2;

            string ParseIdentifier(string description) {
                if (string.IsNullOrEmpty(description)) { return null; }

                var first = description.Split(' ').First();

                return Regex.IsMatch(first, @"([A-Z]*-\d*|\d*)") ? first : null;
            }
        }
    }
}
