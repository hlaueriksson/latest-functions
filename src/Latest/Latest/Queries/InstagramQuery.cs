using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery;
using Newtonsoft.Json.Linq;

namespace Latest.Queries
{
    public class InstagramData
    {
        public string Html { get; set; }
    }

    public class InstagramQuery : IQuery<InstagramData>
    {
    }

    public class InstagramQueryHandler : IQueryHandler<InstagramQuery, InstagramData>
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string LinkUri = "https://www.instagram.com/hlaueriksson/media/";
        private const string HtmlUri = "https://api.instagram.com/oembed/?url={0}";

        public async Task<InstagramData> HandleAsync(InstagramQuery query)
        {
            var link = await GetLink();
            var html = await GetHtml(link);

            return new InstagramData
            {
                Html = html
            };
        }

        private async Task<string> GetLink()
        {
            var response = await HttpClient.GetAsync(LinkUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var item = json.SelectToken("items[0]");

            return item["link"].Value<string>();
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