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

        public Task<IConnection> GetConnectionAsync(long installationId)
        {
            return _memoryCache.GetOrCreateAsync(installationId, async cacheEntry =>
            {
                var token = await _tokenService.GetTokenForInstallationAsync(installationId);
                var userAgent = new ProductHeaderValue("DontMergeMeYet");
                IConnection connection = new Connection(userAgent)
                {
                    Credentials = new Credentials(token)
                };
                cacheEntry.SetValue(connection);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                return connection;
            });
        }
    }
}