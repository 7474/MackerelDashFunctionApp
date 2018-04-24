using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace MackerelDashFunctionApp
{
    public static class PostTsdb
    {
        [FunctionName("PostTsdb")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "tsdb")]HttpRequestMessage req,
            [Table("hostmetrics", Connection = "StorageConnectionString")]ICollector<HostMetric> outTable,
            TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();
            var metrics = data as IEnumerable<dynamic>;

            if (metrics == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a host metric collection in the request body");
            }

            foreach (var metric in metrics)
            {
                outTable.Add(new HostMetric()
                {
                    // XXX Key—v‘f2‚Â‚¾‚Æ‚¢‚¢Š´‚¶‚É‚È‚ç‚È‚¢
                    PartitionKey = metric.name + "-" + metric.hostId,
                    RowKey = metric.time,
                    hostId = metric.hostId,
                    name = metric.name,
                    time = metric.time,
                    value = metric.value,
                });
            }
            return req.CreateResponse(HttpStatusCode.Created);
        }

        public class HostMetric : TableEntity
        {
            public string hostId { get; set; }
            public string name { get; set; }
            public ulong time { get; set; }
            public decimal value { get; set; }
        }
    }
}
