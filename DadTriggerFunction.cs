using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class DadTriggerFunction
    {
        [FunctionName("DadTriggerFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Console.WriteLine("Starting GetData.");
            string baseUrl = "https://icanhazdadjoke.com/";

            dynamic data;
            HttpClient client;
            MediaTypeWithQualityHeaderValue header = new MediaTypeWithQualityHeaderValue("application/json");
            
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(header);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "vpi-DadTriggerFunction");
                using (var res = await client.GetAsync(baseUrl))
                {
                    using (var content = res.Content)
                    {
                        Console.WriteLine("We are now here.");
                        string input = await content.ReadAsStringAsync();
                        data = JsonConvert.DeserializeObject(input);
                        if (data != null)
                        {
                            Console.WriteLine(data.joke);
                        }
                    }
                }

                return data.joke != null
                    ? (ActionResult)new OkObjectResult($"{data.joke}")
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

        }
    }
}

