using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctionChaining
{
    public static class FunctionChaining
    {
        [FunctionName("FunctionChaining")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            string name = context.GetInput<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("FunctionChaining_Hello", name));
            outputs.Add(await context.CallActivityAsync<string>("FunctionChaining_Hello1", name));
            outputs.Add(await context.CallActivityAsync<string>("FunctionChaining_Hello2", name));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("FunctionChaining_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"[Inside Activity Funciton 1] Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("FunctionChaining_Hello1")]
        public static string SayHello1([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"[Inside Activity Funciton 2] Saying hello to {name}.");
            return $"Hello {name}!";
        }


        [FunctionName("FunctionChaining_Hello2")]
        public static string SayHello2([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"[Inside Activity Funciton 3] Saying hello to {name}.");
            return $"Hello {name}!";
        }




        [FunctionName("FunctionChaining_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
             string instanceId = await starter.StartNewAsync("FunctionChaining", 
                "Instance", "Abhishek");

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
 

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}