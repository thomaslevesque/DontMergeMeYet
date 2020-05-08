using System.Threading.Tasks;

namespace DontMergeMeYet.Services.Abstractions
{
    public interface IPullRequestHandler
    {
        Task HandleWebhookEventAsync(PullRequestContext context);
    }
}