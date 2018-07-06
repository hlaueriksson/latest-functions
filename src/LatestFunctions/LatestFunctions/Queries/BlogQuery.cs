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
            var ns = xml.Root.Name.Namespace;
            var entry = xml.Root.Element(ns + "entry");

            return new BlogData
            {
                Title = entry.Element(ns + "title").Value,
                Published = DateTime.Parse(entry.Element(ns + "published").Value),
                Link = entry.Element(ns + "link").Attribute("href").Value
            };
        }
    }
}