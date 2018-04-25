using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;

namespace MackerelDashFunctionApp
{
    public static class GetHost
    {
        [FunctionName("GetHost")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "hosts/{hostId}")]HttpRequestMessage req,
            string hostId,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                host = new
                {
                    createdAt = 0,
                    id = hostId,
                    status = "working",
                    memo = "",
                    roles = new { },
                    interfaces = new { },
                    isRetired = false,
                    displayName = hostId,
                    meta = new { }
                }
            });
        }
    }
}
