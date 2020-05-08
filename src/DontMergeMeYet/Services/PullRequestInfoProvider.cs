using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DontMergeMeYet.Services
{
    class PullRequestInfoProvider : IPullRequestInfoProvider
    {
        public async Task<PullRequestInfo> GetPullRequestInfoAsync(PullRequestContext context)
        {
            context.Logger.LogDebug("Getting commits for pull request {RepoName}#{PullRequestNumber}", context.Payload.Repository.FullName, context.Payload.Number);
            var commits = await GetCommitsAsync(context);
            context.Logger.LogDebug("Getting labels for pull request {RepoName}#{PullRequestNumber}", context.Payload.Repository.FullName, context.Payload.Number);
            var labels = await GetLabelsAsync(context);
            return new PullRequestInfo
            {
                Title = context.Payload.PullRequest.Title,
                Labels = labels.Select(l => l.Name),
                SourceRepositoryFullName = context.Payload.Repository.FullName,
                Head = context.Payload.PullRequest.Head.Sha,
                CommitMessages = commits.Select(c => c.Commit.Message)
            };
        }

        private Task<IReadOnlyList<PullRequestCommit>> GetCommitsAsync(PullRequestContext context)
        {
            var client = new GitHubClient(context.GithubConnection);
            return client.PullRequest.Commits(context.Payload.Repository.Id, context.Payload.PullRequest.Number);
        }

        private Task<IReadOnlyList<Label>> GetLabelsAsync(PullRequestContext context)
        {
            var client = new IssuesLabelsClient(new ApiConnection(context.GithubConnection));
            return client.GetAllForIssue(context.Payload.Repository.Id, context.Payload.PullRequest.Number);
        }
    }
}