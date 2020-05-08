using System.Threading.Tasks;

namespace DontMergeMeYet.Services.Abstractions
{
    public interface IPullRequestInfoProvider
    {
        Task<PullRequestInfo> GetPullRequestInfoAsync(PullRequestContext context);
    }
}
