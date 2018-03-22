using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services
{
    public interface ICommitStatusWriter
    {
        Task WriteCommitStatusAsync(PullRequestEventContext context, CommitState state, string description);
    }
}
