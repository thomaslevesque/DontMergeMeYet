using System.Threading.Tasks;
using Octokit;

namespace DontMergeMeYet.Services
{
    public interface IGithubConnectionCache
    {
        Task<IConnection> GetConnectionAsync(long installationId);
    }
}
