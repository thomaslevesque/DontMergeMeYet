using System.Linq;
using System.Threading.Tasks;
using DontMergeMeYet.Models;
using DontMergeMeYet.Models.Github;
using DontMergeMeYet.Models.Github.Webhooks;
using Newtonsoft.Json;

namespace DontMergeMeYet.Services
{
    class PullRequestInfoProvider : IPullRequestInfoProvider
    {
        private readonly IGithubClientCache _clientCache;

        public PullRequestInfoProvider(IGithubClientCache clientCache)
        {
            _clientCache = clientCache;
        }

        public async Task<PullRequestInfo> GetPullRequestInfoAsync(int installationId, PullRequestEventPayload payload)
        {
            var commits = await GetCommitsAsync(installationId, payload.PullRequest.CommitsUrl);
            return new PullRequestInfo
            {
                Title = payload.PullRequest.Title,
                SourceRepositoryFullName = payload.Repository.FullName,
                Head = payload.PullRequest.Head.Sha,
                CommitMessages = commits.Select(c => c.Details.Message)
            };
        }

        private async Task<Commit[]> GetCommitsAsync(int installationId, string commitsUrl)
        {
            var client = await _clientCache.GetClientForInstallationAsync(installationId);
            using (var response = await client.GetAsync(commitsUrl))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Commit[]>(json);
            }
        }
    }
}