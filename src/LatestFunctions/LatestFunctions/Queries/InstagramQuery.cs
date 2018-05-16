using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandQuery;
using Newtonsoft.Json.Linq;

namespace LatestFunctions.Queries
{
    public class InstagramData
    {
        public string Html { get; set; }
    }

    public class InstagramQuery : IQuery<InstagramData>
    {
    }

    public interface IInstagramQueryHandlerConfiguration
    {
        string InstagramQueryHandlerUsername { get; }
    }

    public class InstagramQueryHandler : IQueryHandler<InstagramQuery, InstagramData>
    {
        private readonly IInstagramQueryHandlerConfiguration _config;
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string LinkUri = "https://www.instagram.com/{0}/";
        private const string HtmlUri = "https://api.instagram.com/oembed/?url={0}";

        public InstagramQueryHandler(IInstagramQueryHandlerConfiguration config)
        {
            _config = config;
        }

        public async Task<InstagramData> HandleAsync(InstagramQuery query)
        {
            var code = await GetCode();
            var link = GetLink(code);
            var html = await GetHtml(link);

            return new InstagramData
            {
                Html = html
            };
        }

        private async Task<string> GetCode()
        {
            var response = await HttpClient.GetAsync(string.Format(LinkUri, _config.InstagramQueryHandlerUsername));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var pattern = @"<script type=""text\/javascript"">.*?""shortcode"":""(.*?)"".*<\/script>";
            var matches = Regex.Matches(content, pattern);

            return matches[0].Groups[1].Value;
        }

        private string GetLink(string code)
        {
            return $"https://www.instagram.com/p/{code}/";
        }

        private async Task<string> GetHtml(string link)
        {
            var uri = string.Format(HtmlUri, link);

            var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            return json["html"].Value<string>();
        }
    }
}