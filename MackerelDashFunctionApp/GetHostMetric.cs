using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace MackerelDashFunctionApp
{
    public static class GetHostMetric
    {
        // https://mackerel.io/ja/api-docs/entry/host-metrics#get
        [FunctionName("GetHostMetric")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "hosts/{hostId}/metrics")]HttpRequestMessage req,
            string hostId,
            [Table("hostmetrics", Connection = "StorageConnectionString")]IQueryable<HostMetric> inTable,
            TraceWriter log)
        {
            // XXX 良い感じに取りたい
            var name = req.GetQueryNameValuePairs().First(x => x.Key == "name").Value;
            var from = req.GetQueryNameValuePairs().First(x => x.Key == "from").Value;
            var to = req.GetQueryNameValuePairs().First(x => x.Key == "to").Value;

            var query = inTable.Where(x => x.PartitionKey == hostId)
                .Where(x => x.RowKey.CompareTo(from) >= 0)
                .Where(x => x.RowKey.CompareTo(to) < 0)
                .Where(x => x.name == name)
                .OrderBy(x => x.RowKey)
                ;
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                metrics = query.Select(x => new
                {
                    time = x.time,
                    value = x.value
                }).ToList()
            });
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
