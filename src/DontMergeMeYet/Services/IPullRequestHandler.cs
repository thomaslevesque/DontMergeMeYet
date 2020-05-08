using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    public interface IPullRequestHandler
    {
        Task HandleWebhookEventAsync(PullRequestContext context);
    }
}