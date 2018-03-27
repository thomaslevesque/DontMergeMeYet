using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    public class PullRequestHandler : IPullRequestHandler
    {
        private readonly IPullRequestInfoProvider _prInfoProvider;
        private readonly IPullRequestPolicy _pullRequestPolicy;
        private readonly ICommitStatusWriter _statusWriter;

        public PullRequestHandler(IPullRequestInfoProvider prInfoProvider, IPullRequestPolicy pullRequestPolicy, ICommitStatusWriter statusWriter)
        {
            _prInfoProvider = prInfoProvider;
            _pullRequestPolicy = pullRequestPolicy;
            _statusWriter = statusWriter;
        }

        public async Task HandleWebhookEventAsync(PullRequestContext context)
        {
            context.Log.Verbose($"Getting details for pull request #{context.Payload.Number}...");
            var prInfo = await _prInfoProvider.GetPullRequestInfoAsync(context);
            context.Log.Verbose($"Evaluating status for pull request #{context.Payload.Number}...");
            var (state, description) = _pullRequestPolicy.GetStatus(context, prInfo);
            context.Log.Info($"Status for pull request #{context.Payload.Number} is '{state}' ({description})");
            context.Log.Verbose($"Writing commit status for pull request #{context.Payload.Number}...");
            await _statusWriter.WriteCommitStatusAsync(context, state, description);
        }
    }
}