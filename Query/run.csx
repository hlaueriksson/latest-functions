#r "LatestApi.dll"

using System.Net;
using System.Reflection;
using CommandQuery.AzureFunctions;

static QueryFunction func = new QueryFunction(Assembly.Load("LatestApi").GetQueryProcessor());

public static async Task<HttpResponseMessage> Run(string queryName, HttpRequestMessage req, TraceWriter log)
{
    return await func.Handle(queryName, req, log);
}