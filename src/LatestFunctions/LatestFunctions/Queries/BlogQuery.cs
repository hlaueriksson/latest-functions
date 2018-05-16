using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommandQuery;

namespace LatestFunctions.Queries
{
    public class BlogData
    {
        public string Title { get; set; }
        public DateTime Published { get; set; }
        public string Link { get; set; }
    }

    public class BlogQuery : IQuery<BlogData>
    {
    }

    public interface IBlogQueryHandlerConfiguration
    {
        string BlogQueryHandlerFeedUri { get; }
    }

    public class BlogQueryHandler : IQueryHandler<BlogQuery, BlogData>
    {
        private readonly IBlogQueryHandlerConfiguration _config;
        private static readonly HttpClient HttpClient = new HttpClient();

        public BlogQueryHandler(IBlogQueryHandlerConfiguration config)
        {
            _config = config;
        }

        public async Task<BlogData> HandleAsync(BlogQuery query)
        {
            var response = await HttpClient.GetAsync(_config.BlogQueryHandlerFeedUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var xml = XDocument.Parse(content);
            var item = xml.Root.Element("channel").Element("item");

            return new BlogData
            {
                Title = item.Element("title").Value,
                Published = DateTime.Parse(item.Element("pubDate").Value),
                Link = item.Element("link").Value
            };
        }
    }
}