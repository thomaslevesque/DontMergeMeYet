using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DontMergeMeYet.Extensions;
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

        private static readonly IGithubSettingsProvider SettingsProvider =
            new DefaultGithubSettingsProvider();

        private static readonly IGithubConnectionCache GithubConnectionCache =
            new GithubConnectionCache(new GithubAppTokenService(SettingsProvider));

        private static readonly IPullRequestHandler PullRequestHandler =
            new PullRequestHandler(
                new PullRequestInfoProvider(),
                new WorkInProgressPullRequestPolicy(),
                new CommitStatusWriter(SettingsProvider));

        private static readonly IGithubPayloadValidator PayloadValidator =
            new GithubPayloadValidator(SettingsProvider);

        [FunctionName("GithubWebhook")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST")]
            HttpRequestMessage request,
            TraceWriter log)
        {
            string eventName = request.Headers.GetValueOrDefault("X-GitHub-Event");
            string deliveryId = request.Headers.GetValueOrDefault("X-GitHub-Delivery");
            string signature = request.Headers.GetValueOrDefault("X-Hub-Signature");

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
                    var connection = await GithubConnectionCache.GetConnectionAsync(payload.Installation.Id);
                    var context = new PullRequestContext(payload, connection);
                    await PullRequestHandler.HandleWebhookEventAsync(context);
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
