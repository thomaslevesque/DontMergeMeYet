using DontMergeMeYet.Models;
using DontMergeMeYet.Models.Github;

namespace DontMergeMeYet.Services
{
    public interface IPullRequestChecker
    {
        CommitStatus GetStatus(PullRequestInfo pullRequest);
    }
}
