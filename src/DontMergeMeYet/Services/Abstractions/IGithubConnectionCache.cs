using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services.Abstractions
{
    public interface IGithubConnectionCache
    {
        Task<IConnection> GetConnectionAsync(long installationId);
    }
}
