using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    class GithubClientCache : IGithubClientCache
    {
        private readonly IGithubAppTokenService _tokenService;
        private readonly ConcurrentDictionary<int, HttpClient> _cache;

        public GithubClientCache(IGithubAppTokenService tokenService)
        {
            _tokenService = tokenService;
            _cache = new ConcurrentDictionary<int, HttpClient>();
        }

        public async Task<HttpClient> GetClientForInstallationAsync(int installationId)
        {
            if (!_cache.TryGetValue(installationId, out var client))
            {
                var token = await _tokenService.GetTokenForInstallationAsync(installationId);
                client = new HttpClient
                {
                    DefaultRequestHeaders =
                    {
                        Authorization = new AuthenticationHeaderValue("Token", token),
                        UserAgent =
                        {
                            ProductInfoHeaderValue.Parse("DontMergeMeYet")
                        }
                    }
                };
                _cache[installationId] = client;
            }

            return client;
        }

        public void Dispose()
        {
            foreach (var client in _cache.Values)
            {
                client.Dispose();
            }
        }
    }
}