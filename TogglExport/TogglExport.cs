using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            Console.WriteLine("Fetching entries...");
            var timeEntries = await FetchTimeEntries(parameters.ApiKey);
            var projects = await FetchProjects(parameters.ApiKey, timeEntries.Select(entry => entry.Wid).Distinct());
            var outputEntries = MapToOutputEntries(timeEntries, projects);
            var lines = MapToLines(outputEntries);
            await File.WriteAllLinesAsync("output.txt", lines, Encoding.UTF8);
            Console.WriteLine("Done!");
        }

        private static async Task<IList<TimeEntry>> FetchTimeEntries(string apiKey) {
            var client = new TogglClient(apiKey);

            var request = new RestRequest("time_entries");
            request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 1)));
            request.AddParameter("start_date", DateTimeToIso(new DateTime(2017, 11, 30)));

            return (await client.Execute<List<TimeEntry>>(request)).Where(IsNotRunning).ToList();

            bool IsNotRunning(TimeEntry entry) => entry.Duration >= 0;
        }

        private static string DateTimeToIso(DateTime value) {
            return value.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }

        private static async Task<IList<Project>> FetchProjects(string apiKey, IEnumerable<int> workspaceIds) {
            var tasks = workspaceIds.Select(async id => {
                var client = new TogglClient(apiKey);

                var request = new RestRequest("workspaces/{workspaceId}/projects");
                request.AddUrlSegment("workspaceId", id);

                return await client.Execute<List<Project>>(request);
            });

            var projects = await Task.WhenAll(tasks);
            return projects.SelectMany(p => p).ToList();
        }
        
        private static IList<OutputEntry> MapToOutputEntries(IEnumerable<TimeEntry> timeEntries, IEnumerable<Project> projects) {
            return timeEntries.GroupBy(entry => new { entry.Start.Date, entry.Description, entry.Pid })
                .Select(item => {
                    (var identifier, var description) = SplitToIdentifierAndDescription(item.Key.Description);

                    return new OutputEntry {
                        Date = item.Key.Date,
                        Code = GetProjectName(item.Key.Pid),
                        Description = description,
                        Duration = RoundToHalfHours(item.Sum(i => i.Duration)),
                        Identifier = identifier
                    };
                })
                .ToList();

            string GetProjectName(int projectId) => projects.Single(p => p.Id == projectId).Name;

            double RoundToHalfHours(int durationInSeconds) => Math.Max(Math.Round(durationInSeconds / 3600.0 * 2) / 2, 0.5);

            (string, string) SplitToIdentifierAndDescription(string description) {
                if (string.IsNullOrEmpty(description)) { return (null, description); }

                var parts = description.Split(' ', 2).Take(2).ToList();

                return Regex.IsMatch(parts[0], @"([A-Z]+-\d+|\d+)") && parts.Count() > 1
                    ? (parts[0], parts[1]) 
                    : (null, description);
            }
        }

        private static IList<string> MapToLines(IEnumerable<OutputEntry> outputEntries) {
            return outputEntries
                .Select(e => $"{e.Date:dd:MM:yyyy}\t{e.Code}\t{e.Duration}\t{e.Description}\t\t{e.Identifier}")
                .ToList();
        }

    }
}
