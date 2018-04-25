using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace MackerelDashFunctionApp
{
    public static class PutHost
    {
        [FunctionName("PutHost")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "hosts/{hostId}")]Host host,
            [Table("hosts", Connection = "StorageConnectionString")]CloudTable outTable,
            TraceWriter log)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
            //if (string.IsNullOrEmpty(host.Name))
            //{
            //    return new HttpResponseMessage(HttpStatusCode.BadRequest)
            //    {
            //        Content = new StringContent("A non-empty Name must be specified.")
            //    };
            //};

            //log.Info($"PersonName={host.Name}");

            //TableOperation updateOperation = TableOperation.InsertOrReplace(host);
            //TableResult result = outTable.Execute(updateOperation);
            //return new HttpResponseMessage((HttpStatusCode)result.HttpStatusCode);
        }

        public class Host : TableEntity
        {
            public string Name { get; set; }
        }
    }
}
