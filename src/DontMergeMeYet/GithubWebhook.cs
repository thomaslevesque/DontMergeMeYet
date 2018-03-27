using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DontMergeMeYet.Extensions;
using DontMergeMeYet.Services;
using Microsoft.Azure.WebJobs;
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

        [FunctionName("GithubWebhook")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger("POST", WebHookType = "github")]
            HttpRequestMessage request,
            TraceWriter log)
        {
            string eventName = request.Headers.GetValueOrDefault("X-GitHub-Event");
            string deliveryId = request.Headers.GetValueOrDefault("X-GitHub-Delivery");

            log.Info($"Webhook delivery: Delivery id = '{deliveryId}', Event name = '{eventName}'");

            if (eventName == "pull_request")
            {
                var payload = await DeserializeBody<PullRequestPayload>(request.Content);
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

        private static async Task<T> DeserializeBody<T>(HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            var serializer = new SimpleJsonSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}
