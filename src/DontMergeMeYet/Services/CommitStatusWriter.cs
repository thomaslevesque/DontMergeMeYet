using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services
{
    class CommitStatusWriter : ICommitStatusWriter
    {
        public async Task WriteCommitStatusAsync(PullRequestEventContext context, CommitState state, string description)
        {
            var client = new CommitStatusClient(new ApiConnection(context.GithubConnection));
            var status = new NewCommitStatus
            {
                State = state,
                Description = description,
                Context = GithubSettings.Default.StatusContext
            };
            await client.Create(context.Payload.Repository.Id, context.Payload.PullRequest.Head.Sha, status);
        }
    }
}