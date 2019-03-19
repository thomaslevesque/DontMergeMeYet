using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DontMergeMeYet.Extensions;
using DontMergeMeYet.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
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

        [FunctionName("GithubWebhook")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage request,
            ILogger logger,
            ExecutionContext executionContext)
        {
            var services = GetServices(executionContext);

            string eventName = request.Headers.GetValueOrDefault("X-GitHub-Event");
            string deliveryId = request.Headers.GetValueOrDefault("X-GitHub-Delivery");
            string signature = request.Headers.GetValueOrDefault("X-Hub-Signature");

            logger.LogInformation("Webhook delivery: Delivery id = '{DeliveryId}', Event name = '{EventName}'", deliveryId, eventName);

            var payloadBytes = await request.Content.ReadAsByteArrayAsync();
            if (!services.PayloadValidator.IsPayloadSignatureValid(payloadBytes, signature))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, "Invalid signature");
            }

            if (eventName == "pull_request")
            {
                var payload = await DeserializeBody<PullRequestEventPayload>(request.Content);
                if (PullRequestActions.Contains(payload.Action))
                {
                    try
                    {
                        logger.LogInformation("Handling action '{PayloadAction}' for pull request #{PullRequestNumber}", payload.Action, payload.Number);
                        var connection = await services.GithubConnectionCache.GetConnectionAsync(payload.Installation.Id);
                        var context = new PullRequestContext(payload, connection, logger);
                        await services.PullRequestHandler.HandleWebhookEventAsync(context);
                        logger.LogInformation("Finished handling action '{PayloadAction}' for pull request #{PullRequestNumber}", payload.Action, payload.Number);
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

        private static Services _services;
        private static Services GetServices(ExecutionContext context)
        {
            if (_services != null)
                return _services;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var options = Options.Create(configuration.GetSection("Github").Get<GithubSettings>());

            var githubConnectionCache = new GithubConnectionCache(new GithubAppTokenService(options));

            var pullRequestHandler = new PullRequestHandler(
                    new PullRequestInfoProvider(),
                    new RepositorySettingsProvider(),
                    new WorkInProgressPullRequestPolicy(),
                    new CommitStatusWriter(options));

            var payloadValidator = new GithubPayloadValidator(options);

            return _services =
                new Services(configuration, options, githubConnectionCache, pullRequestHandler, payloadValidator);
        }

        private class Services
        {
            public Services(IConfigurationRoot configuration, IOptions<GithubSettings> options, GithubConnectionCache githubConnectionCache, PullRequestHandler pullRequestHandler, GithubPayloadValidator payloadValidator)
            {
                Configuration = configuration;
                Options = options;
                GithubConnectionCache = githubConnectionCache;
                PullRequestHandler = pullRequestHandler;
                PayloadValidator = payloadValidator;
            }

            public IConfigurationRoot Configuration { get; }
            public IOptions<GithubSettings> Options { get; }
            public GithubConnectionCache GithubConnectionCache { get; }
            public PullRequestHandler PullRequestHandler { get; }
            public GithubPayloadValidator PayloadValidator { get; }
        }
    }
}
