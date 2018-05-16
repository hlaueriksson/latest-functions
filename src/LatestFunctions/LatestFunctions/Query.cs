using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions;
using LatestFunctions.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;

namespace LatestFunctions
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(typeof(Query).Assembly.GetQueryProcessor(GetContainerBuilder()));

        [FunctionName("Query")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "query/{queryName}")] HttpRequest req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }

        private static ContainerBuilder GetContainerBuilder()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new ContainerBuilder();
            builder
                .RegisterInstance(
                    new Configuration
                    {
                        BlogQueryHandlerFeedUri = config["BlogQueryHandlerFeedUri"],
                        GitHubQueryHandlerUsername = config["GitHubQueryHandlerUsername"],
                        InstagramQueryHandlerUsername = config["InstagramQueryHandlerUsername"]
                    })
                .As<IBlogQueryHandlerConfiguration>()
                .As<IGitHubQueryHandlerConfiguration>()
                .As<IInstagramQueryHandlerConfiguration>();

            return builder;
        }
    }
}