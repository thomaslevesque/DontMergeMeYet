using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    public interface IRepositorySettingsProvider
    {
        Task<RepositorySettings> GetRepositorySettingsAsync(PullRequestContext context);
    }
}
