using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DontMergeMeYet.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Octokit.Internal;

namespace DontMergeMeYet
{
    public static class GithubWebhook
    {
        private static readonly string[] PullRequestActions =
        {
            "labeled",
            "unlabeled",
            "opened",
            "edited",
            "reopened",
            "synchronize"
        };

        private static readonly IPullRequestHandler PullRequestHandler =
            new PullRequestHandler(
                new GithubConnectionCache(new GithubAppTokenService()),
                new WorkInProgressPullRequestPolicy());

        private static readonly IGithubPayloadValidator PayloadValidator =
            new GithubPayloadValidator();

        [FunctionName("GithubWebhook")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST")]
            HttpRequestMessage request,
            TraceWriter log)
        {
            string eventName = request.Headers.GetValues("X-GitHub-Event").FirstOrDefault();
            string deliveryId = request.Headers.GetValues("X-GitHub-Delivery").FirstOrDefault();
            string signature = request.Headers.GetValues("X-Hub-Signature").FirstOrDefault();

            log.Info($"Webhook delivery: Delivery id = '{deliveryId}', Event name = '{eventName}'");

            if (eventName == "pull_request")
            {
                var payloadBytes = await request.Content.ReadAsByteArrayAsync();
                if (!PayloadValidator.IsPayloadSignatureValid(payloadBytes, signature))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Invalid signature");
                }

                var payload = DeserializeBody<PullRequestPayload>(payloadBytes);
                if (PullRequestActions.Contains(payload.Action))
                {
                    log.Info($"Handling pull request action '{payload.Action}'");
                    await PullRequestHandler.HandleWebhookEventAsync(payload);
                }
                else
                {
                    log.Info($"Ignoring pull request action '{payload.Action}'");
                }
            }
            else
            {
                log.Info($"Unknown event '{eventName}', ignoring");
            }

            return request.CreateResponse(HttpStatusCode.NoContent);
        }

        private static T DeserializeBody<T>(byte[] bytes)
        {
            string json = Encoding.UTF8.GetString(bytes);
            var serializer = new SimpleJsonSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}
