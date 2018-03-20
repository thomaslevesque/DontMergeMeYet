using System.Threading.Tasks;
using DontMergeMeYet.Models;
using DontMergeMeYet.Models.Github.Webhooks;

namespace DontMergeMeYet.Services
{
    public interface IPullRequestInfoProvider
    {
        Task<PullRequestInfo> GetPullRequestInfoAsync(int installationId, PullRequestEventPayload payload);
    }
}
