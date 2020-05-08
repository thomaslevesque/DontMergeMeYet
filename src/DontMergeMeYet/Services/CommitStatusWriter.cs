using System.Threading.Tasks;
using DontMergeMeYet.Services.Abstractions;
using Microsoft.Extensions.Options;
using Octokit;

namespace DontMergeMeYet.Services
{
    class CommitStatusWriter : ICommitStatusWriter
    {
        private readonly GithubSettings _settings;

        public CommitStatusWriter(IOptions<GithubSettings> options)
        {
            _settings = options.Value;
        }

        public async Task WriteCommitStatusAsync(PullRequestContext context, CommitState state, string description)
        {
            var client = new CommitStatusClient(new ApiConnection(context.GithubConnection));
            var status = new NewCommitStatus
            {
                State = state,
                Description = description,
                Context = _settings.StatusContext
            };
            await client.Create(context.Payload.Repository.Id, context.Payload.PullRequest.Head.Sha, status);
        }
    }
}