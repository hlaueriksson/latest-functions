using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommandQuery;

namespace Latest.Queries
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

    public class BlogQueryHandler : IQueryHandler<BlogQuery, BlogData>
    {
        public async Task<BlogData> HandleAsync(BlogQuery query)
        {
            var uri = "http://conductofcode.io/feed.xml";

            var client = new HttpClient();
            var response = await client.GetAsync(uri);

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