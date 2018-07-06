using System;
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
    }

    public class GitHubRepo
    {
        public string Name { get; set; }
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
        private const string Uri = "https://api.github.com/users/{0}/repos?sort=created";

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
            var repo = json.FirstOrDefault(x => !x.Value<bool>("fork"));

            return new GitHubData
            {
                Repo = new GitHubRepo
                {
                    Name = repo.Value<string>("full_name"),
                    Link = repo.Value<string>("html_url")
                }
            };
        }
    }
}