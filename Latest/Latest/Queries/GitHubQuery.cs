using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery;
using Newtonsoft.Json.Linq;

namespace Latest.Queries
{
    public class GitHubData
    {
        public GitHubRepo Repo { get; set; }
        public GitHubCommit Commit { get; set; }
    }

    public class GitHubRepo
    {
        public string Name { get; set; }
    }

    public class GitHubCommit
    {
        public string Repo { get; set; }
        public string Sha { get; set; }
        public string Message { get; set; }
    }

    public class GitHubQuery : IQuery<GitHubData>
    {
        public string Username { get; set; }
    }

    public class GitHubQueryHandler : IQueryHandler<GitHubQuery, GitHubData>
    {
        public async Task<GitHubData> HandleAsync(GitHubQuery query)
        {
            var uri = $"https://api.github.com/users/{query.Username}/events";

            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", query.Username);
            var response = await client.GetAsync(uri);

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
            return new GitHubRepo
            {
                Name = createEvents.FirstOrDefault()?.SelectToken("repo.name")?.Value<string>()
            };
        }

        private GitHubCommit GetCommit(IEnumerable<JToken> pushEvents)
        {
            return new GitHubCommit
            {
                Repo = pushEvents.FirstOrDefault()?.SelectToken("repo.name")?.Value<string>(),
                Sha = pushEvents.FirstOrDefault()?.SelectToken("payload.commits[0].sha")?.Value<string>(),
                Message = pushEvents.FirstOrDefault()?.SelectToken("payload.commits[0].message")?.Value<string>()
            };
        }
    }
}