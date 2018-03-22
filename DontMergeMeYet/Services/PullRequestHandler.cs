using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;

namespace DontMergeMeYet.Services
{
    class PullRequestHandler : IPullRequestHandler
    {
        private readonly IGithubAppTokenService _tokenService;
        private readonly IPullRequestPolicy _pullRequestPolicy;

        public PullRequestHandler(IGithubAppTokenService tokenService, IPullRequestPolicy pullRequestPolicy)
        {
            _tokenService = tokenService;
            _pullRequestPolicy = pullRequestPolicy;
        }

        public async Task HandleWebhookEventAsync(PullRequestPayload payload)
        {
            var connection = await GetGithubConnectionAsync(payload.Installation.Id);
            var prInfo = await GetPullRequestInfoAsync(connection, payload);
            var status = _pullRequestPolicy.GetStatus(prInfo);
            await WriteCommitStatusAsync(connection, payload.Repository.Id, payload.PullRequest.Head.Sha, status);
        }

        private async Task<IConnection> GetGithubConnectionAsync(int installationId)
        {
            string token = await _tokenService.GetTokenForInstallationAsync(installationId);
            var credentials = new Credentials(token);
            var credentialStore = new InMemoryCredentialStore(credentials);
            var connection = new Connection(new ProductHeaderValue("DontMergeMeYet"), credentialStore);
            return connection;
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