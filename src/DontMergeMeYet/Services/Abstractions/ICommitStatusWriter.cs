using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services.Abstractions
{
    public interface ICommitStatusWriter
    {
        Task WriteCommitStatusAsync(PullRequestContext context, CommitState state, string description);
    }
}
