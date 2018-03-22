using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.Internal;

namespace DontMergeMeYet.Services
{
    class GithubConnectionCache : IGithubConnectionCache
    {
        private readonly IGithubAppTokenService _tokenService;
        private readonly MemoryCache _memoryCache;

        public GithubConnectionCache(IGithubAppTokenService tokenService)
        {
            _tokenService = tokenService;
            var options = new MemoryCacheOptions();
            var optionsAccessor = Options.Create(options);
            _memoryCache = new MemoryCache(optionsAccessor);
        }

        public Task<IConnection> GetConnectionAsync(int installationId)
        {
            return _memoryCache.GetOrCreateAsync(installationId, async cacheEntry =>
            {
                var token = await _tokenService.GetTokenForInstallationAsync(installationId);
                var credentials = new Credentials(token);
                var credentialStore = new InMemoryCredentialStore(credentials);
                IConnection connection = new Connection(new ProductHeaderValue("DontMergeMeYet"), credentialStore);
                cacheEntry.SetValue(connection);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return connection;
            });
        }
    }
}