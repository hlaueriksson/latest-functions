using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using LatestFunctions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LatestFunctions
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(typeof(Query).Assembly.GetQueryProcessor(GetServiceCollection()));

        [FunctionName("Query")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequest req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }

        private static IServiceCollection GetServiceCollection()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configuration = new Configuration
            {
                BlogQueryHandlerFeedUri = config["BlogQueryHandlerFeedUri"],
                GitHubQueryHandlerUsername = config["GitHubQueryHandlerUsername"],
                InstagramQueryHandlerUsername = config["InstagramQueryHandlerUsername"]
            };

            return new ServiceCollection()
                .AddSingleton<IBlogQueryHandlerConfiguration>(_ => configuration)
                .AddSingleton<IGitHubQueryHandlerConfiguration>(_ => configuration)
                .AddSingleton<IInstagramQueryHandlerConfiguration>(_ => configuration);
        }
    }
}