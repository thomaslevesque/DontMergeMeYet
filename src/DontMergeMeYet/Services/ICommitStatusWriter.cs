using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services
{
    public interface ICommitStatusWriter
    {
        Task WriteCommitStatusAsync(PullRequestContext context, CommitState state, string description);
    }
}
