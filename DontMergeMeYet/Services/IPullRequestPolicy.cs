using Octokit;

namespace DontMergeMeYet.Services
{
    public interface IPullRequestPolicy
    {
        NewCommitStatus GetStatus(PullRequestInfo pullRequest);
    }
}
