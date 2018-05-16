using LatestFunctions.Queries;

namespace LatestFunctions
{
    public class Configuration : IBlogQueryHandlerConfiguration, IGitHubQueryHandlerConfiguration, IInstagramQueryHandlerConfiguration
    {
        public string BlogQueryHandlerFeedUri { get; set; }
        public string GitHubQueryHandlerUsername { get; set; }
        public string InstagramQueryHandlerUsername { get; set; }
    }
}