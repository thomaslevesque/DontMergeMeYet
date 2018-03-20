using System.Net.Http;
using System.Threading.Tasks;
using DontMergeMeYet.Models.Github;
using Newtonsoft.Json;

namespace DontMergeMeYet.Services
{
    class CommitStatusWriter : ICommitStatusWriter
    {
        private readonly IGithubClientCache _clientCache;

        public CommitStatusWriter(IGithubClientCache clientCache)
        {
            _clientCache = clientCache;
        }

        public async Task WriteCommitStatusAsync(int installationId, string repositoryFullName, string commitSha1,
            CommitStatus status)
        {
            var client = await _clientCache.GetClientForInstallationAsync(installationId);
            var url = $"https://api.github.com/repos/{repositoryFullName}/statuses/{commitSha1}";
            var content = new StringContent(JsonConvert.SerializeObject(status));
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }
}