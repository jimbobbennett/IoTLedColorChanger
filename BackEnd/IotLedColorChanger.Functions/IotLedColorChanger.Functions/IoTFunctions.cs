using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Devices;

namespace IotLedColorChanger.Functions
{
    public static class IoTFunctions
    {
        static readonly string connectionString = Environment.GetEnvironmentVariable("iotHubConnectionString");
        static readonly string deviceName = Environment.GetEnvironmentVariable("iotHubDevice");

        static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);

        [FunctionName("UpdateLedRgb")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "rgb")]HttpRequest req, 
            ILogger log)
        {
            var json = JObject.Parse(await req.ReadAsStringAsync());
            var red = json["red"].Value<byte>();
            var green = json["green"].Value<byte>();
            var blue = json["blue"].Value<byte>();

            log.LogInformation($"Update LED to R:{red} G:{green} B:{blue}");

            return new OkResult();
        }
    }
}
