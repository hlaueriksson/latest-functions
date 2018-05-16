using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery;
using Newtonsoft.Json.Linq;

namespace LatestFunctions.Queries
{
    public class GitHubData
    {
        public GitHubRepo Repo { get; set; }
        public GitHubCommit Commit { get; set; }
    }

    public class GitHubRepo
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

    public class GitHubCommit
    {
        public string Repo { get; set; }
        public string Sha { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
    }

    public class GitHubQuery : IQuery<GitHubData>
    {
    }

    public interface IGitHubQueryHandlerConfiguration
    {
        string GitHubQueryHandlerUsername { get; }
    }

    public class GitHubQueryHandler : IQueryHandler<GitHubQuery, GitHubData>
    {
        private readonly IGitHubQueryHandlerConfiguration _config;
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string Uri = "https://api.github.com/users/{0}/events";

        public GitHubQueryHandler(IGitHubQueryHandlerConfiguration config)
        {
            _config = config;
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", _config.GitHubQueryHandlerUsername);
        }

        public async Task<GitHubData> HandleAsync(GitHubQuery query)
        {
            var uri = string.Format(Uri, _config.GitHubQueryHandlerUsername);

            var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JArray.Parse(content);
            var createEvents = json.SelectTokens("$.[?(@.type == 'CreateEvent')]");
            var pushEvents = json.SelectTokens("$.[?(@.type == 'PushEvent')]");

            return new GitHubData
            {
                Repo = GetRepo(createEvents),
                Commit = GetCommit(pushEvents)
            };
        }

        private GitHubRepo GetRepo(IEnumerable<JToken> createEvents)
        {
            var name = createEvents.FirstOrDefault()?.SelectToken("repo.name")?.Value<string>();

            return new GitHubRepo
            {
                Name = name,
                Link = name != null ? $"https://github.com/{name}" : null
            };
        }

        private GitHubCommit GetCommit(IEnumerable<JToken> pushEvents)
        {
            var repo = pushEvents.FirstOrDefault()?.SelectToken("repo.name")?.Value<string>();
            var sha = pushEvents.FirstOrDefault()?.SelectToken("payload.commits[0].sha")?.Value<string>();

            return new GitHubCommit
            {
                Repo = repo,
                Sha = sha,
                Message = pushEvents.FirstOrDefault()?.SelectToken("payload.commits[0].message")?.Value<string>(),
                Link = sha != null ? $"https://github.com/{repo}/commit/{sha}" : null
            };
        }
    }
}