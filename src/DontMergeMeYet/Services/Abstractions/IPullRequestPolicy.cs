using Octokit;

namespace DontMergeMeYet.Services.Abstractions
{
    public interface IPullRequestPolicy
    {
        (CommitState state, string description) GetStatus(PullRequestContext context);
    }
}
