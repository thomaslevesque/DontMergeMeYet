using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    class PullRequestHandler : IPullRequestHandler
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

        public async Task HandleWebhookEventAsync(PullRequestEventContext context)
        {
            var prInfo = await _prInfoProvider.GetPullRequestInfoAsync(context);
            var (state, description) = _pullRequestPolicy.GetStatus(prInfo);
            await _statusWriter.WriteCommitStatusAsync(context, state, description);
        }
    }
}