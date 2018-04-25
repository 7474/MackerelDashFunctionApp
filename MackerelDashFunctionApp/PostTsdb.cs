using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MackerelDashFunctionApp
{
    public static class PostTsdb
    {
        [FunctionName("PostTsdb")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "tsdb")]HttpRequestMessage req,
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
                var hostMetric = new HostMetric()
                {
                    PartitionKey = metric.hostId,
                    RowKey = metric.time + "+" + metric.name,
                    hostId = metric.hostId,
                    name = metric.name,
                    time = metric.time,
                    value = metric.value,
                };
                log.Verbose(JsonConvert.SerializeObject(metric));
                log.Verbose(JsonConvert.SerializeObject(hostMetric));
                outTable.Add(hostMetric);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        public class HostMetric : TableEntity
        {
            public string hostId { get; set; }
            public string name { get; set; }
            public long time { get; set; }
            public double value { get; set; }
        }
    }
}
