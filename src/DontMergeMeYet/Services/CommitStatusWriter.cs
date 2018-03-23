using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services
{
    class CommitStatusWriter : ICommitStatusWriter
    {
        private readonly IGithubSettingsProvider _settingsProvider;

        public CommitStatusWriter(IGithubSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public async Task WriteCommitStatusAsync(PullRequestContext context, CommitState state, string description)
        {
            var client = new CommitStatusClient(new ApiConnection(context.GithubConnection));
            var status = new NewCommitStatus
            {
                State = state,
                Description = description,
                Context = _settingsProvider.Settings.StatusContext
            };
            await client.Create(context.Payload.Repository.Id, context.Payload.PullRequest.Head.Sha, status);
        }
    }
}