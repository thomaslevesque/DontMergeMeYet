using DontMergeMeYet.Extensions;
using DontMergeMeYet.Services.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.Internal;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DontMergeMeYet
{
    public class GithubWebhook
    {
        private static readonly string[] PullRequestActions =
{
            "labeled",
            "unlabeled",
            "opened",
            "edited",
            "reopened",
            "synchronize",
            "ready_for_review"
        };
        private readonly IOptions<GithubSettings> _options;
        private readonly IGithubConnectionCache _githubConnectionCache;
        private readonly IPullRequestHandler _pullRequestHandler;
        private readonly IGithubPayloadValidator _payloadValidator;

        public GithubWebhook(
            IOptions<GithubSettings> options,
            IGithubConnectionCache githubConnectionCache,
            IPullRequestHandler pullRequestHandler,
            IGithubPayloadValidator payloadValidator)
        {
            _options = options;
            _githubConnectionCache = githubConnectionCache;
            _pullRequestHandler = pullRequestHandler;
            _payloadValidator = payloadValidator;
        }

        [FunctionName(nameof(GithubWebhook))]
        public async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage request,
            ILogger logger)
        {
            string eventName = request.Headers.GetValueOrDefault("X-GitHub-Event");
            string deliveryId = request.Headers.GetValueOrDefault("X-GitHub-Delivery");
            string signature = request.Headers.GetValueOrDefault("X-Hub-Signature");

            logger.LogInformation("Webhook delivery: Delivery id = '{DeliveryId}', Event name = '{EventName}'", deliveryId, eventName);

            var payloadBytes = await request.Content.ReadAsByteArrayAsync();
            if (!_payloadValidator.IsPayloadSignatureValid(payloadBytes, signature))
            {
                logger.LogWarning("Invalid signature");
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, new HttpError("Invalid signature"));
            }

            if (eventName == "pull_request")
            {
                var payload = await DeserializeBody<PullRequestEventPayload>(request.Content);
                if (PullRequestActions.Contains(payload.Action))
                {
                    try
                    {
                        logger.LogInformation("Handling action '{PayloadAction}' for pull request {RepoName}#{PullRequestNumber}", payload.Action, payload.Repository.FullName, payload.Number);
                        var connection = await _githubConnectionCache.GetConnectionAsync(payload.Installation.Id);
                        var context = new PullRequestContext(payload.Repository.FullName, payload, connection, logger);
                        await _pullRequestHandler.HandleWebhookEventAsync(context);
                        logger.LogInformation("Finished handling action '{PayloadAction}' for pull request {RepoName}#{PullRequestNumber}", payload.Action, payload.Repository.FullName, payload.Number);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error processing pull request webhook event {DeliveryId}", deliveryId);
                        return request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError(ex.Message));
                    }
                }
                else
                {
                    logger.LogInformation("Ignoring pull request action '{PayloadAction}'", payload.Action);
                }
            }
            else
            {
                logger.LogInformation("Unknown event '{EventName}', ignoring", eventName);
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
