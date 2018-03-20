using System.Threading.Tasks;
using DontMergeMeYet.Models.Github;

namespace DontMergeMeYet.Services
{
    public interface ICommitStatusWriter
    {
        Task WriteCommitStatusAsync(int installationId, string repositoryFullName, string commitSha1,
            CommitStatus status);
    }
}
