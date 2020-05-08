using System.Threading.Tasks;

namespace DontMergeMeYet.Services.Abstractions
{
    public interface IRepositorySettingsProvider
    {
        Task<RepositorySettings> GetRepositorySettingsAsync(PullRequestContext context);
    }
}
