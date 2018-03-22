using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services
{
    class PullRequestHandler : IPullRequestHandler
    {
        private readonly IGithubConnectionCache _connectionCache;
        private readonly IPullRequestPolicy _pullRequestPolicy;

        public PullRequestHandler(IGithubConnectionCache connectionCache, IPullRequestPolicy pullRequestPolicy)
        {
            _connectionCache = connectionCache;
            _pullRequestPolicy = pullRequestPolicy;
        }

        public async Task HandleWebhookEventAsync(PullRequestPayload payload)
        {
            var connection = await _connectionCache.GetConnectionAsync(payload.Installation.Id);
            var prInfo = await GetPullRequestInfoAsync(connection, payload);
            var status = _pullRequestPolicy.GetStatus(prInfo);
            await WriteCommitStatusAsync(connection, payload.Repository.Id, payload.PullRequest.Head.Sha, status);
        }

        private async Task<PullRequestInfo> GetPullRequestInfoAsync(IConnection connection, PullRequestPayload payload)
        {
            var commits = await GetCommitsAsync(connection, payload.Repository.Id, payload.Number);
            return new PullRequestInfo
            {
                Title = payload.PullRequest.Title,
                SourceRepositoryFullName = payload.Repository.FullName,
                Head = payload.PullRequest.Head.Sha,
                CommitMessages = commits.Select(c => c.Commit.Message)
            };
        }

        private Task<IReadOnlyList<PullRequestCommit>> GetCommitsAsync(IConnection connection, long repositoryId, int pullRequestNumber)
        {
            var client = new GitHubClient(connection);
            return client.PullRequest.Commits(repositoryId, pullRequestNumber);
        }

        private async Task WriteCommitStatusAsync(IConnection connection, long repositoryId, string sha1, NewCommitStatus status)
        {
            var client = new CommitStatusClient(new ApiConnection(connection));
            await client.Create(repositoryId, sha1, status);
        }
    }
}