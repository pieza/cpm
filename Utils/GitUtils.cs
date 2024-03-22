using cpm.DTOs;
using LibGit2Sharp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace cpm.Utils
{
    static class GitUtils
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public static async Task<List<TagDTO>> GetRepositoryTagsAsync(string owner, string repo)
        {
            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/tags";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# HttpClient");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            var response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tags = JsonSerializer.Deserialize<List<TagDTO>>(json, _jsonSerializerOptions);

            return tags ?? [];
        }

        public static void CloneRepository(string owner, string repo, string localPath, string tagName)
        {
            var repoUrl = BuildGitUrl(owner, repo);
            CloneRepository(repoUrl, localPath, tagName);
        }

        public static void CloneRepository(string repositoryUrl, string localPath, string tagName)
        {
            Repository.Clone(repositoryUrl, localPath, new CloneOptions
            {
                Checkout = false
            });

            using var repo = new Repository(localPath);
            var commit = repo.Tags[tagName].Target as Commit ?? throw new ArgumentException($"Couldn't find tag '{tagName}' on the repository.");

            Commands.Checkout(repo, commit);
        }

        public static TagDTO GetLatestTag(List<TagDTO> tags)
        {
            tags.Sort((tag1, tag2) => CompareVersions(tag2.Name, tag1.Name));
            return tags[0];
        }

        public static string BuildGitUrl(string owner, string repo)
        {
            return $"https://github.com/{owner}/{repo}.git";
        }

        private static int CompareVersions(string version1, string version2)
        {
            var version1Parts = version1.Split('.');
            var version2Parts = version2.Split('.');

            for (int i = 0; i < Math.Max(version1Parts.Length, version2Parts.Length); i++)
            {
                int part1 = i < version1Parts.Length ? int.Parse(version1Parts[i]) : 0;
                int part2 = i < version2Parts.Length ? int.Parse(version2Parts[i]) : 0;

                if (part1 != part2)
                {
                    return part1.CompareTo(part2);
                }
            }

            return 0;
        }
    }
}
